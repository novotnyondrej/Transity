using Newtonsoft.Json;
using System.CodeDom.Compiler;

namespace Transity.Data.Games
{
    internal class Line
    {
        //Odkud
        [JsonProperty("from-city-index")]
        public int FromIndex;
		//Kam
		[JsonProperty("to-city-index")]
		public int ToIndex;

        [JsonConstructor]
        public Line(int fromIndex, int toIndex)
        {
            if (fromIndex > toIndex)
            {
                int temp = fromIndex;
                fromIndex = toIndex;
                toIndex = fromIndex;
            }
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
    }
}