using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Services.AuthService;
using TicketReservationSystemAPI.Services.TravelerService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton(provider =>
{
    var options = provider.GetService<IOptions<DbSettings>>();

    if (options == null)
    {
        throw new InvalidOperationException("MongoDbSettings is not configured properly");
    }

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

builder.Services.AddSingleton<ITravelerAuthService, TravelerAuthservice>();
builder.Services.AddSingleton<IAgentAuthService, AgentAuthService>();

builder.Services.AddSingleton<ITravelerService, TravelerService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<UserIdMiddleware>();

app.MapControllers();

app.Run();
