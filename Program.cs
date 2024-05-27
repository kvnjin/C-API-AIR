using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await GetCitiesWithHighestPopulationInFranceAsync(15);
        ThreadPool.QueueUserWorkItem(async state => await GetAirQualityAsync("paris"));
        Console.ReadLine();
    }

    private async static Task GetCitiesWithHighestPopulationInFranceAsync(int limit)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "aD5jOB3U948A4Mm6b/VuXw==1egSxgkPiCe4yUZ3");  // Remplacez par votre clé API de Ninja

        var response = await client.GetAsync($"https://api.api-ninjas.com/v1/city?country=FR&limit={limit}");

        if (response.IsSuccessStatusCode)
        {
            var cities = await response.Content.ReadFromJsonAsync<CityResponse[]>();
            if (cities != null && cities.Length > 0)
            {
                foreach (var city in cities)
                {
                    Console.WriteLine($"City: {city.Name}");
                    Console.WriteLine($"Population: {city.Population}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No cities found in France.");
            }
        }
        else
        {
            Console.WriteLine("Error fetching data");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error details: {errorContent}");
        }
    }

    private async static Task GetAirQualityAsync(string city)
    {
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

    public class CityResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("population")]
        public int Population { get; set; }
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
