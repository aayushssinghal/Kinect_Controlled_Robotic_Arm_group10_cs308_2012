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
namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count >0)
            {
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
              //  MessageBox.Show(KinectSensor.KinectSensors.Count.ToString());
                labelKinectDeviceID.Content = KinectSensor.KinectSensors[0].UniqueKinectId;

                sensor = KinectSensor.KinectSensors[0];
                sensor.ColorStream.Enable();
                sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
             //   sensor.ElevationAngle = 0;

                sensor.Start();
                     
            }
            else
            {
                MessageBox.Show("No Device Found !!!");
            }
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

                videoImageControl.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, imageFrame.Width * 4);
                
            }
        }
        

        /// <summary>
        /// Handles the StatusChanged event of the KinectSensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.StatusChangedEventArgs"/> instance containing the event data.</param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            labelDeviceStatus.Content = e.Status.ToString();
        }

        /// <summary>
        /// Handles the Click event of the btnUP control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnUP_Click(object sender, RoutedEventArgs e)
        {
            sensor.ElevationAngle += 5;
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            sensor.ElevationAngle -= 5;
        }
    }
}
