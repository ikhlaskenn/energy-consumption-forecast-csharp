using System;
using EnergyAnalyticsDashboard;

public abstract class Device
{
    public string Name { get; private set; }

    protected Device(string name)
    {
        Name = name;
    }

    public virtual void SimulateConsumption(Random random)
    {
        double hoursUsed = random.Next(1, 12); // Simulate between 1 to 12 hours of usage
        double costPerHour = Math.Round(random.NextDouble() * 5, 2); // Simulate cost/hour between 0 and 5
        double totalCost = Math.Round(hoursUsed * costPerHour, 2);

        // Insert simulated data into the database
        DatabaseHelper.InsertData(Name, hoursUsed, costPerHour, totalCost);
    }
}
