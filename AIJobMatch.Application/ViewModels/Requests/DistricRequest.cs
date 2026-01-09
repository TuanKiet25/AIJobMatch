using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class DistricRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("xa-phuong")]
        public Dictionary<string, WardRequest> Wards { get; set; }
    }
}
