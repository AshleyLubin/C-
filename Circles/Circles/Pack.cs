using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Circles
{
	public class Pack
	{       

		private static double R = 305;
		private static double X = 400;
		private static double Y = 300;

		public Circle bigOne;
		private static List<Circle> circles;
		private static List<Circle> circlesInitial;

		private static List<PointF> bigOnePoints = new List<PointF>();
		private static List<double> bigOneRadius = new List<double>();      


		private int DiametreMax;

		public static Pack BuildPack(int circleNb, int numBig, int diametreMax, int yCircle)
		{
			Pack pack = new Pack() { DiametreMax = diametreMax };
			R = diametreMax;

			// Create circles
			circles = new List<Circle>() { new Circle() { num = 1 }, new Circle() { num = 2 }, new Circle() { num = 3 } };
			if (circleNb > 3) circles.Add(new Circle() { num = 4 });
			if (circleNb > 4) circles.Add(new Circle() { num = 5 });
			if (circleNb > 5) circles.Add(new Circle() { num = 6 });          

			// Big One position
			int bigOnePosition = numBig;

			// Radius
			List<double> rayon = new List<double>();

            //if (circleNb == 6)
            //{                
            //    rayon.Add(1.0);
            //    rayon.Add(rayon[0] * 0.8);
            //    rayon.Add(rayon[1] * 0.9);
            //    rayon.Add(rayon[2] * 0.95);
            //    rayon.Add(rayon[3] * 0.975);
            //    rayon.Add(rayon[4] * 1);                      
            //}


            //2015_03_24 définition position en dur (dans un 1er temps) des cercles englobants
            if (circleNb == 6)
            {
                circles[0].BoundingCircle_x = 400;
                circles[0].BoundingCircle_y = 400;
                circles[0].BoundingCircle_r = 100; // Millieu                  
                
                circles[1].BoundingCircle_x = 200;
                circles[1].BoundingCircle_y = 450;
                circles[1].BoundingCircle_r = 100; // Gauche

                circles[2].BoundingCircle_x = 400;
                circles[2].BoundingCircle_y = 600;
                circles[2].BoundingCircle_r = 100; // Gauche

                circles[3].BoundingCircle_x = 280;
                circles[3].BoundingCircle_y = 230;
                circles[3].BoundingCircle_r = 100; // Gauche

                circles[4].BoundingCircle_x = 600;
                circles[4].BoundingCircle_y = 450;
                circles[4].BoundingCircle_r = 100; // Gauche

                circles[5].BoundingCircle_x = 510;
                circles[5].BoundingCircle_y = 230;
                circles[5].BoundingCircle_r = 100; // Gauche

                foreach (Circle c in circles)
                {
                    c.x = c.BoundingCircle_x;
                    c.y = c.BoundingCircle_y;
                    c.r = c.BoundingCircle_r /3;                   
                }
            }

            return pack;

            if (circleNb == 6)
            {

                int parent = 0;
                int fils = 1;

                // Un Y n'est possible que sur les noeuds 2 à 4
                // puisque le 1er noeud n'a pas de parent, on ne peut pas faire de Y
                if (yCircle == 1) yCircle = 2;
                // On ne peut pas dépasser 4 car il n'y a plus assez de fils pour faire un Y 
                if (yCircle > 3) yCircle = 4;
                for (int i = 0; fils < circleNb; i++)
                {
                    circles[parent].next.Add(circles[fils]); circles[fils].previous = circles[parent];
                    if (yCircle == parent + 1)
                    {
                        // On est dans le cas où on a 2 enfants à ce noeud
                        // On reclacul l'angleWithPrevious du fils courant avant de passer au suivant.
                        double min = 0;
                        min = circles[fils].angleWithPrevious = RandomManager.getDouble2(min, Math.PI / 2);

                        fils++;
                        circles[parent].next.Add(circles[fils]); circles[fils].previous = circles[parent];
                        circles[fils].angleWithPrevious = RandomManager.getDouble2(-Math.PI / 2, min / 6);
                    }
                    if (yCircle < parent + 1)
                        circles[fils].angleWithPrevious = circles[parent].angleWithPrevious + RandomManager.getDouble2(-0.8, 0.8);
                    parent++; fils++;
                }
            }

			circlesInitial = new List<Circle>();

			// Création des cercles dans la représentation initiale des cercles.
			foreach (var item in circles)
				circlesInitial.Add(new Circle() { x = item.x, y = item.y, num = item.num, r = item.r, angleWithPrevious = item.angleWithPrevious, angleWithPreviousVelocity = item.angleWithPreviousVelocity });

			// Hiérarchisation de la représentation initiale des cercles.       
			return pack;
		}

		public static int InAnimFramesNumber = 10;

		public void ProcessInAnim(int frame, int animDuration, double centerX, double centerY)
		{
			// Calcul des valeurs initiales
			// le big one réduit et se déplace jusqu'à atteindre ses coordonnées initiales

			// récupération du cercle bigOnInitial
			Circle initialBigOne = circlesInitial.Single(c => c.num == bigOne.num);
			double bigOneXOffset = centerX - initialBigOne.x;
			double bigOneYOffset = centerY - initialBigOne.y;

			double initialAngleOffset = Math.PI / 3;

			// Contient la vélocité à affecter à chaque cercle pendant l'animation d'entrée
			List<double> velocities = new List<double>();
			// Contient la longueur à ajouter au rayon de chaque cercle pendant l'animation d'entrée
			List<double> radius = new List<double>();
			List<double> initalAngle = new List<double>();
			foreach (var circle in circlesInitial)
			{
				double sign = 1;
				if (circle.angleWithPreviousVelocity < 0) sign = -1;

				velocities.Add(initialAngleOffset/animDuration * sign); // 1/4 de cercle sur toute l'animation d'entrée

				initalAngle.Add(circle.angleWithPrevious);

				if (circle.num != bigOne.num)
					radius.Add(circle.r / Convert.ToDouble(animDuration - 1));
				else
					radius.Add(Convert.ToDouble(DiametreMax - circle.r) / Convert.ToDouble(animDuration -1));
			}

			// initialisation de la position et du rayon de chaque cercle
			//if (frame == 0)
			{
				for (int i = 0; i < circles.Count; i++)
				{
					double sign = 1;
					Circle c = circles[i];

					if (c.angleWithPreviousVelocity > 0) sign = -1;
					c.angleWithPrevious = initalAngle[i] + (sign) * initialAngleOffset + frame * velocities[i];
					//c.angleWithPreviousVelocity = velocities[i];
					if (c == bigOne) c.r = DiametreMax - frame * radius[i];
					else  c.r = (frame) * radius[i];
				}
			}



			PointF bigOneCenter = new PointF(Convert.ToSingle(centerX - frame * (bigOneXOffset / (animDuration - 1))), Convert.ToSingle(centerY - frame * (bigOneYOffset / (animDuration - 1))));

			// affectation des valeurs à chaque cercle.
			ProcessXY(centerX, centerY);
            

			// repositionnement de l'ensemble pour que le bigOne paraisse être le centre de la forme

			// Position du bigOne

			//for (int i = 0; i < animDuration; i++)
			//{
			//    bigOnePoints.Add(new PointF(Convert.ToSingle(centerX - i * (bigOneXOffset / (animDuration - 1))), Convert.ToSingle(centerY - i * (bigOneYOffset / (animDuration - 1)))));
			//    if (i < animDuration)
			//        bigOneRadius.Add(DiametreMax - (i * (DiametreMax - bigOne.r) / (animDuration - 1)));
			//    else
			//    {
			//        // 

			//    }

			//}

			//if (frame == 0)
			//{				
			//    // positionnement initial
			//    bigOne.r = DiametreMax;
			//    bigOne.x = centerX;
			//    bigOne.y = centerY;

			//}
			//else
			//{
			//    bigOne.r = DiametreMax - 
			//}

		}

		public void SetInitialCirclesToCurrentCirclesList()
		{
			circles.Clear();
			circles.AddRange(circlesInitial);
		}

        public void ProcessXY(double finalX, double finalY)
        {           
            // Calculate initial values (first circle is (0,0))
            Circle previous = null;
            Circle current = circles[0];
            current.x = finalX;
            current.y = finalY;

            int index = 1;
            while (index < circles.Count)
            {
                current = circles[index];
                previous = current.previous;
                //Console.WriteLine("Circle " + current.num + " angle = " + current.angleWithPrevious);
                current.x = previous.x + (previous.r + current.r) * Math.Cos(current.angleWithPrevious);
                current.y = previous.y + (previous.r + current.r) * Math.Sin(current.angleWithPrevious);
                index++;
            }            

            // Calculate center
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            foreach (Circle circle in circles)
            {
                if (circle.x - circle.r < minX) minX = circle.x - circle.r;
                if (circle.x + circle.r > maxX) maxX = circle.x + circle.r;
                if (circle.y - circle.r < minY) minY = circle.y - circle.r;
                if (circle.y + circle.r > maxY) maxY = circle.y + circle.r;               
            }
            double centerX = minX + (maxX - minX) / 2.0;
            double centerY = minY + (maxY - minY) / 2.0;

            // Change values
            double dx = finalX - centerX;
            double dy = finalY - centerY;
            index = 0;

            foreach (Circle circle in circles)
            {
                circle.x += dx;
                circle.y += dy;

                index++;
            }
        }

        public void InitialiseBigOneCoord()
        {
            circlesInitial[bigOne.num - 1].x = bigOne.x;
            circlesInitial[bigOne.num - 1].y = bigOne.y;
        }

        public void Draw(Graphics g)
        {          

            foreach (Circle circle in circles)
            {
                circle.Draw(g);
            }
            
            g.DrawLine(Pens.BlueViolet, new Point(400, 200), new Point(400, 600));
            g.DrawLine(Pens.BlueViolet, new Point(200, 400), new Point(600, 400));          
            g.DrawEllipse(Pens.Black, 400 - 305, 400 - 305, 305 * 2, 305 * 2);                   
        }

        public void Animate()
		{
			foreach (Circle circle in circles)
            {

                double deltaX = Math.Abs(circle.x - circle.BoundingCircle_x);
                double deltaY = Math.Abs(circle.y - circle.BoundingCircle_y);
                double dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                double distR = dist + circle.r;

                if (distR <= circle.BoundingCircle_r)
                {
                    circle.x += RandomManager.getDistance();                    
                }
                else if (distR > circle.BoundingCircle_r)
                {
                    
                }
            }
		}

        //public bool checkInCircle()
        //{
        //    bool checking = false;

        //    Circle circle = new Circle();        

        //    double deltaX = Math.Abs(circle.x - circle.BoundingCircle_x);
        //    double deltaY = Math.Abs(circle.y - circle.BoundingCircle_y);
        //    double dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        //    double distR = dist + circle.r;

        //    if (distR <= circle.BoundingCircle_r)
        //    {
        //        circle.x = RandomManager.getDistance();
        //    }
        //    return checking;
        //}      
       
		public bool CheckCollison()
		{
			bool result = false;

			for (int i = 0; i < circles.Count; i++)
			{
				for (int j = 0; j < circles.Count; j++)
				{
					Circle circle1 = circles[i];
					Circle circle2 = circles[j];
					if ((circle1 != circle2) && (circle1 != circle2.previous) && (circle2 != circle1.previous) && (i < j))
					{
						if (CheckCollison(circle1, circle2) == 1.0) result = true;
					}
				}
			}

			return result;
		}



		private double CheckCollison(Circle c1, Circle c2)
		{
			double result = 0;

			double deltaX = Math.Abs(c1.x - c2.x);
			double deltaY = Math.Abs(c1.y - c2.y);
			double dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

			double minDist = c1.r + c2.r;
			result = (1.1 * minDist - dist) / (0.8 * minDist);
			if (result < 0.0) result = 0.0;
			if (result > 1.0) result = 1.0;

			if (result == 0)
			{
				if (c1.collisions.ContainsKey(c2))
				{
					c1.collisions.Remove(c2);
					c1.amplifyVelocityCount = 2;
				}
				if (c2.collisions.ContainsKey(c1))
				{
					c2.amplifyVelocityCount = 2;
					c2.collisions.Remove(c1);
				}
			}
			else
			{
				if (c1.collisions.ContainsKey(c2)) c1.collisions[c2] = result; else c1.collisions.Add(c2, result);
				if (c2.collisions.ContainsKey(c1)) c2.collisions[c1] = result; else c2.collisions.Add(c1, result);
			}

			return result;
		}

		Dictionary<Circle, double> collisionOriginalVelocity = new Dictionary<Circle, double>();


		public void PreventCollision()
		{
			//			Console.WriteLine("===== PreventCollision 2 =====");
			foreach (Circle current in circles)
			{
				//				Console.WriteLine("----- Circle " + current.num + " -----");
				if (current.collisions.Count > 0)
				{
					//if (current.collisions.Count == 1)
					{
						//Console.WriteLine("CAS 1");
						// Une collision, on donne des impulsions opposées et dans le bon sens aux cercles en collision
						// 2 cas possibles :
						// Cas 1 : les cercles sont parents.
						// il n'y a pas de ramification entre les cercles (collision entre un cercle et 1 de ses ancêtres 
						// il faut donc modifier l'angle des noeuds
						Circle collisionCircle = null;
						foreach (var item in current.collisions) { collisionCircle = item.Key; break; }

						// Recherche de parent
						Circle parent = current.previous;
						bool found = false;
						if (parent != null)
						{

							Circle collisionNext = current; // noeud fils du noeud en collision parent de current
							while (parent != null && parent != collisionCircle)
							{
								collisionNext = parent; parent = collisionNext.previous;
								if (parent == collisionCircle) { found = true; break; }
							}

							if (parent != null) // noeud parent
							{
								// Calcul de l'angle entre les centre des noeud en collision
								double angleCollision = GetAngleBetweenCircles(collisionNext, current);
								double _2pi = Math.PI * 2;
								double collisionNextAngle = (collisionNext.angleWithPrevious - Math.PI) < 0 ? _2pi + (collisionNext.angleWithPrevious - Math.PI) : (collisionNext.angleWithPrevious - Math.PI);
								collisionNextAngle = (collisionNextAngle) % _2pi >= 0 ? collisionNextAngle % _2pi : _2pi + collisionNextAngle % _2pi;
								if (collisionNextAngle < angleCollision || (collisionNextAngle - angleCollision > Math.PI && collisionNextAngle > angleCollision))
								{
									// Alors current est plus à droite que parentNext
									// il faut appliquer une vélocité négative à current et une positive à 
									//if (current.angleWithPreviousVelocity <= 0) current.angleWithPreviousVelocity -= RandomManager.getDouble(0.001, 0.005);
									//else current.angleWithPreviousVelocity = 0;

									if (current.angleWithPreviousVelocity <= 0 || (current.angleWithPreviousVelocity > 0 && (current.angleWithPreviousVelocity < collisionNext.angleWithPreviousVelocity || Math.Abs(current.angleWithPreviousVelocity - collisionNext.angleWithPreviousVelocity) < 0.5)))
									{
										if (Math.Abs(current.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) current.angleWithPreviousVelocity += RandomManager.getDouble(0.0015, 0.006);
										else current.angleWithPreviousVelocity = current.angleWithPreviousVelocity / 2.0;

										if (Math.Abs(collisionNext.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) collisionNext.angleWithPreviousVelocity -= RandomManager.getDouble(0.0015, 0.006);
										else if (collisionNext.angleWithPreviousVelocity > 0) collisionNext.angleWithPreviousVelocity = collisionNext.angleWithPreviousVelocity / 2.0;

										if (current.angleWithPreviousVelocity > Circle.maxVelocity) current.angleWithPreviousVelocity = Circle.maxVelocity;
										if (collisionNext.angleWithPreviousVelocity < -Circle.maxVelocity) collisionNext.angleWithPreviousVelocity = -Circle.maxVelocity;
									}
								}
								else
								{
									if (current.angleWithPreviousVelocity >= 0 || (current.angleWithPreviousVelocity < 0 && (current.angleWithPreviousVelocity < collisionNext.angleWithPreviousVelocity || Math.Abs(current.angleWithPreviousVelocity - collisionNext.angleWithPreviousVelocity) < 0.5)))
									{
										if (Math.Abs(current.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) current.angleWithPreviousVelocity -= RandomManager.getDouble(0.0015, 0.006);
										else current.angleWithPreviousVelocity = current.angleWithPreviousVelocity / 2.0;

										if (Math.Abs(collisionNext.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) collisionNext.angleWithPreviousVelocity += RandomManager.getDouble(0.0015, 0.006);
										else collisionNext.angleWithPreviousVelocity = collisionNext.angleWithPreviousVelocity / 2.0;

										if (current.angleWithPreviousVelocity < -Circle.maxVelocity) current.angleWithPreviousVelocity = -Circle.maxVelocity;
										if (collisionNext.angleWithPreviousVelocity > Circle.maxVelocity) collisionNext.angleWithPreviousVelocity = Circle.maxVelocity;
									}

								}

								// Calcul de la proximité entre les bords des cercles : distance entre les centre - les rayons des cercles.
								// La proximité permet de déterminer la force de la répulsion entre les cercles.

							}
							else
							{
								if (current.num > 2)
								{
									// Noeud d'une ramification
									// Cas 2 : il y a une ramification, il faut retrouver le premier parent commun et modifier les angles des cercles problématiques.
									// Construction de la liste des parent de current
									List<Circle> currentParents = new List<Circle>();
									parent = current.previous;
									while (parent != null) { currentParents.Add(parent); parent = parent.previous; }
									List<Circle> collisionParents = new List<Circle>();
									parent = collisionCircle.previous;
									while (parent != null) { collisionParents.Add(parent); parent = parent.previous; }

									Circle commonParent = null;
									foreach (var item in currentParents)
										if (collisionParents.Contains(item)) { commonParent = item; break; }

									// calcul des angles entre le parent commun et les cercle en collision


									double angleCurrent = GetAngleBetweenCircles(commonParent, current);
									double angleCollision = GetAngleBetweenCircles(commonParent, collisionCircle);
									double _2pi = Math.PI * 2;
									double collisionNextAngle = angleCollision < 0 ? _2pi + (angleCollision - Math.PI) : (angleCollision);

									if ((collisionNextAngle < angleCurrent && angleCurrent - collisionNextAngle < Math.PI) || (collisionNextAngle - angleCurrent > Math.PI && collisionNextAngle > angleCurrent))
									{
										// Alors current est plus à droite que parentNext
										// il faut appliquer une vélocité négative à current et une positive à 
										//if (current.angleWithPreviousVelocity <= 0) current.angleWithPreviousVelocity -= RandomManager.getDouble(0.001, 0.005);
										//else current.angleWithPreviousVelocity = 0;

										if (current.angleWithPreviousVelocity <= 0 || (current.angleWithPreviousVelocity > 0 && (current.angleWithPreviousVelocity < collisionCircle.angleWithPreviousVelocity || Math.Abs(current.angleWithPreviousVelocity - collisionCircle.angleWithPreviousVelocity) < 0.05)))
										{
											if (Math.Abs(current.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) current.angleWithPreviousVelocity += RandomManager.getDouble(0.0015, 0.006);
											else current.angleWithPreviousVelocity = current.angleWithPreviousVelocity / 2.0;

											//if (Math.Abs(collisionCircle.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) collisionCircle.angleWithPreviousVelocity -= RandomManager.getDouble(0.0015, 0.006);
											//else collisionCircle.angleWithPreviousVelocity = collisionCircle.angleWithPreviousVelocity / 2.0;

											if (current.angleWithPreviousVelocity > Circle.maxVelocity) current.angleWithPreviousVelocity = Circle.maxVelocity;
											//if (collisionCircle.angleWithPreviousVelocity < -Circle.maxVelocity) collisionCircle.angleWithPreviousVelocity = -Circle.maxVelocity;
										}
									}
									else
									{
										if (current.angleWithPreviousVelocity >= 0 || (current.angleWithPreviousVelocity < 0 && (current.angleWithPreviousVelocity < collisionCircle.angleWithPreviousVelocity || Math.Abs(current.angleWithPreviousVelocity - collisionCircle.angleWithPreviousVelocity) < 0.5)))
										{
											if (Math.Abs(current.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) current.angleWithPreviousVelocity -= RandomManager.getDouble(0.0015, 0.006);
											else current.angleWithPreviousVelocity = current.angleWithPreviousVelocity / 2.0;

											//if (Math.Abs(collisionCircle.angleWithPreviousVelocity) < Circle.maxVelocity / 3.0) collisionCircle.angleWithPreviousVelocity += RandomManager.getDouble(0.0015, 0.006);
											//else collisionCircle.angleWithPreviousVelocity = collisionCircle.angleWithPreviousVelocity / 2.0;

											if (current.angleWithPreviousVelocity < -Circle.maxVelocity) current.angleWithPreviousVelocity = -Circle.maxVelocity;
											//if (collisionCircle.angleWithPreviousVelocity > Circle.maxVelocity) collisionCircle.angleWithPreviousVelocity = Circle.maxVelocity;
										}

									}

								}
							}
						}

					}
				}
				else
				{
					//Console.WriteLine("Nothing to do");
				}
				//Console.WriteLine("-----------------------");
			}
		}

		public double GetAngleBetweenCircles(Circle c1, Circle c2)
		{
			//
			//		c1 (fils)
			//		+
			//		 \			   
			//		  \		 
			//		   \
			//			+------------+ Origine
			//			c2 (parent)
			//
			double cos = c2.x - c1.x;
			double sin = c2.y - c1.y;
			double angle = Math.Acos(cos / Math.Sqrt(Math.Pow(cos, 2) + Math.Pow(sin, 2)));
			if (sin < 0) angle = Math.PI * 2 - angle;

			return angle;
		}


		public void PreventCollision_old()
		{
			Console.WriteLine("===== PreventCollision =====");

			foreach (Circle current in circles)
			{
				Console.WriteLine("----- Circle -----");
				if (current.collisions.Count > 0)
				{
					if (collisionOriginalVelocity.ContainsKey(current) == false)
					{
						if (current.previous != null)
						{
							Console.WriteLine("Adding " + current + " with velocity " + current.angleWithPreviousVelocity);
							collisionOriginalVelocity.Add(current, current.angleWithPreviousVelocity);
						}
					}

					if (collisionOriginalVelocity.ContainsKey(current) == true)
					{
						Console.WriteLine("Original Velocity = " + collisionOriginalVelocity[current]);
						Console.WriteLine("Current Max Collision = " + current.getMaxCollision());
						current.angleWithPreviousVelocity -= collisionOriginalVelocity[current] * current.getMaxCollision();
						Console.WriteLine("Calculted Velocity = " + current.angleWithPreviousVelocity);
						if (Math.Abs(current.angleWithPreviousVelocity) > Math.Abs(collisionOriginalVelocity[current])) current.angleWithPreviousVelocity = -collisionOriginalVelocity[current];
						Console.WriteLine("Corrected Velocity = " + current.angleWithPreviousVelocity);
					}
				}
				else
				{
					if (collisionOriginalVelocity.ContainsKey(current) == true)
					{
						Console.WriteLine("Removing " + current + " with velocity " + current.angleWithPreviousVelocity);
						collisionOriginalVelocity.Remove(current);
					}
				}
			}
		}

		public string GetFormattedPack()
		{
			string str = "<Harries>";

			int id = 1;
			foreach (Circle circle in circles)
			{
				str += "<Harry id=\"" + id.ToString() + "\" X=\"" + circle.x.ToString() + "\" Y=\"" + circle.y.ToString() + "\" Rot=\"0\" Rayon=\"" + circle.r.ToString() + "\" Scale=\"1\" />";
				id++;
			}
			str += "</Harries>";

			return str;

		}


	}

}
