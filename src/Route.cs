using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace vCDR.src
{
    public class Route
    {
        public string Orig { get; set; }
        public string Dest { get; set; }
        public string Nav { get; set; }
        public string Coordination { get; set; }
        public string Crossing { get; set; }
        public string DepartureFix { get; set; }
        public string CodedRoute { get; set; }
        public string Altitude { get; set; }
        public string Note { get; set; }

        public static (double Latitude, double Longitude)? GetVOR(string name)
        {
            string vorPath = File.ReadAllText(Loader.LoadFile("Database", "vor.json"));
            JObject VORs = JObject.Parse(vorPath);
            var feature = VORs["features"]?.FirstOrDefault(f =>
                f["properties"]?["text"]?.Any(t => t.ToString() == name) == true);

            if (feature != null)
            {
                var coordinates = feature["geometry"]?["coordinates"];
                if (coordinates != null && coordinates.Type == JTokenType.Array)
                {
                    double longitude = coordinates[0]?.Value<double>() ?? 0;
                    double latitude = coordinates[1]?.Value<double>() ?? 0;
                    return (latitude, longitude);
                }
            }
            return null;
        }

        public static (double Latitude, double Longitude)? GetFix(string name)
        {
            string vorPath = File.ReadAllText(Loader.LoadFile("Database", "fixes.json"));
            JObject VORs = JObject.Parse(vorPath);
            var feature = VORs["features"]?.FirstOrDefault(f =>
                f["properties"]?["text"]?.Any(t => t.ToString() == name) == true);

            if (feature != null)
            {
                var coordinates = feature["geometry"]?["coordinates"];
                if (coordinates != null && coordinates.Type == JTokenType.Array)
                {
                    double longitude = coordinates[0]?.Value<double>() ?? 0;
                    double latitude = coordinates[1]?.Value<double>() ?? 0;
                    return (latitude, longitude);
                }
            }
            return null;
        }
    }
}
