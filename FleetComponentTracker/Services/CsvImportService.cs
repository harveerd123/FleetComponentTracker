using FleetComponentTracker.Database;

namespace FleetComponentTracker.Services
{
    public class CsvImportService
    {

        //Dependency Injection
        private readonly ApplicationDbContext _context;

        public CsvImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<FleetComponentTracker.Models.Components> ValidRows, List<string> Errors)> ImportCsvAsync(Stream fileStream)
        {
            var validRows = new List<FleetComponentTracker.Models.Components>();
            var errors = new List<string>();

            using var reader = new StreamReader(fileStream);
            string? line;
            int lineNumber = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                if (lineNumber == 1) continue; // Skip header column

                var fields = line.Split(',');

                if (fields.Length < 4)
                {
                    errors.Add($"Line {lineNumber}: Not enough fields.");
                    continue;
                }

                string serial = fields[0].Trim();
                string desc = fields[1].Trim();
                string vehicleNumber = fields[2].Trim();
                string dateString = fields[3].Trim();

                // Do check if fields are empty, if so don't add to list and go to next one
                if (string.IsNullOrWhiteSpace(serial))
                {
                    errors.Add($"Line {lineNumber}: SerialNumber is required.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(desc))
                {
                    errors.Add($"Line {lineNumber}: Description is required.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(vehicleNumber))
                {
                    errors.Add($"Line {lineNumber}: VehicleNumber is required.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(dateString))
                {
                    errors.Add($"Line {lineNumber}: InstallDate is required.");
                    continue;
                }

                //If data format is fine, parse it out as a DataOnly field (installDate)
                if (!DateOnly.TryParse(dateString, out var installDate))
                {
                    errors.Add($"Line {lineNumber}: Invalid InstallDate format '{dateString}'.");
                    continue;
                }

                // Validate foreign key relationship
                var vehicle = await _context.Vehicles.FindAsync(vehicleNumber);
                if (vehicle == null)
                {
                    errors.Add($"Line {lineNumber}: Vehicle '{vehicleNumber}' not found.");
                    continue;
                }

                var component = new FleetComponentTracker.Models.Components
                {
                    SerialNumber = serial,
                    Description = desc,
                    VehicleNumber = vehicleNumber,
                    InstallDate = installDate
                };

                validRows.Add(component);
            }

            // Save all valid components to DB
            if (validRows.Any())
            {
                _context.Components.AddRange(validRows);
                await _context.SaveChangesAsync();
            }

            return (validRows, errors);
        }

    }
}
