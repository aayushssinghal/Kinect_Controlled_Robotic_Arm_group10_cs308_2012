// Author :  Abhijit Jana
// Blog : http://abhijitjana.net
// Twitter : http://twitter.com/abhijitjana
//-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
namespace DepthImageStream
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Kinect Sensor
        KinectSensor sensor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += new EventHandler(MainWindow_Closed);
        }

        /// <summary>
        /// Handles the Closed event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.StartSensor();
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            }
            else
            {
                MessageBox.Show("No Device Connected !!");
            }
        }

        /// <summary>
        /// Handles the StatusChanged event of the KinectSensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.StatusChangedEventArgs"/> instance containing the event data.</param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.Title = e.Status.ToString();
            if (e.Status == KinectStatus.Connected)
            {
                this.StartSensor();
            }
            else
            {
                if (sensor != null)
                {
                    sensor.Stop();
                }
            }
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        private void StartSensor()
        {
            // Get the first Sensor
            sensor = KinectSensor.KinectSensors[0];
            sensor.DepthStream.Enable(DepthImageFormat.Resolution80x60Fps30);
            sensor.SkeletonStream.Enable();       
            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            sensor.Start();
        }

        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame == null)
                {
                    return;
                }
                
                byte[] pixelData = GetPixelData(depthImageFrame);

                depthImageControl.Source = BitmapSource.Create(depthImageFrame.Width, depthImageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, depthImageFrame.Width * 4);

            }
        }

        /// <summary>
        /// Gets the pixel data.
        /// </summary>
        /// <param name="depthImageFrame">The depth image frame.</param>
        /// <returns></returns>
        private byte[] GetPixelData(DepthImageFrame depthImageFrame)
        {
            //get the raw data from kinect with the depth for every pixel
            short[] rawDepthData = new short[depthImageFrame.PixelDataLength];
            depthImageFrame.CopyPixelDataTo(rawDepthData);

            //use depthFrame to create the image to display on-screen
            //depthFrame contains color information for all pixels in image
            //Height x Width x 4 (Red, Green, Blue, empty byte)
            Byte[] pixels = new byte[depthImageFrame.Height * depthImageFrame.Width * 4];

            //Bgr32  - Blue, Green, Red, empty byte
            //Bgra32 - Blue, Green, Red, transparency 
            //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

            //hardcoded locations to Blue, Green, Red (BGR) index positions       
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;

            for (int depthIndex = 0, colorIndex = 0;
                depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                depthIndex++, colorIndex += 4)
            {
                //get the player 
                int player = rawDepthData[depthIndex] & DepthImageFrame.PlayerIndexBitmask;

                //gets the depth value
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                //.9M or 2.95'
                if (depth <= 900)
                {
                    //we are very close
                    pixels[colorIndex + BlueIndex] = 255;
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 0;

                }
                // .9M - 2M or 2.95' - 6.56'
                else if (depth > 900 && depth < 2000)
                {
                    //we are a bit further away
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 255;
                    pixels[colorIndex + RedIndex] = 0;
                }
                // 2M+ or 6.56'+
                else if (depth > 2000)
                {
                    //we are the farthest
                    pixels[colorIndex + BlueIndex] = 0;
                    pixels[colorIndex + GreenIndex] = 0;
                    pixels[colorIndex + RedIndex] = 255;
                }


                //////equal coloring for monochromatic histogram
                //byte intensity = CalculateIntensityFromDepth(depth);
                //pixels[colorIndex + BlueIndex] = intensity;
                //pixels[colorIndex + GreenIndex] = intensity;
                //pixels[colorIndex + RedIndex] = intensity;


                //Color all players "gold"
                if (player > 0)
                {
                    pixels[colorIndex + BlueIndex] = Colors.Gold.B;
                    pixels[colorIndex + GreenIndex] = Colors.Gold.G;
                    pixels[colorIndex + RedIndex] = Colors.Gold.R;
                }

            }


            return pixels; 
        }
    }
}
