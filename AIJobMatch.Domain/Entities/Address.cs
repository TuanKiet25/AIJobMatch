using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public City? City { get; set; } 

        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public District? District { get; set; }
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public Ward? Ward { get; set; }

        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
