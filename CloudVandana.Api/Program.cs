using CloudVandana.Api.Configuration;
using CloudVandana.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SalesforceSettings>(builder.Configuration.GetSection("Salesforce"));

builder.Services.AddScoped<SalesforceAuthService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<SalesforceService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    // Angular is running on http://localhost:4200
    // API is running on https://localhost:7262
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("https://localhost:4200")
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


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseSwagger();

app.UseSwaggerUI();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
