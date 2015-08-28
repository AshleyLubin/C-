using System;

namespace Circles
{
	public class RandomManager
	{
		private static Random random = null;

		public static double getDouble(double min, double max)
		{
			if (random == null) random = new Random(DateTime.Now.Millisecond);
			double result;
			do { result = min + (max - min) * random.NextDouble(); }
			while ((result < min) || (result > max));
			if (min == -0.8)
			Console.WriteLine("Random("+min+", "+max+") = "+ result);
			return result;
		}

		public static double getDouble2(double min, double max)
		{
			int res = random.Next(Convert.ToInt32(min * 1000), Convert.ToInt32(max * 1000));
			return Convert.ToDouble(res) / 1000.0;
		}

        public static double getDistance()
        {
            Random rnd = new Random();
            double NewpositionX = rnd.Next(100 /2);
                       
            return NewpositionX;

        }
	}
}
