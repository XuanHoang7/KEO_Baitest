using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Services;
using KEO_Baitest.Services.Implements;
using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add scoped repository
builder.Services.AddScoped<IPhieuVatTuRepository, PhieuVatTuRepository>();
builder.Services.AddScoped<IPhieuVatTuDetailRepository, PhieuVatTuDetailRepository>();
builder.Services.AddScoped<IThanhPhamRepository, ThanhPhamRepository>();
builder.Services.AddScoped<IPhieuThanhPhamDetailRepository, PhieuThanhPhamDetailRepository>();
builder.Services.AddScoped<IVatTuRepository, VatTuRepository>();
builder.Services.AddScoped<INhaCungCapRepository, NhaCungCapRepository>();
builder.Services.AddScoped<IDonViTinhRepository, DonViTinhRepository>();
builder.Services.AddScoped<INhomVatTuRepository, NhomVatTuRepository>();
builder.Services.AddScoped<INhomThanhPhamRepository, NhomThanhPhamRepository>();
builder.Services.AddScoped<IKhoVatTuRepository, KhoVatTuRepository>();
builder.Services.AddScoped<IThanhPhamRepository, ThanhPhamRepository>();
builder.Services.AddScoped<IKhoThanhPhamRepository, KhoThanhPhamRepository>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IQRVatTuRepository, QRVatTuRepository>();
builder.Services.AddScoped<IQRThanhPhamRepository, QRThanhPhamRepository>();
builder.Services.AddScoped<IPhieuThanhPhamRepository, PhieuThanhPhamRepository>();



//// Add scoped services
//builder.Services.AddScoped<IPhieuVatTuDetailService, Phieu>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IGenericService<NhaCungCapDTO>, NhaCungCapService>();
builder.Services.AddScoped<IGenericService<NhomVatTuDTO>, NhomVatTuService>();
builder.Services.AddScoped<IGenericService<NhomThanhPhamDTO>, NhomThanhPhamService>();
builder.Services.AddScoped<IGenericService<DonViTinhDTO>, DonViTinhService>();
builder.Services.AddScoped<IGenericService<KhoVatTuDTO>, KhoVatTuService>();
builder.Services.AddScoped<IGenericService<KhoThanhPhamDTO>, KhoThanhPhamService>();
builder.Services.AddScoped<IGenericService<VatTuDTO>, VatTuService>();
builder.Services.AddScoped<IGenericService<ThanhPhamDTO>, ThanhPhamService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQRVatTuService, QRVatTuService>();
builder.Services.AddScoped<IQRThanhPhamService, QRThanhPhamService>();
builder.Services.AddScoped<IPhieuThanhPhamService, PhieuThanhPhamService>();
builder.Services.AddScoped<IPhieuVatTuService, PhieuVatTuService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
ConfigurationManager configuration = builder.Configuration;
//builder.Services.AddScoped<IEmpoyeeServices, EmployeeServices>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true;
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Định nghĩa security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Áp dụng security scheme
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
});
//builder.Services.AddControllers();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}


app.MapControllers();
app.Run();
