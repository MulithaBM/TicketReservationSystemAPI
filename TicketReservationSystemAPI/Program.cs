using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Services.AdminService;
using TicketReservationSystemAPI.Services.AgentService;
using TicketReservationSystemAPI.Services.TravelerService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Configuration.GetSection("AppSettings").Get<JWTSettings>();

builder.Services.AddSingleton(provider =>
{
    var options = provider.GetService<IOptions<DbSettings>>() ?? throw new InvalidOperationException("MongoDbSettings is not configured properly");
    return new DataContext(options);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    c =>
    {
        c.AddSecurityDefinition("oauth2",
        new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme, e.g. \"bearer {token}\"",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        c.OperationFilter<SecurityRequirementsOperationFilter>();
    }
);

builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Admin services
builder.Services.AddSingleton<IAdminService, AdminService>();
builder.Services.AddSingleton<IAdminTravelerService, AdminTravelerService>();

builder.Services.AddSingleton<IAgentService, AgentService>();


builder.Services.AddSingleton<ITravelerService, TravelerService>();

string token = builder.Configuration["AppSettings:Token"] ?? throw new NullReferenceException("Missing token");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8
                .GetBytes(token)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<UserIdMiddleware>();

app.MapControllers();

app.Run();
