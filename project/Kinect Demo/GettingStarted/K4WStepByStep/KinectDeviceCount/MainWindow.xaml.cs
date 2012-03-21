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

namespace KinectDeviceCount
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
                // If Kinect Connected Show the number of device.
                labelKinectSensor.Content = string.Format("Kinect Device Found : {0} ", KinectSensor.KinectSensors.Count.ToString());

                //Display the device ID of Kinect. 
                // KinectSensors[0] will pickup the first Kinect Device if there are multiple Kinect Device
                labelKinectDeviceId.Content = KinectSensor.KinectSensors[0].UniqueKinectId;

                //-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_
                // if there is multiple kinect and you want to display all of their ID

                        //KinectSensorCollection sensors = KinectSensor.KinectSensors;

                        //foreach (var sensor in sensors)
                        //{
                        //    // Display it over here  either in list or appending the string
                        //    // sensor.UniqueKinectId
                        //}
                //-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

            }
            else
            {
                // If there is no device connected
                labelKinectSensor.Content = "No Kinect Device Found";
            }
        }
    }
}
