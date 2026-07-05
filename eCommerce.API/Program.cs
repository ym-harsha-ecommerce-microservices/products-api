using eCommerce.BLL;
using eCommerce.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);

builder.Services.AddBusinessLogicLayer();

var app = builder.Build();

app.Run();
