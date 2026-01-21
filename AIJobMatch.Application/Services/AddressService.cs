using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CityReponse>> GetAllCityAsync()
        {
            try
            {
                var cities = await _unitOfWork.cityRepository.GetAllAsync(null);
                if (cities == null || !cities.Any())
                {
                    throw new KeyNotFoundException("No cities found !!!");
                }
                var cityResponses = _mapper.Map<List<CityReponse>>(cities);
                return cityResponses;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DistrictResponse>> GetAllDistrictByCityCodeAsync(string cityCode)
        {
            try
            {
                var districts = await _unitOfWork.districtRepository.GetAllAsync(d => d.CityCode == cityCode);
                if (districts == null || !districts.Any())
                {
                    throw new KeyNotFoundException("No District found !!!");
                }
                var districtResponses = _mapper.Map<List<DistrictResponse>>(districts);
                return districtResponses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<WardResponse>> GetAllWardByDistrictCodeAsync(string districtCode)
        {
            try
            {
                var wards = await _unitOfWork.wardRepository.GetAllAsync(w => w.DistrictCode == districtCode);
                if (wards == null || !wards.Any())
                {
                    throw new KeyNotFoundException("No Ward found !!!");
                }
                var wardResponses = _mapper.Map<List<WardResponse>>(wards);
                return wardResponses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
