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


namespace SkeletonTracking
{
    public class HandPosition
    {
        public static double humanArmLength = 0.1;
        public Skeleton skeleton;
        public double baseAngle;
        public double handDistance;
        public double horizontalAngle;
        public bool initialized = false;

        public HandPosition(Skeleton sk)
        {
            skeleton = sk;
            Measure();
        }

        public void Measure()
        {
            Joint shl = skeleton.Joints[JointType.ShoulderLeft];
            Joint sp = skeleton.Joints[JointType.Spine];
            Joint shr = skeleton.Joints[JointType.ShoulderRight];
            Joint handR = skeleton.Joints[JointType.HandRight];

            Plane p = new Plane(shl, sp, shr);
            Direction hand = new Direction(shr, handR);
            Direction projectedhand = new Direction(hand.x, 0, hand.z);
            double distance = Math.Sqrt(Mathematics.norm(hand));
            humanArmLength = 0.75;//Math.Max(humanArmLength, distance);
            Direction vertical = new Direction(0, 1, 0);
            double baseangle = Mathematics.angleBetweenLines2pi(p.planePerpendicular(), projectedhand, vertical) - Math.PI / 2.0; // in 2nd quardant 
            double angle2 = Mathematics.angleBetweenLines2pi(hand, vertical);
            if (angle2 > Math.PI)
            {
                angle2 = 2 * Math.PI - angle2;
            }
            if (angle2 > Math.PI / 2)
            {
                angle2 = Math.PI / 2;
            }
            angle2 = Math.Abs(angle2);
            angle2 = Math.PI / 2 - angle2;

            baseAngle = baseangle;
            handDistance = (distance / humanArmLength) * RoboticArm.MaxLength;
            Console.Write("{0}, {1}, {2}\n", humanArmLength, distance, handDistance);
            horizontalAngle = angle2;
            initialized = true;
        }

        public double getBaseAngle()
        {
            return baseAngle;
        }

        public double getHorizonAngle()
        {
            return horizontalAngle;
        }

        public double getHandDistance()
        {
            return handDistance;
        }
    }


    public class HandPositionLower
    {
        public static double humanArmLength = 0.1;
        public Skeleton skeleton;
        public double baseAngle;
        public double handDistance;
        public double horizontalAngle;
        public bool initialized = false;

        public HandPositionLower(Skeleton sk)
        {
            skeleton = sk;
            Measure();
        }

        public void Measure()
        {
            Joint shl = skeleton.Joints[JointType.ShoulderLeft];
            Joint sp = skeleton.Joints[JointType.Spine];
            Joint shr = skeleton.Joints[JointType.ShoulderRight];
            Joint handR = skeleton.Joints[JointType.HandRight];

            Plane p = new Plane(shl, sp, shr);
            Direction hand = new Direction(shr, handR);
            hand.y += 0.3;
            Direction projectedhand = new Direction(hand.x, 0, hand.z);
            double distance = Mathematics.norm(hand);
            humanArmLength = 0.45;//Math.Max(humanArmLength, distance);
            Direction vertical = new Direction(0, 1, 0);
            double baseangle = Mathematics.angleBetweenLines2pi(p.planePerpendicular(), projectedhand, vertical) - Math.PI / 2.0; // in 2nd quardant 
            double angle2 = Mathematics.angleBetweenLines2pi(hand, vertical);
            if (angle2 > Math.PI)
            {
                angle2 = 2 * Math.PI - angle2;
            }
            if (angle2 > Math.PI / 2)
            {
                angle2 = Math.PI / 2;
            }
            angle2 = Math.Abs(angle2);
            angle2 = Math.PI / 2 - angle2;

            baseAngle = baseangle;
            handDistance = (distance / humanArmLength) * RoboticArm.MaxLength;
            Console.Write("{0}, {1}, {2}\n", humanArmLength, distance, handDistance);
            horizontalAngle = angle2;
            initialized = true;
        }

        public double getBaseAngle()
        {
            return baseAngle;
        }

        public double getHorizonAngle()
        {
            return horizontalAngle;
        }

        public double getHandDistance()
        {
            return handDistance;
        }
    }

}

namespace SkeletonTracking
{
    public class RoboticArm
    {
        public static double Arm1 = 9;
        public static double Arm2 = 8;
        public static double Arm3 = 10;
        public static double MaxLength = Arm1+Arm2+Arm3;
        
    }
}