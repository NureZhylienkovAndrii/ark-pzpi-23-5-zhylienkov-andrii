using System.Net.Http;
using System.Net.Http.Json;

Console.WriteLine("IoT device started...");

var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://localhost:7048")
};

const int periodDays = 30;

while (true)
{
    try
    {
        var bookings = await httpClient
            .GetFromJsonAsync<List<BookingDto>>("/api/iot/bookings?days=30");

        if (bookings == null || bookings.Count == 0)
        {
            Console.WriteLine("No bookings data received");
            await Task.Delay(10000);
            continue;
        }

        var stats = bookings
            .GroupBy(b => b.CarId)
            .Select(group =>
            {
                var bookedDays = group.Sum(b =>
                    (b.EndDate.Date - b.StartDate.Date).Days);

                var frequency = Math.Round(
                    (double)bookedDays / periodDays * 100, 2);

                return new CarBookingStat(group.Key, frequency);
            })
            .OrderByDescending(s => s.BookingFrequencyPercent)
            .ToList();

        var response = await httpClient
            .PostAsJsonAsync("/api/iot/analytics", stats);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[{DateTime.Now}] Analytics sent:");
            foreach (var stat in stats)
            {
                Console.WriteLine(
                    $"Car {stat.CarId}: {stat.BookingFrequencyPercent}%");
            }
        }
        else
        {
            Console.WriteLine($"Error sending analytics: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"IoT error: {ex.Message}");
    }

    await Task.Delay(10000);
}

record BookingDto(int CarId, DateTime StartDate, DateTime EndDate);

record CarBookingStat
(
    int CarId,
    double BookingFrequencyPercent
);
