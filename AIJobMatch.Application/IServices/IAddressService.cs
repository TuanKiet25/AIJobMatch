using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IAddressService
    {
        public Task<List<CityReponse>> GetAllCityAsync();
        public Task<List<DistrictResponse>> GetAllDistrictByCityCodeAsync(string cityCode);
        public Task<List<WardResponse>> GetAllWardByDistrictCodeAsync(string districtCode);
    }
}
