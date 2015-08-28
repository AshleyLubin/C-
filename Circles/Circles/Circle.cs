using System.Collections.Generic;
using System.Drawing;
using System;

namespace Circles
{
	public class Circle
	{     

        //2015_03_24 ajout information du cercle englobant, dans lequel le "petit" cercle ne doit depasser
        public double BoundingCircle_x;
        public double BoundingCircle_y;
        public double BoundingCircle_r;

		public const double minVelocity = 0.005;
		public const double maxVelocity = 0.015;     


		public double x;
		public double y;
		public double r;     
		public double intialR;
		public int num;       

		public int amplifyVelocityCount = 0;

		public double angleWithPrevious;
		public double angleWithPreviousVelocity;

		// 0 : not colliding
		// 0 < value < 1 : about to collide
		// 1 : colliding
		public Dictionary<Circle, double> collisions;        

		public Circle previous;
		public List<Circle> next;

		public Circle()
		{
			x = 0;
			y = 0;
			r = 1;
			angleWithPrevious = RandomManager.getDouble(-1.2, 1.2);
			angleWithPreviousVelocity = RandomManager.getDouble(0.009, 0.015);
			if (RandomManager.getDouble(-1, 1) < 0) angleWithPreviousVelocity = -angleWithPreviousVelocity;
			collisions = new Dictionary<Circle, double>();
			next = new List<Circle>();

           
		}     

		public void Draw(Graphics g)
		{
			Font f = new Font("Arial", 14);
			Pen pen = Pens.Green;
			double maxCollision = getMaxCollision();
			if (maxCollision > 0.0) pen = Pens.Orange;
			if (maxCollision == 1.0) pen = Pens.Red;
			g.DrawEllipse(pen, (int)(x - r), (int)(y - r), (int)(2 * r), (int)(2 * r));
			g.DrawString(num.ToString(), f, Brushes.Black, Convert.ToSingle(x), Convert.ToSingle(y));
            
            //2015_03_24
            g.DrawEllipse(Pens.Red, (int)(BoundingCircle_x - BoundingCircle_r), (int)(BoundingCircle_y - BoundingCircle_r), (int)BoundingCircle_r * 2, (int)BoundingCircle_r * 2);
		}

        public void MovePrevious()
        {
            angleWithPrevious += angleWithPreviousVelocity;
            // Permet de redynamiser le cercle après une détection de collision
            if (amplifyVelocityCount > 0)
            {
                Console.Write("Amplification de vélocité sur cercle " + num + " : " + angleWithPreviousVelocity + " -> ");
                angleWithPreviousVelocity = angleWithPreviousVelocity * 1.15;
                Console.WriteLine(angleWithPreviousVelocity);
                amplifyVelocityCount--;
            }
        }      

		public double getMaxCollision()
		{
			double result = 0.0;

			foreach (double value in collisions.Values)
				if (value > result) result = value;
			return result;
		}
   }
}