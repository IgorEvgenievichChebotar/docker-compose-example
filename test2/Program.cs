using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
        message = "This is the service for testing communication with another service when using docker-compose." +
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

app.MapGet("/test", () => new { message = $"service {thisServiceName} works properly!" })
    .WithName("test")
    .WithOpenApi();

app.Run();