using System.IO.Compression;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", GzipUtil.CompressDecompress("Scorching")
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class GzipUtil
{
    public static string CompressDecompress(string value)
    {
        var compressed = CompressToGZipString(value);

        return DecompressFromGZipString(compressed);
    }
    
    /// <summary>
    /// Compresses <paramref name="uncompressedString"/> and returns the result.
    /// </summary>
    /// <param name="uncompressedString">An uncompressed string.</param>
    /// <returns>A base-64 string.</returns>
    public static string CompressToGZipString(string uncompressedString)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                // Write uncompressed string to the gzip stream which
                // writes to the underlying memory stream.
                using (var writer = new StreamWriter(gzipStream))
                {
                    writer.Write(uncompressedString);
                }
            }

            // Return a base-64 string from the byte array
            // in the memory stream.
            return Convert.ToBase64String(
                outputStream.ToArray()
            );
        }
    }

    /// <summary>
    /// Decompresses <paramref name="compressedBase64String"/> and returns the result.
    /// </summary>
    /// <param name="compressedBase64String">A compressed base-64 string.</param>
    /// <returns>A string.</returns>
    public static string DecompressFromGZipString(string compressedBase64String)
    {
        // Convert the base-64 string to a byte array.
        var bytes = Convert.FromBase64String(compressedBase64String);
        using (var ms = new MemoryStream(bytes))
        {
            using (var gs = new GZipStream(ms, CompressionMode.Decompress))
            {
                // Read from the gzip stream which reads 
                // from the underlying memory stream.
                using (var reader = new StreamReader(gs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
