using System.Net.Http.Json;
using System.Text.Json.Serialization;


internal class Program
{
    private static async Task Main(string[] args)
    {
        var cities = await GetCitiesWithHighestPopulationInFranceAsync(15);

        if (cities != null && cities.Count > 0)
        {
            var cityAirQualityList = new List<CityAirQuality>();

            foreach (var city in cities)
            {
                Console.WriteLine($"Processing air quality for city: {city.Name}");
                var airQuality = await GetAirQualityAsync(city.Name);
                if (airQuality != null)
                {
                    cityAirQualityList.Add(new CityAirQuality
                    {
                        CityName = city.Name,
                        AirQualityIndex = airQuality.Value
                    });
                }
            }

            cityAirQualityList.Sort((a, b) => a.AirQualityIndex.CompareTo(b.AirQualityIndex));

            Console.WriteLine("Cities sorted by AQI:");
            foreach (var cityAirQuality in cityAirQualityList)
            {
                Console.WriteLine($"City: {cityAirQuality.CityName}, AQI: {cityAirQuality.AirQualityIndex}");
            }
        }

        Console.ReadLine();
    }

    private async static Task<List<CityResponse>> GetCitiesWithHighestPopulationInFranceAsync(int limit)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "aD5jOB3U948A4Mm6b/VuXw==1egSxgkPiCe4yUZ3");  

        var response = await client.GetAsync($"https://api.api-ninjas.com/v1/city?country=FR&limit={limit}");

        if (response.IsSuccessStatusCode)
        {
            var cities = await response.Content.ReadFromJsonAsync<List<CityResponse>>();
            if (cities != null && cities.Count > 0)
            {
                foreach (var city in cities)
                {
                    Console.WriteLine($"City: {city.Name}, Population: {city.Population}");
                }
            }
            else
            {
                Console.WriteLine("No cities found in France.");
            }
            return cities;
        }
        else
        {
            Console.WriteLine("Error fetching data");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error details: {errorContent}");
            return new List<CityResponse>();
        }
    }

    private async static Task<int?> GetAirQualityAsync(string city)
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://api.waqi.info/");
        var response = await client.GetAsync($"feed/{city}/?token=9f08b89a29ddf8fad59694a44fc0c5edf25a89dd");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AirQualityResponse>();
            if (result != null && result.Data != null && result.Data.Aqi != 0)
            {
                Console.WriteLine($"City: {result.Data.City.Name}, AQI: {result.Data.Aqi}");
                return result.Data.Aqi;
            }
            else
            {
                Console.WriteLine($"No data available for city: {city}");
                return null;
            }
        }
        else
        {
            Console.WriteLine("Error fetching data for city: " + city);
            return null;
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

    public class CityAirQuality
    {
        public string CityName { get; set; }
        public int AirQualityIndex { get; set; }
    }
}
