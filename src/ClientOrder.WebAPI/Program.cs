using System.Security.Claims;
using System.Text.Json.Serialization;
using ClientOrder.IoC;
using ClientOrder.Services.Interfaces;
using ClientOrder.WebAPI.ExceptionHandling;
using ClientOrder.WebAPI.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization"
        };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var requiresAuth = metadata.OfType<IAuthorizeData>().Any();
        var allowAnonymous = metadata.OfType<IAllowAnonymous>().Any();

        if (requiresAuth && !allowAnonymous)
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer")] = []
            });
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddClientOrderServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserResolverService, UserResolverService>();

var keyText = builder.Configuration["JwtSettings:Key"]
    ?? throw new InvalidOperationException("Complete the JwtSettings configuration.");
var keyBytes = Convert.FromBase64String(keyText);

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException("The JWT key needs to be at least 32 random bytes.");
}

if (string.IsNullOrWhiteSpace(builder.Configuration["JwtSettings:Issuer"])
    || string.IsNullOrWhiteSpace(builder.Configuration["JwtSettings:Audience"])
    || builder.Configuration.GetValue<int>("JwtSettings:ExpirationInMinutes") <= 0)
{
    throw new InvalidOperationException("Complete the JwtSettings configuration.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "name",
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var subject = context.Principal?.FindFirst("sub")?.Value;

                if (!int.TryParse(subject, out var id) || id <= 0)
                {
                    context.Fail("A valid user ID is required.");
                    return;
                }

                var accountRoles = context.HttpContext.RequestServices.GetRequiredService<IAccountRoleService>();
                var role = await accountRoles.GetRoleAsync(id);

                if (string.IsNullOrWhiteSpace(role) || context.Principal?.Identity is not ClaimsIdentity identity)
                {
                    context.Fail("A valid user ID is required.");
                    return;
                }

                var roleClaims = identity.FindAll(identity.RoleClaimType)
                    .Concat(identity.FindAll("role"))
                    .Distinct()
                    .ToList();

                foreach (var claim in roleClaims)
                {
                    identity.RemoveClaim(claim);
                }

                identity.AddClaim(new Claim("role", role));

                if (!string.Equals(identity.RoleClaimType, "role", StringComparison.Ordinal))
                {
                    identity.AddClaim(new Claim(identity.RoleClaimType, role));
                }
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.Services.Seed();
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.AddPreferredSecuritySchemes("Bearer"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
