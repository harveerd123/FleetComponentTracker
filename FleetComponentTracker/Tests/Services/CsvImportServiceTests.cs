using Xunit;
using Microsoft.EntityFrameworkCore;
using FleetComponentTracker.Services;
using FleetComponentTracker.Database;
using FleetComponentTracker.Models;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace FleetComponentTracker.Tests.Services
{

    public class CsvImportServiceTests
    {
        //Creating a in-memory DbContext with one Vehicle for testing purposes
        private ApplicationDbContext CreateDbContext()
        {
            //Setup clean temp data for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Vehicles.Add(new Vehicles
            {
                VehicleNumber = "V001",
                FleetName = "Test Fleet",
                DateIntoService = new DateOnly(2020, 1, 1),
                DateEndsService = new DateOnly(2030, 1, 1)
            });

            context.SaveChanges();
            return context;
        }

        //Create a MemoryStream from string so it works like a file when testing
        private Stream GenerateStream(string content) =>
            new MemoryStream(Encoding.UTF8.GetBytes(content));

        //Valid Component Test
        [Fact]
        public async Task ImportCsvAsync_ValidRow_ShouldReturnValidComponent()
        {
            var dbContext = CreateDbContext();
            var service = new CsvImportService(dbContext);
            string csv = "SerialNumber,Description,FleetName,InstallDate\nS001,Brake Pad,V001,2024-01-15";

            using var stream = GenerateStream(csv);
            var (validRows, errors) = await service.ImportCsvAsync(stream);

            Assert.Single(validRows);
            Assert.Empty(errors);
            Assert.Equal("S001", validRows[0].SerialNumber);
        }

        //Missing fields in Component test
        [Fact]
        public async Task ImportCsvAsync_MissingFields_ShouldReturnError()
        {
            var dbContext = CreateDbContext();
            var service = new CsvImportService(dbContext);
            string csv = "SerialNumber,Description,FleetName,InstallDate\nS001,Brake Pad";

            using var stream = GenerateStream(csv);
            var (validRows, errors) = await service.ImportCsvAsync(stream);

            Assert.Empty(validRows);
            Assert.Single(errors);
            Assert.Contains("Not enough fields", errors[0]);
        }

        //Invalid date in Component test
        [Fact]
        public async Task ImportCsvAsync_InvalidDate_ShouldReturnError()
        {
            var dbContext = CreateDbContext();
            var service = new CsvImportService(dbContext);
            string csv = "SerialNumber,Description,FleetName,InstallDate\nS001,Brake Pad,V001,no-date";

            using var stream = GenerateStream(csv);
            var (validRows, errors) = await service.ImportCsvAsync(stream);

            Assert.Empty(validRows);
            Assert.Contains("Invalid InstallDate format", errors[0]);
        }

        //Invalid Vehicle Number (checking relationship with Vehicle table)
        [Fact]
        public async Task ImportCsvAsync_UnknownVehicle_ShouldReturnError()
        {
            var dbContext = CreateDbContext();
            var service = new CsvImportService(dbContext);
            string csv = "SerialNumber,Description,FleetName,InstallDate\nS001,Brake Pad,V999,2024-01-15";

            using var stream = GenerateStream(csv);
            var (validRows, errors) = await service.ImportCsvAsync(stream);

            Assert.Empty(validRows);
            Assert.Contains("Vehicle 'V999' not found", errors[0]);
        }
    }

}
