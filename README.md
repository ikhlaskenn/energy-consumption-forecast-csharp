# Energy Consumption Analytics and Reporting Tool (C#)

A desktop application that models household appliances as objects, generates consumption sessions for them, stores every session in a SQLite database, and exports the result as a PDF report and an Excel workbook.

The point of the project is the **pipeline**: an object-oriented device model feeding a persistent store, and a reporting layer reading back from it — the full path from data generation to a document a user can open.

![Language](https://img.shields.io/badge/language-C%23-239120)
![Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4)
![Database](https://img.shields.io/badge/database-SQLite-003B57)
![UI](https://img.shields.io/badge/UI-Windows%20Forms-0078D4)

## What it does

On each run the application:

1. Creates the SQLite database and the `DeviceConsumption` table if they do not exist.
2. Walks the list of appliances and generates one consumption session per device: hours used, cost per hour, and the resulting total cost.
3. Writes every session to the database with a timestamp, so runs accumulate into a history.
4. Reads the full table back.
5. Exports it to `ConsumptionReport.pdf` (iTextSharp) and `ConsumptionReport.xlsx` (ClosedXML), then confirms with a message box.

## Design

The appliance model is where the object-oriented work is. `Device` is an abstract base class holding the shared behaviour — the simulation of a session and its insertion into the database — and each appliance derives from it:

```
Device (abstract)
 ├── AirConditioner
 ├── Heating
 ├── Fridge
 └── Light
```

`Simulator` holds the devices as a `List<Device>` and iterates over them without knowing their concrete types:

```csharp
public void SimulateAll()
{
    Random random = new Random();
    foreach (var device in Devices)
    {
        device.SimulateConsumption(random);
    }
}
```

Adding a new appliance is a new class and one line in the list — nothing else changes. `SimulateConsumption` is `virtual` precisely so that an appliance with its own consumption profile (a fridge running continuously, heating that depends on the season) can override it without touching the rest.

Persistence is isolated in a single static class, `DatabaseHelper`, which owns the connection string, the schema, the inserts and the exports. Inserts use parameterized queries rather than string concatenation, so the values cannot break the statement.

## Data model

| Column | Type | Meaning |
|---|---|---|
| `DeviceName` | TEXT | Appliance that produced the session |
| `HoursUsed` | REAL | Hours of use in this session |
| `CostPerHour` | REAL | Unit cost applied |
| `TotalCost` | REAL | `HoursUsed × CostPerHour` |
| `Timestamp` | DATETIME | Insertion time, defaults to the current time |

`GetDailyConsumptionAndCost()` aggregates this table by day with `strftime('%d', Timestamp)`, returning a `DataTable` of day, total hours and total cost — the query a chart would bind to.

## Technology stack

| Layer | Choice |
|---|---|
| Runtime | .NET Framework 4.7.2, Windows Forms (`WinExe`) |
| Storage | SQLite via `System.Data.SQLite` |
| PDF export | iTextSharp |
| Excel export | ClosedXML |

## How to run

1. Open `Energy Analytics Dashboard.sln` in Visual Studio.
2. Restore the NuGet packages listed in `packages.config`.
3. Build and run.
4. `EnergyData.db`, `ConsumptionReport.pdf` and `ConsumptionReport.xlsx` appear in the output directory.

Each run appends a new set of sessions, so the database grows and the reports get longer.

## Project structure

```
Energy Analytics Dashboard/
  Program.cs            entry point: init, simulate, read back, export
  Device.cs             abstract base class, shared session logic
  Light.cs              appliance
  Fridge.cs             appliance
  Heating.cs            appliance
  Air Conditioner.cs    appliance
  Simulator.cs          device list and iteration
  DatabaseHelper.cs     schema, inserts, queries, PDF and Excel export
  Form1.cs              Windows Forms shell
  packages.config       NuGet dependencies
```

## Current state and limits

Worth stating plainly, since it shapes what comes next:

- **The data is synthetic.** Hours and unit cost are drawn randomly per session. There is no measurement input and no tariff model, so the figures demonstrate the pipeline rather than describe a real household.
- **No forecasting yet.** The application analyses and reports what has been recorded; it does not project future consumption. That is the next step, and `GetDailyConsumptionAndCost()` already provides the time series it would need.
- **The entry point is batch, not interactive.** `Main()` runs the whole pipeline and reports through a message box; the Windows Forms window is not yet wired to the model.
- **The Excel export writes one line per row in a single column**, mirroring the text report rather than producing typed columns.

## Roadmap

- [x] Object-oriented appliance model, SQLite persistence, PDF and Excel export
- [ ] Wire the Windows Forms UI: appliance selection, manual hours and tariff input, results grid
- [ ] Typed Excel export with one column per field, so the data can be filtered and charted
- [ ] Daily consumption chart bound to `GetDailyConsumptionAndCost()`
- [ ] Per-appliance consumption profiles overriding `SimulateConsumption`
- [ ] Forecasting: moving average or linear regression over the stored history

## Author

**Ikhlass Kennich**, MSc student in Electronics, Electrical Energy and Automatic Control (E3A), Université Paris-Saclay.

## License

MIT
