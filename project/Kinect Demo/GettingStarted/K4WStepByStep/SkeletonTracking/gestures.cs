using System;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

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

    public Plane(Joint a, Joint b, Joint c)
    {
        Direction d1 = new Direction(a, b);
        Direction d2 = new Direction(a, c);
        Direction per = Mathematics.crossProduct(d1, d2);
        x = per.x;
        y = per.y;
        z = per.z;
    }

    public Direction planePerpendicular()
    {
        Direction d = new Direction(x, y, z);
        return d;
    }

    bool isValid()
    {
        return (Mathematics.norm(planePerpendicular()) != 0);
    }
}

public class Mathematics
{
    public static Direction upDirection = new Direction(0, 0, 1);
    public static Direction crossProduct(Direction a, Direction b)
    {
        Direction p = new Direction();
        p.x = a.y * b.z - b.y * a.z;
        p.y = - a.x * b.z + a.z * b.x;
        p.z = a.x * b.y - a.y * b.x;
        return p;
    }

    public static double square(double x)
    {
        return x * x;
    }

    public static double dotProduct(Direction a, Direction b)
    {
        double val = a.x * b.x + a.y * b.y + a.z * b.z;
        return val;
    }
    
    public static double norm(Direction x)
    {
        return Math.Sqrt(x.x*x.x + x.y*x.y+x.z*x.z);
    }

    public static double angleBetweenPlanes2pi(Plane a, Plane b)
    {
        return angleBetweenLines2pi(a.planePerpendicular(), b.planePerpendicular());
    }

    public static double angleBetweenPlanes(Plane a, Plane b)
    {
        return angleBetweenLines(a.planePerpendicular(), b.planePerpendicular());
    }

    /**
     * Angle rotated to go from direction a to direction b, while thumb pointing
     * towards or closer to up direction
     */
    public static double angleBetweenLines2pi(Direction a, Direction b)
    {
        double n = norm(a) * norm(b);
        if (n == 0)
        {
            Console.WriteLine("norm is 0, angle not defined");
            return 0;
        }
        double x = dotProduct(a, b) / n;
        if (double.IsNaN(x))
        {
            Console.WriteLine("getting a NAN, angle not defined");
            return 0;
        }
        Direction cross = crossProduct(a, b);
        double up = dotProduct(upDirection, cross);
        if (up >= 0) return Math.Acos(x);
        else return 2*Math.PI - Math.Acos(x);
    }

    public static double angleBetweenLines(Direction a, Direction b)
    {
        return Math.Cos(angleBetweenLines2pi(a, b));
    }

    public static double angleBetweenLinesAndPlanes2pi(Plane p, Direction d)
    {
        Direction per = p.planePerpendicular();
        double sangle = angleBetweenLines2pi(per, d);
        return (sangle + 3 * Math.PI / 2) % (2 * Math.PI);
    }

    public static double angleBetweenLinesAndPlanes(Direction d, Plane p)
    {
        return Math.Cos(angleBetweenLinesAndPlanes2pi(p, d));
    }

    public static bool isInPlane(Direction d, Plane p)
    {
        double angle = angleBetweenLinesAndPlanes(d, p);
        return isCosineApproxEqual(angle, 1.0);
    }

    public static bool isCosineApproxEqual(double a, double b)
    {
        return (Math.Abs(a - b) < 0.1); // TODO
    }

    public static double toDegrees(double radians)
    {
        return 180.0 * radians / Math.PI;
    }

}

public class Direction
{
    public double x, y, z;
    public double cosx, cosy, cosz;

    public bool isCosineEqual(double a, double b)
    {
        return (Math.Abs(a - b) < 0.1); // TODO
    }

    public Direction(Joint i, Joint j)
    {
        createDirection(
            j.Position.X - i.Position.X, 
            j.Position.Y - i.Position.Y,
            j.Position.Z - i.Position.Z);
    }

    public Direction(double x = 0, double y = 0, double z = 0) {
        createDirection(x, y, z);
    }

	public void createDirection(double x, double y, double z)
	{
        this.x = x;
        this.y = y;
        this.z = z;
        
        double s = Math.Sqrt(x*x + y*y + z*z);
        if (s == 0) {
            cosx = cosy = cosz = 1;
            return;
        }
        cosx = x / s;
        cosy = y / s;
        cosz = z / s;
	}

    // Returns angle between two lines
    public Double lineAngle(Direction line)
    {
        return Mathematics.angleBetweenLines(this, line);
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

public enum GestureType {NONE, ZERO, HOLD};

// Unused
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
        double angle_shoulder_elbow = shoulders.lineAngle(elbow_shoulder);
        double angle_elbow = elbow_shoulder.lineAngle(hand_elbow);
        if (shoulders.isHorizontal())
        {
        }
        return new GestureType();
    }
}