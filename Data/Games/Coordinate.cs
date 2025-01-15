using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Transity.Data.Games
{
	internal class Coordinate
	{
		//X souradnice
		public readonly int X;
		//Y souradnice
		public readonly int Y;

		//Konstruktor
		public Coordinate(
			int x,
			int y
		)
		{
			X = x;
			Y = y;
		}
		//Ziska vzdalenost k jine souradnici
		public double GetDistanceTo(Coordinate coordinate)
		{
			return Math.Sqrt(Math.Pow(X - coordinate.X, 2) + Math.Pow(Y - coordinate.Y, 2));
		}
	}
}