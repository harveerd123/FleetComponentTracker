
using FleetComponentTracker.Database;
using FleetComponentTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetComponentTracker.Services
{
    public class VehicleDataService
    {
        //Dependency Injection
        private readonly ApplicationDbContext _context;

        public VehicleDataService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Adding Vehicle table data, this will happen when program first runs (setup in Program.cs)
        public void SeedVehicleData()
        {
            var vehicles = new List<Vehicles>
            {
                new Vehicles { VehicleNumber = "VH001", FleetName = "Fleet A", DateIntoService = new DateOnly(2000, 1, 1), DateEndsService = new DateOnly(2030, 12, 31) },
                new Vehicles { VehicleNumber = "VH002", FleetName = "Fleet A", DateIntoService = new DateOnly(2000, 3, 15), DateEndsService = new DateOnly(2031, 3, 15) },
                new Vehicles { VehicleNumber = "VH003", FleetName = "Fleet B", DateIntoService = new DateOnly(2020, 6, 1), DateEndsService = new DateOnly(2050, 6, 1) },
                new Vehicles { VehicleNumber = "VH004", FleetName = "Fleet B", DateIntoService = new DateOnly(2020, 11, 20), DateEndsService = new DateOnly(2050, 11, 20) },
                new Vehicles { VehicleNumber = "VH005", FleetName = "Fleet C", DateIntoService = new DateOnly(2018, 8, 10), DateEndsService = new DateOnly(2031, 8, 10) },
                new Vehicles { VehicleNumber = "VH006", FleetName = "Fleet C", DateIntoService = new DateOnly(2018, 5, 5), DateEndsService = new DateOnly(2031, 5, 5) },
                new Vehicles { VehicleNumber = "VH007", FleetName = "Fleet D", DateIntoService = new DateOnly(1994, 2, 1), DateEndsService = new DateOnly(2026, 2, 1) },
                new Vehicles { VehicleNumber = "VH008", FleetName = "Fleet D", DateIntoService = new DateOnly(1994, 2, 1), DateEndsService = new DateOnly(2026, 2, 1) },
                new Vehicles { VehicleNumber = "VH009", FleetName = "Fleet A", DateIntoService = new DateOnly(2000, 9, 1), DateEndsService = new DateOnly(2030, 9, 1) },
                new Vehicles { VehicleNumber = "VH010", FleetName = "Fleet B", DateIntoService = new DateOnly(2020, 1, 1), DateEndsService = new DateOnly(2050, 1, 1) },
            };

            if (!_context.Vehicles.Any())
            {
                _context.Vehicles.AddRange(vehicles);
                _context.SaveChanges();
            }


        }

        //Retrieve all components, including Vehicle properties
        public async Task<List<FleetComponentTracker.Models.Components>> GetAllComponentsAsync()
        {
            return await _context.Components
                                 .Include(c => c.Vehicle)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        //Get individual component by unique id field
        public async Task<FleetComponentTracker.Models.Components?> GetComponentByIdAsync(int id)
        {
            return await _context.Components.FindAsync(id);
        }

        //Add component 
        public async Task AddComponentAsync(FleetComponentTracker.Models.Components component)
        {
            _context.Components.Add(component);
            await _context.SaveChangesAsync();
        }

        //Update component 
        public async Task UpdateComponentAsync(FleetComponentTracker.Models.Components component)
        {
            var existing = await _context.Components
                                         .Include(c => c.Vehicle)
                                         .FirstOrDefaultAsync(c => c.Id == component.Id);

            if (existing == null)
                throw new InvalidOperationException("Component not found");

            existing.SerialNumber = component.SerialNumber;
            existing.Description = component.Description;
            existing.InstallDate = component.InstallDate;

            existing.VehicleNumber = component.VehicleNumber;

            await _context.SaveChangesAsync();
        }

        //Delete component
        public async Task DeleteComponentAsync(int id)
        {
            var component = await _context.Components.FindAsync(id);
            if (component != null)
            {
                _context.Components.Remove(component);
                await _context.SaveChangesAsync();
            }
        }

    }
}