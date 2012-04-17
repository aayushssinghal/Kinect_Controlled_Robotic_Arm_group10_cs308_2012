﻿using System;
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
using Coding4Fun.Kinect.Wpf;
using DexterER2;

namespace SkeletonTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        Window1 W;
        const int sampleSize = 10;
        int[][] lastFewValues = new int[7][];
        int currentValue = 0;
        double[] variance = new double[7];
        int[] AxisAngles = new int[7];

        public static int GetMedian(int[] sourceNumbers)
        {
            //Framework 2.0 version of this method. there is an easier way in F4        
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return 0;

            //make sure the list is sorted, but use a new array
            int[] sortedPNumbers = (int[])sourceNumbers.Clone();
            sourceNumbers.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            int median = (size % 2 != 0) ? (int)sortedPNumbers[mid] : ((int)sortedPNumbers[mid] + (int)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Closed += new EventHandler(MainWindow_Closed);
            W= new Window1();
            W.Show();
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
           
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 7; i++)
            {
                lastFewValues[i] = new int[sampleSize];
                for (int j = 0; j < sampleSize; j++) lastFewValues[i][j] = 90;
            }
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.ColorStream.Enable();
                sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
                sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
                sensor.SkeletonStream.Enable();
                
                sensor.Start();
            }
            else
            {
                MessageBox.Show("No Device Found !!!");
            }
            
        }
        //augmented code
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

                colorImageControl.Source = BitmapSource.Create(imageFrame.Width, imageFrame.Height, 60, 60, PixelFormats.Bgr32, null, pixelData, imageFrame.Width * 4);

            }
        }





        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame=e.OpenSkeletonFrame())
            {
               if (skeletonFrame == null)
                {
                    return; 
                }

                
                skeletonFrame.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                         where s.TrackingState == SkeletonTrackingState.Tracked
                                         select s).FirstOrDefault();

                if (first == null)
                {
                    return;
                }



                //set scaled position
                ScalePosition(head, first.Joints[JointType.Head]);
                ScalePosition(shoulder_center, first.Joints[JointType.ShoulderCenter]);
                ScalePosition(leftHand, first.Joints[JointType.HandLeft]);
                ScalePosition(rightHand, first.Joints[JointType.HandRight]);
                ScalePosition(leftElbow, first.Joints[JointType.ElbowLeft]);
                ScalePosition(rightElbow, first.Joints[JointType.ElbowRight]);
                ScalePosition(leftShoulder, first.Joints[JointType.ShoulderLeft]);
                ScalePosition(rightShoulder, first.Joints[JointType.ShoulderRight]);
                ScaleEdges(elbow_hand_left, first.Joints[JointType.ElbowLeft], first.Joints[JointType.HandLeft]);
                ScaleEdges(elbow_hand_right, first.Joints[JointType.ElbowRight], first.Joints[JointType.HandRight]);
                ScaleEdges(shoulder_elbow_left, first.Joints[JointType.ShoulderLeft], first.Joints[JointType.ElbowLeft]);
                ScaleEdges(shoulder_elbow_right, first.Joints[JointType.ShoulderRight], first.Joints[JointType.ElbowRight]);
                ScaleEdges(shoulder_center_left, first.Joints[JointType.ShoulderCenter], first.Joints[JointType.ShoulderLeft]);
                ScaleEdges(shoulder_center_right, first.Joints[JointType.ShoulderCenter], first.Joints[JointType.ShoulderRight]);
                ScaleEdges(head_shoulder, first.Joints[JointType.Head], first.Joints[JointType.ShoulderCenter]);


                
                HandPositionLower measures = new HandPositionLower(first);
                double baseA = measures.getBaseAngle();
                double horA = measures.getHorizonAngle();
                double hDist = measures.getHandDistance();
                PolygonAngles poly = new PolygonAngles(hDist);
                double theta, phi;
                theta = poly.getTheta();
                phi = poly.getPhi();


                lastFewValues[1][currentValue] = round(180 - (int)Mathematics.toDegrees(baseA));
                lastFewValues[2][currentValue] = round(180 - (int)Mathematics.toDegrees(horA + phi));
                lastFewValues[3][currentValue] = round((int)Mathematics.toDegrees(theta) + 90);
                lastFewValues[4][currentValue] = round((int)Mathematics.toDegrees(theta) + 90);
                //lastFewValues[5][currentValue] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s1l(first))));
                lastFewValues[5][currentValue] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s2l(first))));
                lastFewValues[5][currentValue] = Math.Min(180, Math.Max(90, 180 - lastFewValues[5][currentValue] + 90 - 20));
                lastFewValues[6][currentValue] = round(180 - (int)Mathematics.toDegrees(Math.Acos(Angles.el(first))) - 30);

                currentValue = (currentValue + 1) % sampleSize;

                for (int i = 1; i < 7; i++ ) AxisAngles[i] = GetMedian(lastFewValues[i]);
                if (AxisAngles[1] > 180) AxisAngles[1] = 180;

                /*
                AxisAngles[1] =  ( round(180 - (int)Mathematics.toDegrees(baseA)) + lastFewAverages[1])/2;
                if (AxisAngles[1] > 270) AxisAngles[1] = 0;
                AxisAngles[2] = ( round(180-(int)Mathematics.toDegrees(horA + phi)) + lastFewAverages[2])/2;

                AxisAngles[3] = ( round((int)Mathematics.toDegrees(theta)+90) + lastFewAverages[3])/2;
                AxisAngles[4] = ( round((int)Mathematics.toDegrees(theta)+90) + lastFewAverages[4])/2;
                AxisAngles[5] = ( round((int)Mathematics.toDegrees(Math.Acos(Angles.s1l(first)))) + lastFewAverages[5])/2;
                AxisAngles[6] = ( round(180 - (int)Mathematics.toDegrees(Math.Acos(Angles.el(first))) - 30) + lastFewAverages[6])/2;
                */

                // Real Part that sets the 6 angles for the robotic arm
                /*
                AxisAngles[1] = round(180-(int)Mathematics.toDegrees(Math.Acos(Angles.s1r(first)))+20);
                AxisAngles[2] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s2r(first))));
                AxisAngles[3] = round((int)Mathematics.toDegrees(Math.Acos(Angles.er(first))) + 90);
                
                AxisAngles[4] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s2l(first))));
                AxisAngles[5] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s1l(first))));
                AxisAngles[6] = round(180 - (int)Mathematics.toDegrees(Math.Acos(Angles.el(first)))-30) ;
                */


                // Displaying on GUI
                //Console.WriteLine("{0:F} {1:F} {2:F}", angleAtRightElbow, angleShoulderElbow, angleShoulderWithVertical);
               /* W.axis1.Content = AxisAngles[1].ToString();//.Substring(0,4);
                W.axis2.Content = AxisAngles[2].ToString();//.Substring(0, 4);
                W.axis3.Content = AxisAngles[3].ToString();
                W.axis4.Content = AxisAngles[4].ToString();
                W.axis5.Content = AxisAngles[5].ToString();
                W.axis6.Content = AxisAngles[6].ToString();
                */
                W.axis1.Content = AxisAngles[1].ToString();//.Substring(0,4);
                
                Direction d = new Direction(first.Joints[JointType.ShoulderRight], first.Joints[JointType.HandRight]);
                d.y += 0.3;
                W.axis2.Content = AxisAngles[2].ToString();//
                W.axis6.Content = AxisAngles[6].ToString();//d.x.ToString().Substring(0, 4) + " : " + d.y.ToString().Substring(0, 4) + " : " + d.z.ToString().Substring(0, 4);

                W.axis3.Content = AxisAngles[3].ToString();// Mathematics.toDegrees(horA).ToString() + " : " + Mathematics.toDegrees(phi).ToString() +" : " + hDist.ToString();//.Substring(0, 4);
                W.axis4.Content = AxisAngles[4].ToString();
                W.axis5.Content = AxisAngles[5].ToString();
                //W.axis6.Content = AxisAngles[6].ToString();


                if (W.RoboticArmMovement)
                    MoveRoboticArm(AxisAngles);
            }
        }

        private int round(int x)
        {
            if (Math.Abs(90 - x) < 5)
                return 90;
            return x;
        }

        public void MoveRoboticArm(int[] angles) 
        {
            for(int i=1;i<=6;i++){
                W.Arm.servo_move(i,angles[i],W.ArmSpeed);
            }
        }
                

        private void ScaleEdges(Line line, Joint one, Joint two)
        {
            Joint scaledJoint1 = one.ScaleTo(720, 720, .8f, .8f);
            Joint scaledJoint2 = two.ScaleTo(720, 720, .8f, .8f);
            line.X1 = scaledJoint1.Position.X;
            line.X2 = scaledJoint2.Position.X;
            line.Y1 = scaledJoint1.Position.Y;
            line.Y2 = scaledJoint2.Position.Y;
        }

        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            //convert the value to X/Y
            //Joint scaledJoint = joint.ScaleTo(1280, 720); 

            //convert & scale (.3 = means 1/3 of joint distance)
            Joint scaledJoint = joint.ScaleTo(720, 720, .8f, .8f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);
            SolidColorBrush mybrush = new SolidColorBrush();
            byte r, g, b;
            b = (byte)(100 * scaledJoint.Position.Z - 100);
            b = (byte)(b*1.3);
            g = r = 0;
            //Console.Write("Color b : ");
            //Console.WriteLine(b);
            mybrush.Color = Color.FromArgb(255, r, g, b);
            ((Ellipse)element).Fill = mybrush;
        }
    }
}
