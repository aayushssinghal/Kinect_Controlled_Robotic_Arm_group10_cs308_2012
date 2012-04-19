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
using Coding4Fun.Kinect.Wpf;
using DexterER2;

namespace KinectControlledRoboticArm
{
    /// <summary>
    /// Interaction logic for SkeletonTracking.xaml
    /// </summary>
    /*
     * Class: SkeletonTracking
     * skeleton tracking window class
     */
    public partial class SkeletonTracking : Window
    {
        KinectSensor sensor;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        GUIcontrol W;
        const int sampleSize = 10;
        int[] lastFewValues = new int[7];
        double[] variance = new double[7];
        int[] AxisAngles = new int[7];
        int inputmethod = 1;
        Medians medians;
        
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

        public SkeletonTracking()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(SkeletonTracking_Loaded);
            Closed += new EventHandler(SkeletonTracking_Closed);
            W= new GUIcontrol();
            W.Show();
        }

        void SkeletonTracking_Closed(object sender, EventArgs e)
        {
           
        }

        void SkeletonTracking_Loaded(object sender, RoutedEventArgs e)
        {
            medians = new Medians();
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
                MessageBox.Show("Kinect Device not found !!!");
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

                inputmethod = W.inputmethod;
                skeletonFrame.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                         where s.TrackingState == SkeletonTrackingState.Tracked
                                         select s).FirstOrDefault();

                // skeleton data may not be available
                if (first == null)
                {
                    return;
                }

                // display skeleton on window
                displaySkeleton(first);
                
                // Selecting the input method
                switch (inputmethod)
                {
                    case 0: // input version 1
                        Angles angles1 = new Angles(first);
                        AxisAngles = angles1.getFinalAngles();
                        break;

                    case 2: // input version 2
                        HandPosition angles2 = new HandPosition(first);
                        AxisAngles = angles2.getFinalAngles();
                        break;

                    case 1: // input version 3
                        HandPositionLower angles3 = new HandPositionLower(first);
                        AxisAngles = angles3.getFinalAngles();
                        break;

                    default:
                        MessageBox.Show("Error !!!");
                        break;

                }

                AxisAngles = medians.getNextMedian(AxisAngles);


                // Displaying on GUI
                W.axis1.Content = AxisAngles[1].ToString();
                W.axis2.Content = AxisAngles[2].ToString();
                W.axis3.Content = AxisAngles[3].ToString();
                W.axis4.Content = AxisAngles[4].ToString();
                W.axis5.Content = AxisAngles[5].ToString();
                W.axis6.Content = AxisAngles[6].ToString();


                if (W.RoboticArmMovement)
                    MoveRoboticArm(AxisAngles);
            }
        }

        public static int round(int x)
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

        /**
         * Displays the skeleton of body on the frame using lines and circles
         */
        private void displaySkeleton(Skeleton first) {
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
