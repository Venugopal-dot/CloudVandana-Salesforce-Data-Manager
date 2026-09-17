using CloudVandana.Api.Configuration;
using CloudVandana.Api.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SalesforceSettings>(
    builder.Configuration.GetSection("Salesforce"));

builder.Services.AddScoped<SalesforceAuthService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<SalesforceService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // Required for Angular frontend and API running on different domains
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


// --------------------------------------------------
// Forwarded Headers
// Required when deployed behind Render's proxy
// --------------------------------------------------

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


// --------------------------------------------------
// CORS
// Local development:
// https://localhost:4200
//
// Production:
// FrontendUrl will be configured in Render
// --------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        var frontendUrl =
            builder.Configuration["FrontendUrl"]
            ?? "https://localhost:4200";

        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

var app = builder.Build();


// --------------------------------------------------
// Forwarded Headers
// Must be before HTTPS redirection
// --------------------------------------------------

app.UseForwardedHeaders();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// --------------------------------------------------
// HTTPS
// --------------------------------------------------

app.UseHttpsRedirection();


// --------------------------------------------------
// CORS
// --------------------------------------------------

app.UseCors("AngularPolicy");


// --------------------------------------------------
// Swagger
// --------------------------------------------------

app.UseSwagger();

app.UseSwaggerUI();


// --------------------------------------------------
// Session
// --------------------------------------------------

app.UseSession();


// --------------------------------------------------
// Authorization
// --------------------------------------------------

app.UseAuthorization();


// --------------------------------------------------
// Controllers
// --------------------------------------------------

app.MapControllers();

app.Run();