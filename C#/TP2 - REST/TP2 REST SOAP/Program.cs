﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static readonly HttpClient client = new HttpClient();

    static async Task Main()
    {
        string apiKey = "e65de5172e58282b856fd72204eb35c710d1e336";

        try
        {
            // Assume you have the user's provided latitude and longitude
            double userLatitude = 48.8566; // Example latitude
            double userLongitude = 2.3522; // Example longitude

            // Get stations for the specified contract
            List<Station> stations = await GetStationsAsync(apiKey);

            // Calculate distances to the stations and sort by distance
            stations.ForEach(station =>
            {
                station.distanceToTheStation = CalculateDistance(userLatitude, userLongitude, station.position.lat, station.position.lng);
            });

            // Sort stations by distance
            stations.Sort((s1, s2) => s1.distanceToTheStation.CompareTo(s2.distanceToTheStation));

            // Display sorted stations
            foreach (var station in stations)
            {
                Console.WriteLine($"Station: {station.name}, Distance: {station.distanceToTheStation} meters");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }

    static async Task<List<Station>> GetStationsAsync(string apiKey)
    {
        using HttpResponseMessage responseStations = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations?apiKey={apiKey}");
        responseStations.EnsureSuccessStatusCode();
        string responseBody = await responseStations.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Station>>(responseBody);
    }

    static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
       
        var coord1 = new GeoCoordinate(lat1, lon1);
        var coord2 = new GeoCoordinate(lat2, lon2);
        return coord1.GetDistanceTo(coord2);
    }
}

class Station
{
    public int number { get; set; }
    public string contract_name { get; set; }
    public Position position { get; set; }
    public string name { get; set; }
    public double distanceToTheStation { get; set; }
}

class Position
{
    public double lat { get; set; }
    public double lng { get; set; }
}
