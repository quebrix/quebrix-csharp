using Example;
using QuebrixClient.QuebrixDependencyInjection;

var builder = WebApplication.CreateBuilder(args);


//Add Quebrix
builder.Services.AddQuebrix(op =>
{
    op.Host = "127.30.0.1";
    op.Port = 6022;
    op.UserName = "admin";
    op.Password = "123456";
}).WithCache()//use cache
.WithEFSharding<FakeSharder>().ByQuebrixDbContext<FakeAppDbContext>();//use sharding


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
