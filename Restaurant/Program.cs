using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TableReservationApp
{
    static void Main(string[] args)
    {
        ReservationManager manager = new ReservationManager();
        manager.AddRestaurant("A", 10);
        manager.AddRestaurant("B", 5);

        Console.WriteLine(manager.BookTable("A", new DateTime(2023, 12, 25), 3)); // True
        Console.WriteLine(manager.BookTable("A", new DateTime(2023, 12, 25), 3)); // False

        // Додано модульні тести для нового функціоналу
        Console.WriteLine("Free Tables on 2023-12-25:");
        var freeTables = manager.FindAllFreeTables(new DateTime(2023, 12, 25));
        foreach (var table in freeTables)
        {
            Console.WriteLine(table);
        }

        manager.SortRestaurantsByAvailabilityForUsers(new DateTime(2023, 12, 25));

        Console.WriteLine("Sorted Restaurants by Availability:");
        foreach (var restaurant in manager.Restaurants)
        {
            Console.WriteLine($"{restaurant.Name} - Available Tables: {manager.CountAvailableTables(restaurant, new DateTime(2023, 12, 25))}");
        }
    }
}

public class ReservationManager
{
    public List<Restaurant> Restaurants { get; private set; }

    public ReservationManager()
    {
        Restaurants = new List<Restaurant>();
    }

    public void AddRestaurant(string name, int tableCount)
    {
        try
        {
            Restaurant restaurant = new Restaurant(name, tableCount);
            Restaurants.Add(restaurant);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
        }
    }

    private void LoadRestaurantsFromFile(string filePath)
    {
        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[1], out int tableCount))
                {
                    AddRestaurant(parts[0], tableCount);
                }
                else
                {
                    Console.WriteLine(line);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
        }
    }

    public List<string> FindAllFreeTables(DateTime date)
    {
        try
        {
            return Restaurants
                .SelectMany(r => r.Tables
                    .Where(t => !t.IsBooked(date))
                    .Select(t => $"{r.Name} - Table {t.TableNumber + 1}"))
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
            return new List<string>();
        }
    }

    public bool BookTable(string restaurantName, DateTime date, int tableNumber)
    {
        try
        {
            Restaurant restaurant = Restaurants.FirstOrDefault(r => r.Name == restaurantName);
            if (restaurant != null && tableNumber >= 0 && tableNumber < restaurant.Tables.Count)
            {
                return restaurant.Tables[tableNumber].Book(date);
            }

            throw new Exception("Invalid table number or restaurant not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
            return false;
        }
    }

    public void SortRestaurantsByAvailabilityForUsers(DateTime date)
    {
        try
        {
            Restaurants.Sort((r1, r2) =>
                CountAvailableTables(r2, date).CompareTo(CountAvailableTables(r1, date)));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
        }
    }

    public int CountAvailableTables(Restaurant restaurant, DateTime date)
    {
        try
        {
            return restaurant.Tables.Count(t => !t.IsBooked(date));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
            return 0;
        }
    }
}

public class Restaurant
{
    public string Name { get; private set; }
    public List<RestaurantTable> Tables { get; private set; }

    public Restaurant(string name, int tableCount)
    {
        Name = name;
        Tables = Enumerable.Range(1, tableCount)
            .Select(i => new RestaurantTable(i))
            .ToList();
    }
}

public class RestaurantTable
{
    public int TableNumber { get; private set; }
    private List<DateTime> BookedDates { get; set; }

    public RestaurantTable(int tableNumber)
    {
        TableNumber = tableNumber;
        BookedDates = new List<DateTime>();
    }

    public bool Book(DateTime date)
    {
        try
        {
            if (BookedDates.Contains(date))
            {
                return false;
            }

            BookedDates.Add(date);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error");
            return false;
        }
    }

    public bool IsBooked(DateTime date)
    {
        return BookedDates.Contains(date);
    }
}
