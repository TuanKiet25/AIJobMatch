using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Ward
    {
        [Key]
        public string WardCode { get; set; }
        public string WardName { get; set; }
        public string DistrictCode { get; set; }
        public District? District { get; set; }
        public List<Address>? Addresses { get; set; }
    }
}
