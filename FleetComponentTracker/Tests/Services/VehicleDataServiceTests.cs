using FleetComponentTracker.Models;
using FleetComponentTracker.Database;
using FleetComponentTracker.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FleetComponentTracker.Tests.Services
{
    public class VehicleDataServiceTests
    {
        //Creating a in-memory DbContext for testing purposes
        private DbContextOptions<ApplicationDbContext> CreateDbContext()
        {
            //Setup clean temp data for each test
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        //Add Component Test
        [Fact]
        public async Task AddComponentAsync_AddsComponent()
        {
            var options = CreateDbContext();

            var newComponent = new FleetComponentTracker.Models.Components
            {
                Id = 1,
                SerialNumber = "SN123",
                Description = "Test component",
                VehicleNumber = "VH001",
                InstallDate = new System.DateOnly(2023, 5, 1)
            };

            using (var context = new ApplicationDbContext(options))
            {
                var service = new VehicleDataService(context);
                await service.AddComponentAsync(newComponent);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var component = await context.Components.FindAsync(1);
                Assert.NotNull(component);
                Assert.Equal("SN123", component.SerialNumber);
            }
        }

        //Get Component by id Test
        [Fact]
        public async Task GetComponentByIdAsync_ReturnsComponent()
        {
            var options = CreateDbContext();

            using (var context = new ApplicationDbContext(options))
            {
                context.Components.Add(new FleetComponentTracker.Models.Components
                {
                    Id = 2,
                    SerialNumber = "SN456",
                    Description = "Another component",
                    VehicleNumber = "VH002",
                    InstallDate = new System.DateOnly(2023, 6, 1)
                });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new VehicleDataService(context);
                var component = await service.GetComponentByIdAsync(2);
                Assert.NotNull(component);
                Assert.Equal("SN456", component.SerialNumber);
            }
        }

        //Delete Component Test
        [Fact]
        public async Task DeleteComponentAsync_RemovesComponent()
        {
            var options = CreateDbContext();

            using (var context = new ApplicationDbContext(options))
            {
                context.Components.Add(new FleetComponentTracker.Models.Components
                {
                    Id = 3,
                    SerialNumber = "SN789",
                    Description = "To be deleted",
                    VehicleNumber = "VH003",
                    InstallDate = new System.DateOnly(2023, 7, 1)
                });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new VehicleDataService(context);
                await service.DeleteComponentAsync(3);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var component = await context.Components.FindAsync(3);
                Assert.Null(component);
            }
        }

        //Update Component Test
        [Fact]
        public async Task UpdateComponentAsync_UpdatesComponent()
        {
            var options = CreateDbContext();

            using (var context = new ApplicationDbContext(options))
            {
                context.Components.Add(new FleetComponentTracker.Models.Components
                {
                    Id = 4,
                    SerialNumber = "SN000",
                    Description = "description",
                    VehicleNumber = "VH004",
                    InstallDate = new System.DateOnly(2023, 8, 1)
                });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var service = new VehicleDataService(context);
                var component = await service.GetComponentByIdAsync(4);
                component.Description = "Updated description";
                await service.UpdateComponentAsync(component);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var component = await context.Components.FindAsync(4);
                Assert.Equal("Updated description", component.Description);
            }
        }
    }
}
