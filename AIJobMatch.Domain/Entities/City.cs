using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class City
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public List<District>? Districts { get; set; }
        public List<Address>? Addresses { get; set; }
    }
}
