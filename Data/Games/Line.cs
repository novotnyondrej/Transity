using Newtonsoft.Json;

namespace Transity.Data.Games
{
    //Linka, ktera jezdi mezi dvema mesty
    internal class Line
    {
        //Odkud
        [JsonProperty("from-city-index")]
        public int FromIndex;
		//Kam
		[JsonProperty("to-city-index")]
		public int ToIndex;
        //Identifikator
        [JsonIgnore]
        public string Id => FromIndex + "_" + ToIndex;

        [JsonConstructor]
        public Line(int fromIndex, int toIndex)
        {
            //Aby v linkach byl nejaky system, stanovime pravidlo, ze prvni bude vzdycky ten nizsi index
            if (fromIndex > toIndex)
            {
                int temp = fromIndex;
                fromIndex = toIndex;
                toIndex = temp;
            }
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }
    }
}