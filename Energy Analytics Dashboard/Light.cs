using System;

public class Light : Device
{
    public Light() : base("Light") { }

    public override void SimulateConsumption(Random random)
    {
        base.SimulateConsumption(random);
        // Add any Light-specific behavior here if needed
    }
}
