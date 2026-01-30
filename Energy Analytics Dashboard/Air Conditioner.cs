using System;

public class AirConditioner : Device
{
    public AirConditioner() : base("Air Conditioner") { }

    public override void SimulateConsumption(Random random)
    {
        base.SimulateConsumption(random);
        // Add any Air Conditioner-specific behavior here if needed
    }
}
