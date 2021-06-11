using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArtPix_Dashboard.Models.Machine
{
    public class Machine
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id_for_api")]
        public string IdForApi { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public int JobsCount { get; set; }

        public int Workstation
		{
            get
			{
                var machine = int.Parse(Name);
                if (machine >= 1 && machine <= 4)
				{
                    return 1;
				}
                if (machine >= 5 && machine <= 8)
                {
                    return 2;
                }
                if (machine >= 9 && machine <= 12)
                {
                    return 3;
                }
                if (machine >= 13 && machine <= 14)
                {
                    return 4;
                }
                if (machine >= 15 && machine <= 16)
                {
                    return 5;
                }
                if (machine >= 18 && machine <= 21)
                {
                    return 6;
                }
                if (machine >= 22 && machine <= 25)
                {
                    return 7;
                }
                if (machine >= 26 && machine <= 27)
                {
                    return 8;
                }
                if (machine >= 28 && machine <= 29)
                {
                    return 9;
                }
                if (machine >= 30 && machine <= 32)
                {
                    return 10;
                }
                if (machine == 33)
                {
                    return 11;
                }
                return 12;
            }
		}

        public string JobsCountColor => JobsCount < 3 ? "DarkRed" : JobsCount < 5 ? "DarkOrange" : "DarkGreen";
    }

    public class MachineModel
    {
        [JsonProperty("data")]
        public List<Machine> Data { get; set; }
    }
}
