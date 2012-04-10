using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System;

public class Plane
{
    public double
        x,
        y,
        z;
    public Plane()
    {
        x = y = z = 1;
    }

    public Plane(double xx, double yy, double zz)
    {
        x = xx;
        y = yy;
        z = zz;
    }

    public Direction planePerpendicular()
    {
        Direction d = new Direction(x, y, z);
        return d;
    }
}

public class angles
{
    public double s1r(Skeleton first)
    {

        Joint shl = first.Joints[JointType.ShoulderLeft];
        Joint sp = first.Joints[JointType.Spine];
        Joint shr = first.Joints[JointType.ShoulderRight];
        Joint elr = first.Joints[JointType.ElbowRight];
        
        Plane p = new Plane(shl,sp,shr);
        Direction D = new Direction(shr, elr);
        return Mathematics.angleBetweenLinesAndPlanes(D, p);
    }

    public double er(Skeleton first)
    {
        Joint hr = first.Joints[JointType.HandRight];
        Joint shr = first.Joints[JointType.ShoulderRight];
        Joint elr = first.Joints[JointType.ElbowRight];
        Direction D1 = new Direction(elr,shr);
        Direction D2 = new Direction(hr, elr);
        return Mathematics.angleBetweenLines(D1, D2);
    }
    
    public double s2r(Skeleton first)
    {
        
        Joint shr = first.Joints[JointType.ShoulderRight];
        Joint elr = first.Joints[JointType.ElbowRight];

        Direction D1 = new Direction(shr, elr);
        Direction D2 = new Direction(0,1,0);
        return Mathematics.angleBetweenLines(D1,D2);
    }
}

public class Mathematics
{
    public Plane crossProduct(Direction a, Direction b)
    {
        Plane p = new Plane();
        p.x = a.y * b.z - b.y * a.z;
        p.y = - a.x * b.z + a.z*b.x;
        p.z = a.x * b.y - a.y*b.x;
        return p;
    }

    public double dotProduct(Direction a, Direction b)
    {
        double val = a.x * b.x + a.y * b.y + a.z * b.z;
        return val;
    }
    
    public double norm(Direction x)
    {
        return Math.Sqrt(x.x*x.x + x.y*x.y+x.z*x.z);
    }

    public double angleBetweenPlanes(Plane a, Plane b)
    {
        return angleBetweenLines(a.planePerpendicular(), b.planePerpendicular());
    }

    public double angleBetweenLines(Direction a, Direction b)
    {
        return dotProduct(a, b) / (norm(a) * norm(b));
    }
}

public class Direction
{
    public double x, y, z;
    public double cosx, cosy, cosz;

    public bool isCosineEqual(double a, double b)
    {
        return (Math.Abs(a - b) < 0.1);
    }

	public Direction(double x, double y, double z)
	{
        this.x = x;
        this.y = y;
        this.z = z;
        
        double s = Math.Sqrt(x*x + y*y + z*z);
        if (s == 0) {
            cosx = cosy = cosz = 0;
            return;
        }
        cosx = x / s;
        cosy = y / s;
        cosz = z / s;
	}

    // Returns angle between two lines
    public Direction lineAngle(Direction line)
    {
        
        double norm = Math.Sqrt(x * line.x + y * line.y + z * line.z);
        double xx, yy, zz;
        xx = x * line.x / norm;
        yy = y * line.y / norm;
        zz = z * line.z / norm;
        return new Direction(xx, yy, zz);
    }

    public bool isHorizontal()
    {
        return (isCosineEqual(cosy, 0) && isCosineEqual(cosz, 0) && isCosineEqual(cosx, 1));
    }

    public bool isVertical()
    {
        return (isCosineEqual(cosy, 1) && isCosineEqual(cosz, 0) && isCosineEqual(cosx, 0));
    }

    public bool isPerpendicular()
    {
        return (isCosineEqual(cosy, 0) && isCosineEqual(cosz, 1) && isCosineEqual(cosx, 0)) ;
    }

    public bool isStraightLine()
    {
        return (isCosineEqual(cosx, 0) && isCosineEqual(cosy, 0) && isCosineEqual(cosz, 0));
    }
}

enum GestureType {NONE, ZERO, HOLD};

public class gestures
{
    public GestureType ges;
    public Direction
        hand_elbow,
        elbow_shoulder,
        shoulders,
        head_shoulders;

    public gestures()
    {
        ges = GestureType.NONE;
        
    }

    public GestureType getGestureType()
    {
        Direction angle_shoulders_elbow = shoulders.lineAngle(elbow_shoulder);
        Direction angle_elbow = elbow_shoulder.lineAngle(hand_elbow);
        if (shoulders.isHorizontal())
        {
        }

    }
}