using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* Class: Mathematics
  Class to Calculate distances and line angles and other mathematical
  functions.
 */
public class Mathematics
{
    public static Direction upDirection = new Direction(0, 0, 1);
    
    /*
    	Function: crossProduct
    	Gives the 3-D vector cross product of two given vectors
    	
    	Parameters:
    		a - Direction
    		b - Direction
    	
    	Return:
    		Direction
    */
    public static Direction crossProduct(Direction a, Direction b)
    {
        Direction p = new Direction();
        p.x = a.y * b.z - b.y * a.z;
        p.y = -a.x * b.z + a.z * b.x;
        p.z = a.x * b.y - a.y * b.x;
        return p;
    }

	/*
		Function: square
		Gives the numerical square of a real number
		
		Parameters:
			x - double
		
		Returns:
			double
	*/
    public static double square(double x)
    {
        return x * x;
    }

	/*
		Function: dotProduct
		Gives the dot product of two vector directions
		
		Parameters:
			a - Direction
			b - Direction
		
		Returns:
			double
	*/
    public static double dotProduct(Direction a, Direction b)
    {
        double val = a.x * b.x + a.y * b.y + a.z * b.z;
        return val;
    }
	
	/*
		Function: norm
		Gives the norm of a vector. It is equal to the length
		of vector
		
		Parameters:
			x - Direction
		
		Returns:
			double
	*/
    public static double norm(Direction x)
    {
        return Math.Sqrt(x.x * x.x + x.y * x.y + x.z * x.z);
    }

	/*
		Function: angleBetweenPlanes2pi
		Gives the angle between two planes. It gives the angle between
		two plane perpendiculars. Angle is determined by rotating from
		perpendicular of first plane, to perpendicular of second plane
		in anti-clockwise direction.
		
		Parameters:
			a - Plane
			b - Plane
			
		Returns:
			angle from a range of [0, 2PI)
	*/
    public static double angleBetweenPlanes2pi(Plane a, Plane b)
    {
        return angleBetweenLines2pi(a.planePerpendicular(), b.planePerpendicular());
    }

	/*
		Function: angleBetweenPlanes
		
		Gives the cosine of angle between perpendiculars of two
		planes.
		
		Parameters:
			a - Plane
			b - Plane
			
		Returns:
			angle cosine from range of [0, 1]
	*/
    public static double angleBetweenPlanes(Plane a, Plane b)
    {
        return angleBetweenLines(a.planePerpendicular(), b.planePerpendicular());
    }

    /*
    	Function: angleBetweenLines2pi
      Angle rotated to go from direction a to direction b, while thumb pointing
      towards or closer to up direction
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
        else return 2 * Math.PI - Math.Acos(x);
    }

	/*
		Function: angleBetweenLines2pi
		Gives the angle between two lines. This angle is determined 
		by rotation made, to go from first direction to second 
		direction, such that, thumb points towards vertical direction.
		
		Parameters:
			a - Direction
			b - Direction
			vertical - Direction
		
		Returns:
			angle from a range of [0, 2PI)
	*/
    public static double angleBetweenLines2pi(Direction a, Direction b, Direction vertical)
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
        double up = dotProduct(vertical, cross);
        if (up >= 0) return Math.Acos(x);
        else return 2 * Math.PI - Math.Acos(x);
    }

    public static double angleBetweenLines(Direction a, Direction b)
    {
        return Math.Cos(angleBetweenLines2pi(a, b));
    }

    public static double angleBetweenLinesAndPlanes2pi(Plane p, Direction d, Direction vertical)
    {
        Direction per = p.planePerpendicular();
        double sangle = angleBetweenLines2pi(per, d, vertical);
        return (-sangle + 5 * Math.PI / 2) % (2 * Math.PI); // 90 - sangle
    }

    public static double angleBetweenLinesAndPlanes2pi(Plane p, Direction d)
    {
        Direction per = p.planePerpendicular();
        double sangle = angleBetweenLines2pi(per, d);
        return (-sangle + 5 * Math.PI / 2) % (2 * Math.PI); // 90 - sangle
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

	/*
		Function: toDegrees
		Converts a radian angle into degrees
		
		Parameters:
			radians - double
		Returns:
			double angle in degrees
	*/
		
    public static double toDegrees(double radians)
    {
        return 180.0 * radians / Math.PI;
    }

}
