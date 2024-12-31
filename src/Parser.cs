using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vCDR.src
{
    public class Parser
    {
        /*
        {
            "RCode": "ABECLTHV",
            "Orig": "KABE",
            "Dest": "KCLT",
            "DepFix": "ETX",
            "Route String": "KABE ETX RAV J64 BURNI TYROO QUARM AIR HVQ LNDIZ PARQR4 KCLT",
            "DCNTR": "ZNY",
            "ACNTR": "ZTL",
            "TCNTRs": "ZID ZNY ZOB ZTL",
            "CoordReq": "N",
            "Play": "",
            "NavEqp": "1"
        }
        */
        public static List<JObject> ParseDatabase(string filePath)
        {
            var routes = new List<JObject>();
            var lines = File.ReadAllLines(filePath);
            if (!lines.Any()) return routes;
            var headers = lines[0].Split(',');
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split(',');
                if (fields.Length == headers.Length)
                {
                    var route = new JObject();

                    for (int i = 0; i < headers.Length; i++)
                    {
                        route[headers[i]] = fields[i];
                    }

                    routes.Add(route);
                }
            }
            return routes;
        }
    }
}
