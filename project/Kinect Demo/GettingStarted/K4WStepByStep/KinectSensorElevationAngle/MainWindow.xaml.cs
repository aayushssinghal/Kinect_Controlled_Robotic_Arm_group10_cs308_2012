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

namespace KinectSensorElevationAngle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Kinect sensor
        KinectSensor sensor;

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
            this.StopKinect();
        }

        /// <summary>
        /// Stops the kinect.
        /// </summary>
        private void StopKinect()
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if device is connected.
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // Attach the status change event
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);

                //  Call the kinect start method
                this.StartKinect();

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
                this.StartKinect();
            }
            else
            {
                this.StopKinect();
            }

        }

        /// <summary>
        /// Initializes the kinect.
        /// </summary>
        private void StartKinect()
        {

            // Get the first Kinect Sensor
            sensor = KinectSensor.KinectSensors[0];

         
       

            // Use Parameterized value if any different format required.
            // sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30)
            
            sensor.ColorStream.Enable();
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            sensor.Start();

            // set the elevation Angle to 0 when start
            this.sensor.ElevationAngle = 0;
            labelAngle.Content = this.sensor.ElevationAngle;

        }

        /// <summary>
        /// Handles the ColorFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.ColorImageFrameReadyEventArgs"/> instance containing the event data.</param>
        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame == null)
                {
                    return;
                }

                byte[] pixelData = new byte[imageFrame.PixelDataLength];

                imageFrame.CopyPixelDataTo(pixelData);

                colorImageControl.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, imageFrame.Width * 4);

            }
        }

        /// <summary>
        /// Handles the Click event of the btnUP control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnUP_Click(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle + 5 <= sensor.MaxElevationAngle)
            {
                sensor.ElevationAngle += 5;
            }

            labelAngle.Content = this.sensor.ElevationAngle;
        }

        /// <summary>
        /// Handles the Click event of the btnDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (sensor.ElevationAngle - 5 >= sensor.MinElevationAngle)
            {
                sensor.ElevationAngle -= 5;
            }
            labelAngle.Content = this.sensor.ElevationAngle;
        }
    }
}
