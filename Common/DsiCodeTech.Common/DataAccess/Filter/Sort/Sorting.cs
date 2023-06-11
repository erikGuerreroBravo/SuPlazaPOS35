using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Filter.Sort
{
    public class Sorting
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public Direction Direction { get; set; }

        public Sorting() { }

        public Sorting(string Name, Direction Direction)
        {
            this.Name = Name;
            this.Direction = Direction;
        }
    }
}
