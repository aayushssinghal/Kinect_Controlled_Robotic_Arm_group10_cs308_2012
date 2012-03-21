// Author :  Abhijit Jana
// Blog : http://abhijitjana.net
// Twitter : http://twitter.com/abhijitjana
//-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
namespace SensorStateChangeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CountKinect();
        }

        /// <summary>
        /// Counts the kinect.
        /// </summary>
        private static void CountKinect()
        {
            // Check if there is any kinect Connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                // If Kinect Connected Show the number of device.
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Format("Kinect Device Found : {0} ", KinectSensor.KinectSensors.Count.ToString()));

                //Display the device ID of Kinect. 
                // KinectSensors[0] will pickup the first Kinect Device if there are multiple Kinect Device
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Device ID : {0} ", KinectSensor.KinectSensors[0].UniqueKinectId);

                // Attach the Kinect State Change  Event Handler...
                KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);

                Console.ReadLine();
            }
            else
            {
                // If there is no device connected
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("No Kinect Device Found");
            }
        }

        /// <summary>
        /// Handles the StatusChanged event of the KinectSensors control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.StatusChangedEventArgs"/> instance containing the event data.</param>
        static void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(e.Status.ToString());
        }
    }
}
