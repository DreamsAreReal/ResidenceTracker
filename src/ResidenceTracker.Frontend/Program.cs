using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Frontend.Components;
using ResidenceTracker.Infastructure.DataAccess.PostgreSql;
using ResidenceTracker.Infrastructure.Abstractions;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;
using ResidenceTracker.UseCases.Validation.Abstractions;
using ResidenceTracker.UseCases.Validation.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
builder.Services.AddMudServices();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddTransient<IRepository<Member>, MemberRepository>();
builder.Services.AddTransient<IRepository<House>, HouseRepository>();
builder.Services.AddTransient<IRepository<Flat>, FlatRepository>();
builder.Services.AddTransient<IRepository<ResidencyEventLog>, ResidencyEventLogRepository>();
builder.Services.AddTransient<IRepository<Bill>, BillRepository>();

builder.Services.AddTransient<AbstractFormValidator<Member>, MemberValidator>();
builder.Services.AddTransient<AbstractFormValidator<House>, HouseValidator>();
builder.Services.AddTransient<AbstractFormValidator<Flat>, FlatValidator>();
builder.Services.AddTransient<AbstractFormValidator<ResidencyEventLog>, ResidencyEventLogValidator>();
builder.Services.AddTransient<AbstractFormValidator<Bill>, BillValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();