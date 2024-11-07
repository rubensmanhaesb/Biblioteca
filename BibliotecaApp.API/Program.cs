using BibliotecaApp.API.Extensions;
using BibliotecaApp.Infra.Data.Extensions;
using BibliotecaApp.Aplication.Extensions;
using BibliotecaApp.Domain.Extensions;
using BibliotecaApp.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

#region Services - Extensions
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSwaggerConfig();
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices();
builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddCorsConfig(builder.Configuration);
builder.Services.AddJwtConfig(builder.Configuration);
#endregion

var app = builder.Build();

#region Middlewares
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseMiddleware<NotFoundExceptionMiddleware>();
app.UseMiddleware<RecordAlreadyExistsException>();
#endregion 

builder.Services.AddEndpointsApiExplorer();

#region Extensions - App
app.UseCorsConfig();
app.UseSwaggerConfig();
app.UseJwtConfig();
#endregion Extensions - App

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
