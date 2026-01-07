using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class District
    {
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string CityCode { get; set; }
        public City? City { get; set; }
        public List<Ward>? Wards { get; set; }
        public List<Address>? Addresses { get; set; }

    }
}
