using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Circles;

namespace Tester
{
	public partial class Form1 : Form
	{
		Graphics g;

		public Form1()
		{
			InitializeComponent();
			g = this.CreateGraphics();
		}

		List<Circle> circles = new List<Circle>();

		private void btnTestDraw_Click(object sender, EventArgs e)
		{
			Circle c1 = new Circle() { x = 400, y = 400, num = 1, r = 100 };
			Circle c2 = new Circle() { x = 400, y = 400, num = 2, r = 80, previous = c1, angleWithPrevious = Math.PI };
			c1.next.Add(c2);
			circles = new List<Circle>() { c1, c2 };

			Circle previous = c1;
			Circle current = c2;

			previous = current.previous;
			//Console.WriteLine("Circle " + current.num + " angle = " + current.angleWithPrevious);
			current.x = previous.x + (previous.r + current.r) * Math.Cos(current.angleWithPrevious);
			current.y = previous.y + (previous.r + current.r) * Math.Sin(current.angleWithPrevious);

			Draw(g);
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


	
	}
}
