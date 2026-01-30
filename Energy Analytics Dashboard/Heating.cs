using System;

public class Heating : Device
{
    public Heating() : base("Heating") { }

    public override void SimulateConsumption(Random random)
    {
        base.SimulateConsumption(random);
        // Add any Heating-specific behavior here if needed
    }
}
