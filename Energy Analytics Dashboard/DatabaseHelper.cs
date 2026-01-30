using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EnergyAnalyticsDashboard
{
    public static class DatabaseHelper
    {
        private static readonly string _connectionString = "Data Source=EnergyData.db;Version=3;";

        // Method to create the database and table if they don't exist
        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS DeviceConsumption (
                        DeviceName TEXT NOT NULL,
                        HoursUsed REAL NOT NULL,
                        CostPerHour REAL NOT NULL,
                        TotalCost REAL NOT NULL,
                        Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to insert data into the database
        public static void InsertData(string deviceName, double hoursUsed, double costPerHour, double totalCost)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO DeviceConsumption (DeviceName, HoursUsed, CostPerHour, TotalCost, Timestamp) VALUES (@DeviceName, @HoursUsed, @CostPerHour, @TotalCost, @Timestamp)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@DeviceName", deviceName);
                    command.Parameters.AddWithValue("@HoursUsed", hoursUsed);
                    command.Parameters.AddWithValue("@CostPerHour", costPerHour);
                    command.Parameters.AddWithValue("@TotalCost", totalCost);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to retrieve all data from the database
        public static string RetrieveData()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM DeviceConsumption";
                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    string data = "";
                    while (reader.Read())
                    {
                        data += $"Device: {reader["DeviceName"]}, Hours Used: {reader["HoursUsed"]}, Cost/Hour: ${reader["CostPerHour"]}, Total Cost: ${reader["TotalCost"]}, Timestamp: {reader["Timestamp"]}\n";
                    }
                    return data;
                }
            }
        }

        // Method to get daily consumption and cost for charts
        public static DataTable GetDailyConsumptionAndCost()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        strftime('%d', Timestamp) AS Day,
                        SUM(HoursUsed) AS Consumption,
                        SUM(TotalCost) AS Cost
                    FROM DeviceConsumption
                    GROUP BY Day
                    ORDER BY Day ASC";
                using (var command = new SQLiteCommand(query, connection))
                using (var adapter = new SQLiteDataAdapter(command))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table; // Returns a table with columns: Day, Consumption, Cost
                }
            }
        }

        // Method to export data to a PDF
        public static void ExportToPDF(string filePath, string data)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("Device Consumption Report\n\n", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(title);

                var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var content = new Paragraph(data, contentFont);
                document.Add(content);

                document.Close();
            }
        }

        // Method to export data to an Excel file
        public static void ExportToExcel(string filePath, string data)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Device Consumption");

                var lines = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int row = 1;
                foreach (var line in lines)
                {
                    worksheet.Cell(row, 1).Value = line;
                    row++;
                }

                workbook.SaveAs(filePath);
            }
        }
    }
}
