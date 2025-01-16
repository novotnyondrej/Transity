namespace Transity.Data.Games
{
	//Souradnice na mape
	internal class Coordinate
	{
		//X souradnice
		public readonly double X;
		//Y souradnice
		public readonly double Y;

		public Coordinate(
			double x,
			double y
		)
		{
			X = x;
			Y = y;
		}
		//Matematicke operace
		public static Coordinate operator +(Coordinate coordinateA, Coordinate coordinateB)
		{
			return new Coordinate(coordinateA.X + coordinateB.X, coordinateA.Y + coordinateB.Y);
		}
		public static Coordinate operator -(Coordinate coordinate)
		{
			return new Coordinate(-coordinate.X, -coordinate.Y);
		}
		public static Coordinate operator -(Coordinate coordinateA, Coordinate coordinateB)
		{
			return coordinateA + (-coordinateB);
		}
		public static Coordinate operator *(Coordinate coordinate, double value)
		{
			return new Coordinate(coordinate.X * value, coordinate.Y * value);
		}
		//Ziska vzdalenost od nuloveho bodu
		public double Size()
		{
			return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
		}
		//Ziska uhel, pod jakym se nachazi
		public double Angle()
		{
			if (X == 0)
			{
				if (Y > 0) return 90;
				else return 270;
			}
			return Math.Atan(Y / (double)X) * 180 / Math.PI;
		}
		//Ziska vzdalenost k jine souradnici
		public double GetDistanceTo(Coordinate coordinate)
		{
			return (this - coordinate).Size();
		}
	}
}