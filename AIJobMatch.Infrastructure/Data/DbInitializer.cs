using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1. Check xem DB có dữ liệu chưa. Nếu có rồi (Count > 0) thì dừng ngay.
            if (context.Cities.Any()) return;

            // 2. Tìm đường dẫn file tree.json
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "tree.json");

            if (!File.Exists(path))
            {
                // Fallback: Tìm ở thư mục gốc nếu chạy local
                path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "SeedData", "tree.json");
            }

            if (!File.Exists(path)) return; // Không thấy file 

            // 3. Đọc và Deserialize JSON
            var jsonString = await File.ReadAllTextAsync(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // JSON gốc là một Dictionary lớn chứa các Tỉnh
            var citysDict = JsonSerializer.Deserialize<Dictionary<string, CityRequest>>(jsonString, options);

            if (citysDict == null) return;

            // 4. Chuẩn bị List để lưu (Batch Insert)
            var cities = new List<City>();
            var districts = new List<District>();
            var wards = new List<Ward>();

            // 5. VÒNG LẶP THẦN THÁNH (Nested Loop)
            foreach (var pDto in citysDict.Values)
            {
                // Tạo Tỉnh
                var city = new City
                {
                    CityCode = pDto.Code,
                    CityName = pDto.Name 
                };
                cities.Add(city);

                // Quét qua các Quận/Huyện của Tỉnh đó
                if (pDto.Districts != null)
                {
                    foreach (var dDto in pDto.Districts.Values)
                    {
                        var district = new District
                        {
                            DistrictCode = dDto.Code,
                            DistrictName = dDto.Name,
                            CityCode = pDto.Code // <--- Gán Khóa Ngoại thủ công ở đây
                        };
                        districts.Add(district);

                        // Quét qua các Xã/Phường của Huyện đó
                        if (dDto.Wards != null)
                        {
                            foreach (var wDto in dDto.Wards.Values)
                            {
                                var ward = new Ward
                                {
                                    WardCode = wDto.Code,
                                    WardName = wDto.Name,
                                    DistrictCode = dDto.Code // <--- Gán Khóa Ngoại thủ công ở đây
                                };
                                wards.Add(ward);
                            }
                        }
                    }
                }
            }

            // 6. Lưu xuống Database (Transaction để đảm bảo an toàn)
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Dùng AddRangeAsync để insert hàng loạt (Cực nhanh)
                await context.Cities.AddRangeAsync(cities);
                await context.Districts.AddRangeAsync(districts);
                await context.Wards.AddRangeAsync(wards);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
