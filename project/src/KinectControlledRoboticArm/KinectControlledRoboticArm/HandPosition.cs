using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;



namespace KinectControlledRoboticArm
{
    /**
     * Class: HandPosition
     * Class to Find out Hand angles and distances for slightly intuitive input 
     * interface. It assumes that, Right shoulder is equivalent to base of 
     * robotic arm. (Input Method - version 2)
     */
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
            //Console.Write("{0}, {1}, {2}\n", humanArmLength, distance, handDistance);
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

        public int[] getFinalAngles()
        {
            Skeleton first = skeleton;
            int[] lastFewValues = new int[7];
            HandPositionLower measures = new HandPositionLower(first);
            double baseA = measures.getBaseAngle();
            double horA = measures.getHorizonAngle();
            double hDist = measures.getHandDistance();
            PolygonAngles poly = new PolygonAngles(hDist);
            double theta, phi;
            theta = poly.getTheta();
            phi = poly.getPhi();

            Angles angles = new Angles(first);
            lastFewValues[1] = round(180 - (int)Mathematics.toDegrees(baseA));
            lastFewValues[2] = round(180 - (int)Mathematics.toDegrees(horA + phi));
            lastFewValues[3] = round((int)Mathematics.toDegrees(theta) + 90);
            lastFewValues[4] = round((int)Mathematics.toDegrees(theta) + 90);
            lastFewValues[5] = round((int)Mathematics.toDegrees(Math.Acos(angles.s2l())));
            lastFewValues[5] = Math.Min(180, Math.Max(90, 180 - lastFewValues[5] + 90 - 20));
            lastFewValues[6] = round(180 - (int)Mathematics.toDegrees(Math.Acos(angles.el())) - 30);

            if (lastFewValues[1] > 180) lastFewValues[1] = 180;
            return lastFewValues;
        }

        public static int round(int x)
        {
            if (Math.Abs(90 - x) < 5)
                return 90;
            return x;
        }
    }
}

