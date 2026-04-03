using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Infrastructure;
using AIJobMatch.Infrastructure.Data;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayOS;
using System.Text;
using System.Text.Json.Serialization;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter JWT Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"]))
    };
});

//Đăng ký PayOS
builder.Services.AddSingleton(new PayOSClient(
    builder.Configuration["PayOS:ClientId"] ?? throw new Exception("Missing ClientId"),
    builder.Configuration["PayOS:ApiKey"] ?? throw new Exception("Missing ApiKey"),
    builder.Configuration["PayOS:ChecksumKey"] ?? throw new Exception("Missing ChecksumKey")
));

builder.Services.AddAuthorization();
builder.Services.Configure<TurnstileSettings>(builder.Configuration.GetSection("TurnstileSettings"));

var app = builder.Build();
//testconection elasticsearch
using (var scope = app.Services.CreateScope())
{
    // 1. Gọi đúng kiểu dữ liệu ElasticsearchClient của v8
    var elasticClient = scope.ServiceProvider.GetRequiredService<ElasticsearchClient>();

    // 2. Trong v8, hàm Ping trả về một đối tượng khác, ta dùng IsValidResponse
    var pingResponse = elasticClient.Ping();

    Console.WriteLine("\n=======================================");

    // 3. Kiểm tra IsValidResponse thay vì IsValid
    if (pingResponse.IsValidResponse)
    {
        Console.WriteLine("✅ KẾT NỐI ELASTICSEARCH V8 THÀNH CÔNG!");

        // 4. Cách lấy URL trong v8 có chút thay đổi qua Elasticstack settings
        // Nếu dòng dưới này quá dài, bạn có thể bỏ qua vì ta chỉ cần biết nó Success là đủ
        Console.WriteLine("✅ Trạng thái: Server đang phản hồi tốt.");
    }
    else
    {
        Console.WriteLine("❌ LỖI KẾT NỐI ELASTICSEARCH V8!");

        // 5. Lấy thông tin lỗi từ ElasticServerError hoặc DebugInformation
        if (pingResponse.ElasticsearchServerError != null)
        {
            Console.WriteLine($"❌ Chi tiết: {pingResponse.ElasticsearchServerError.Error.Reason}");
        }
        else
        {
            Console.WriteLine($"❌ Debug: {pingResponse.DebugInformation}");
        }
    }
    Console.WriteLine("=======================================\n");
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Tự động tạo bảng khi deploy
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Tự động tạo bảng nếu chưa có
        context.Database.Migrate();

        // Gọi hàm nạp dữ liệu
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi nạp dữ liệu hành chính.");
    }
}
// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AIJobMatch API v1");
        options.RoutePrefix = string.Empty; // Dòng thần thánh này giúp Swagger hiện ngay trang chủ
    });

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
