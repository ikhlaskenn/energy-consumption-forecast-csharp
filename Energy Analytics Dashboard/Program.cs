using System;
using System.Windows.Forms;

namespace EnergyAnalyticsDashboard
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                // Enable Windows Forms features for MessageBox
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Initialize the database
                Console.WriteLine("Initializing Database...");
                DatabaseHelper.InitializeDatabase();

                // Simulate data generation
                Console.WriteLine("Simulating Data...");
                Simulator simulator = new Simulator();
                simulator.SimulateAll();

                // Retrieve data from the database
                Console.WriteLine("Retrieving Data...");
                string allData = DatabaseHelper.RetrieveData();
                Console.WriteLine(allData);

                // Generate file paths for exports
                string pdfPath = "ConsumptionReport.pdf";
                string excelPath = "ConsumptionReport.xlsx";

                // Export data to PDF and Excel
                Console.WriteLine("Exporting Reports...");
                DatabaseHelper.ExportToPDF(pdfPath, allData);
                DatabaseHelper.ExportToExcel(excelPath, allData);

                // Notify the user with a message box
                MessageBox.Show(
                    $"Reports generated successfully:\nPDF: {pdfPath}\nExcel: {excelPath}",
                    "Export Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions and display an error message
                MessageBox.Show(
                    $"An error occurred: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
