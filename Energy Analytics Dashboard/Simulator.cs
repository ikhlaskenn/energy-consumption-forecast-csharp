using System;
using System.Collections.Generic;

public class Simulator
{
    public List<Device> Devices { get; private set; }

    public Simulator()
    {
        Devices = new List<Device>
        {
            new AirConditioner(),
            new Heating(),
            new Fridge(),
            new Light()
        };
    }

    public void SimulateAll()
    {
        Random random = new Random();
        foreach (var device in Devices)
        {
            device.SimulateConsumption(random);
        }
    }
}
