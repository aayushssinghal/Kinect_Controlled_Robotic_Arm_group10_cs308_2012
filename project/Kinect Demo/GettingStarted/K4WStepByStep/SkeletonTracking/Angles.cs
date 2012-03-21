using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace SkeletonTracking
{
    public class Angles
    {
        /**
         * Angle between Shoulder and body plane
         */
        public static double s1r(Skeleton first)
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
        public static double er(Skeleton first)
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
        public static double s2r(Skeleton first)
        {
            Joint shr = first.Joints[JointType.ShoulderRight];
            Joint elr = first.Joints[JointType.ElbowRight];

            Direction D1 = new Direction(shr, elr);
            Direction D2 = new Direction(0, 1, 0);
            return Mathematics.angleBetweenLines(D1, D2);
        }


        public static double s1l(Skeleton first)
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
        public static double el(Skeleton first)
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
        public static double s2l(Skeleton first)
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
        public static double wristAngle(Skeleton first)
        {
            Joint hand = first.Joints[JointType.HandRight];
            Joint wrist = first.Joints[JointType.WristRight];
            Joint elbow = first.Joints[JointType.ElbowRight];
            Direction D1 = new Direction(wrist, hand);
            Direction D2 = new Direction(elbow, wrist);
            return Mathematics.angleBetweenLines(D1, D2);
            }
    }
}
