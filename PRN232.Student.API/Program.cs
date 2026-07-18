using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRN232.LMS.Contracts;
using PRN232.Student.API;
using PRN232.Student.Repositories;
using PRN232.Student.Services;
using Serilog;
using Serilog.Formatting.Compact;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
Log.Logger = new LoggerConfiguration().WriteTo.Console(new CompactJsonFormatter()).CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, _, configuration) => configuration.Enrich.FromLogContext().WriteTo.Console(new CompactJsonFormatter()));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddStudentRepositories(builder.Configuration).AddStudentApplicationServices();
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<CourseEnrollmentLookup.CourseEnrollmentLookupClient>(options =>
    options.Address = new Uri(builder.Configuration["Grpc:CourseUrl"] ?? "http://localhost:9083"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt configuration is required.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Fail("Unauthorized"));
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return context.Response.WriteAsJsonAsync(ApiResponse<object?>.Fail("Forbidden"));
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "PRN232 Student Service", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT" });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = [] });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<StudentDbContext>().Database.MigrateAsync();
}
app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<StudentLookupGrpcService>();
app.MapHealthChecks("/health");
app.Run();

public partial class Program
{
}
