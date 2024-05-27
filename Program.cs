using System.Net.Http.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ThreadPool.QueueUserWorkItem(GetUrl);
        Console.ReadLine();
    }

    private async static void GetUrl(object state)
    {
        string city = "paris";
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://api.waqi.info/");
        var response = await client.GetAsync($"feed/" + city + "/?token=9f08b89a29ddf8fad59694a44fc0c5edf25a89dd");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AirQualityResponse>();
            Console.WriteLine($"City: {result.Data.City.Name}");
            Console.WriteLine($"AQI: {result.Data.Aqi}");
        }
        else
        {
            Console.WriteLine("Error fetching data");
        }
    }

    public class AirQualityResponse
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonPropertyName("aqi")]
        public int Aqi { get; set; }

        [JsonPropertyName("city")]
        public City City { get; set; }
    }

    public class City
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
