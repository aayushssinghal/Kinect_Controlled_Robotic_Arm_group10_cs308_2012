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
        int[] lastFewAverages = new int[7];
        double[] variance = new double[7];
        int[] AxisAngles = new int[7];

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
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
                sensor.SkeletonStream.Enable();
                sensor.Start();
            }
            else
            {
                MessageBox.Show("No Device Found !!!");
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

                
                AxisAngles[1] = round(180-(int)Mathematics.toDegrees(Math.Acos(Angles.s1r(first))));
                AxisAngles[2] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s2r(first))));
                AxisAngles[3] = round((int)Mathematics.toDegrees(Math.Acos(Angles.er(first))) + 90);
                
                AxisAngles[4] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s2l(first))));
                AxisAngles[5] = round((int)Mathematics.toDegrees(Math.Acos(Angles.s1l(first))));
                AxisAngles[6] = round(180 - (int)Mathematics.toDegrees(Math.Acos(Angles.el(first)))) ;
                
                //Console.WriteLine("{0:F} {1:F} {2:F}", angleAtRightElbow, angleShoulderElbow, angleShoulderWithVertical);
                W.axis1.Content = AxisAngles[1].ToString();//.Substring(0,4);
                W.axis2.Content = AxisAngles[2].ToString();//.Substring(0, 4);
                W.axis3.Content = AxisAngles[3].ToString();
                W.axis4.Content = AxisAngles[4].ToString();
                W.axis5.Content = AxisAngles[5].ToString();
                W.axis6.Content = AxisAngles[6].ToString();


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
                W.Arm.servo_move(i,angles[i],20);
            }
        }
                

        private void ScaleEdges(Line line, Joint one, Joint two)
        {
            Joint scaledJoint1 = one.ScaleTo(320, 320, .5f, .5f);
            Joint scaledJoint2 = two.ScaleTo(320, 320, .5f, .5f);
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
            Joint scaledJoint = joint.ScaleTo(320, 320, .5f, .5f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);
            SolidColorBrush mybrush = new SolidColorBrush();
            byte r, g, b;
            b = (byte)(100 * scaledJoint.Position.Z - 100);
            b = (byte)(b*1.3);
            g = r = 0;
            Console.Write("Color b : ");
            Console.WriteLine(b);
            mybrush.Color = Color.FromArgb(255, r, g, b);
            ((Ellipse)element).Fill = mybrush;
        }
    }
}
