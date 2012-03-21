// Author :  Abhijit Jana
// Blog : http://abhijitjana.net
// Twitter : http://twitter.com/abhijitjana
//-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectDeviceCountConsole
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
                Console.WriteLine("Device ID : {0} ",KinectSensor.KinectSensors[0].UniqueKinectId);

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
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("No Kinect Device Found");
            }
        }
    }
}
