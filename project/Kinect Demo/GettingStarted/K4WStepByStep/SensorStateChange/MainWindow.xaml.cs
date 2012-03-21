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
namespace SensorStateChange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);

        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if there is any kinect Connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // Call the method to show Kinect count and device id
                this.ShowKinectSensorInfo();
             
               // Attach the sensor status change event handler.
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);

            }
            else
            {
                // If there is no device connected
                labelKinectSensor.Content = "No Kinect Device Found";
            }
        }

        private void ShowKinectSensorInfo()
        {
            // If Kinect Connected Show the number of device.
            labelKinectSensor.Content = string.Format("Kinect Device Found : {0} ", KinectSensor.KinectSensors.Count.ToString());

            //Display the device ID of Kinect. 
            // KinectSensors[0] will pickup the first Kinect Device if there are multiple Kinect Device
            labelKinectDeviceId.Content = KinectSensor.KinectSensors[0].UniqueKinectId;
        }

        /// <summary>
        /// Handles the StatusChanged event of the KinectSensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.StatusChangedEventArgs"/> instance containing the event data.</param>
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            labelKinectStatus.Content = e.Status.ToString();

            if (e.Status == KinectStatus.Connected)
            {
                // Call the method to show Kinect count and device id
                this.ShowKinectSensorInfo();
            }
            else
            {
                labelKinectSensor.Content = "No Kinect Device Found";
                labelKinectDeviceId.Content = string.Empty;
            }
        }
    }
}
