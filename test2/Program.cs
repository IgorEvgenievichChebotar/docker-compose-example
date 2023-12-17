using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TestDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

const string thisServiceName = "test2";
const string anotherServiceName = "test1";

app.MapGet("/", () => new
    {
        message = "This is the service for testing communication with another service when using docker-compose. " +
                  $"You can GET /network to check if service {thisServiceName} is available."
    })
    .WithName("root")
    .WithOpenApi();


app.MapGet("/network", async () =>
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri($"http://{anotherServiceName}:8080/");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        var httpResponseMessage = await client.GetAsync("test");
        return await httpResponseMessage.Content.ReadAsStringAsync();
    })
    .WithName($"connectTo->{anotherServiceName}")
    .WithOpenApi();

app.MapGet("/test", () => new
    {
        message = $"service {thisServiceName} works properly!",
        takenFromDb = app.Services.GetService<TestDbContext>()?.testobjects?
            .FirstOrDefault(dbo => dbo.id == 1)?.label
    })
    .WithName("test")
    .WithOpenApi();

app.Run();

internal class TestDbContext : DbContext
{
    public DbSet<TestObject>? testobjects { get; set; }

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }
}

internal class TestObject
{
    public int id { get; set; }
    public string? label { get; set; }
}