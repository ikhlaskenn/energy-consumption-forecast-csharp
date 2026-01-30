using System;

public class Fridge : Device
{
    public Fridge() : base("Fridge") { }

    public override void SimulateConsumption(Random random)
    {
        base.SimulateConsumption(random);
        // Add any Fridge-specific behavior here if needed
    }
}
