using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace KinectControlledRoboticArm
{
    /* Class: Angles
      This class is used for angles in direct mapping.Class finds angles in kinect captured human body. It was used with
      less intuitive input interface, as it directly give all angles for
      robotic arm, and it's hard to learn for a human.
      (Input Method - version 1)
     */
    public class Angles
    {
        Skeleton first;

        /*
          Constructor: Angles
         */
        public Angles(Skeleton sk)
        {
            first = sk;
        }

        /* 
        * Function: s1r
       
            Angle between Shoulder and body plane
         
    	    

   			Returns:

      	    Angle between plane formed by shoulder and right elbow
        */
        public double s1r()
        {

            Joint shl = first.Joints[JointType.ShoulderLeft];
            Joint sp = first.Joints[JointType.Spine];
            Joint shr = first.Joints[JointType.ShoulderRight];
            Joint elr = first.Joints[JointType.ElbowRight];

            Plane p = new Plane(shl, sp, shr);
            Direction per = p.planePerpendicular();
            Direction D = new Direction(shr, elr);
            return Mathematics.angleBetweenLines(D, per);
        }

        /**
         * Angle at right elbow
         */
        public double er()
        {
            Joint hr = first.Joints[JointType.HandRight];
            Joint shr = first.Joints[JointType.ShoulderRight];
            Joint elr = first.Joints[JointType.ElbowRight];
            Direction D1 = new Direction(elr, shr);
            Direction D2 = new Direction(hr, elr);
            return Mathematics.angleBetweenLines(D1, D2);
        }

        /**
         * Angle between shoulder and y-axis or vertical
         */
        public double s2r()
        {
            Joint shr = first.Joints[JointType.ShoulderRight];
            Joint elr = first.Joints[JointType.ElbowRight];

            Direction D1 = new Direction(shr, elr);
            Direction D2 = new Direction(0, 1, 0);
            return Mathematics.angleBetweenLines(D1, D2);
        }


        public double s1l()
        {

            Joint shl = first.Joints[JointType.ShoulderLeft];
            Joint sp = first.Joints[JointType.Spine];
            Joint shr = first.Joints[JointType.ShoulderRight];
            Joint ell = first.Joints[JointType.ElbowLeft];

            Plane p = new Plane(shl, sp, shr);
            Direction per = p.planePerpendicular();
            Direction D = new Direction(shl, ell);
            return Mathematics.angleBetweenLines(D, per);
        }

        /**
         * Angle at right elbow
         */
        public double el()
        {
            Joint hr = first.Joints[JointType.HandLeft];
            Joint shr = first.Joints[JointType.ShoulderLeft];
            Joint elr = first.Joints[JointType.ElbowLeft];
            Direction D1 = new Direction(elr, shr);
            Direction D2 = new Direction(hr, elr);
            return Mathematics.angleBetweenLines(D1, D2);
        }

        /**
         * Angle between shoulder and y-axis or vertical
         */
        public double s2l()
        {

            Joint shl = first.Joints[JointType.ShoulderLeft];
            Joint ell = first.Joints[JointType.ElbowLeft];

            Direction D1 = new Direction(shl, ell);
            Direction D2 = new Direction(0, 1, 0);
            return Mathematics.angleBetweenLines(D1, D2);
        }

        /**
         * Wrist angle
         */
        public double wristAngle()
        {
            Joint hand = first.Joints[JointType.HandRight];
            Joint wrist = first.Joints[JointType.WristRight];
            Joint elbow = first.Joints[JointType.ElbowRight];
            Direction D1 = new Direction(wrist, hand);
            Direction D2 = new Direction(elbow, wrist);
            return Mathematics.angleBetweenLines(D1, D2);
        }

        public int[] getFinalAngles()
        {
            int[] AxisAngles = new int[7];
            AxisAngles[1] = round(180 - (int)Mathematics.toDegrees(Math.Acos(s1r())) + 20);
            AxisAngles[2] = round((int)Mathematics.toDegrees(Math.Acos(s2r())));
            AxisAngles[3] = round((int)Mathematics.toDegrees(Math.Acos(er())) + 90);

            AxisAngles[4] = round((int)Mathematics.toDegrees(Math.Acos(s2l())));
            AxisAngles[5] = round((int)Mathematics.toDegrees(Math.Acos(s1l())));
            AxisAngles[6] = round(180 - (int)Mathematics.toDegrees(Math.Acos(el())) - 30);
            return AxisAngles;
        }

        public static int round(int x)
        {
            if (Math.Abs(90 - x) < 5)
                return 90;
            return x;
        }
    }
}
