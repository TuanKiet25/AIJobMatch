using AIJobMatch.Application.IServices;
using AIJobMatch.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpGet("city-name")]
        public async Task<IActionResult> GetAllCityName()
        {
            try
            {
                var result = await _addressService.GetAllCityAsync();
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("No city found !!!");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("district-name/{cityCode}")]
        public async Task<IActionResult> GetDistrictNameByCityCode(string cityCode)
        {
            try
            {
                var result = await _addressService.GetAllDistrictByCityCodeAsync(cityCode);
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound("No district found !!!");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }   

        }
        [HttpGet("ward-name/{districtCode}")]
        public async Task<IActionResult> GetWardNameByDistrictCode(string districtCode)
        {
            try
            {
                var result = await _addressService.GetAllWardByDistrictCodeAsync(districtCode);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(" No ward found !!!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
