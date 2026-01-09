using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class CityRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } 
        
        // Quan trọng: Map key "quan-huyen" trong JSON vào biến Districts
        [JsonPropertyName("quan-huyen")]
        public Dictionary<string, DistricRequest> Districts { get; set; }
    }
}
