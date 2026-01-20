using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class AddressRequest
    {
        public string? Street { get; set; }
        
        [Required]
        public string CityCode { get; set; }

        [Required]
        public string DistrictCode { get; set; }

        [Required]
        public string WardCode { get; set; }
    }
}
