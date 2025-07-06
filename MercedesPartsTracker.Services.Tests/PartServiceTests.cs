using MercedesPartsTracker.api.Controllers;
using MercedesPartsTracker.EntityFramework;
using MercedesPartsTracker.EntityFramework.Models;
using MercedesPartsTracker.EntityFrameworkServices.Implementations;
using MercedesPartsTracker.EntityFrameworkServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercedesPartsTracker.Services.Tests
{
    public class PartServiceTests
    {
        private readonly Mock<IPartService> _mockService;
        private readonly PartsController _controller;

        public PartServiceTests()
        {
            _mockService = new Mock<IPartService>();
            _controller = new PartsController(_mockService.Object);
        }

        // Integration test for in-memory database
        private DBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DBContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetAllPartsAsync_ReturnsParts()
        {
            using var context = GetInMemoryDbContext();
            context.Parts.Add(new Part { PartNumber = "P1", Description = "Test", QuantityOnHand = 10, LocationCode = "L01", LastStockTake = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new PartService(context);
            var parts = await service.GetAllPartsAsync();

            Assert.Single(parts);
        }

        [Fact]
        public async Task GetPartByNumberAsync_ReturnsPart()
        {
            using var context = GetInMemoryDbContext();
            context.Parts.Add(new Part { PartNumber = "P2", Description = "Oil", QuantityOnHand = 5, LocationCode = "L02", LastStockTake = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new PartService(context);
            var part = await service.GetPartByNumberAsync("P2");

            Assert.NotNull(part);
            Assert.Equal("Oil", part.Description);
        }

        [Fact]
        public async Task CreatePartAsync_AddsPart()
        {
            using var context = GetInMemoryDbContext();
            var service = new PartService(context);

            var part = new Part { PartNumber = "P3", Description = "New", QuantityOnHand = 3, LocationCode = "L03", LastStockTake = DateTime.UtcNow };
            var created = await service.CreatePartAsync(part);

            Assert.Equal("P3", created.PartNumber);
            Assert.Equal(1, context.Parts.Count());
        }

        [Fact]
        public async Task UpdatePartAsync_UpdatesPart()
        {
            using var context = GetInMemoryDbContext();

            var existingPart = new Part
            {
                PartNumber = "P4",
                Description = "Old",
                QuantityOnHand = 7,
                LocationCode = "L04",
                LastStockTake = DateTime.UtcNow
            };

            context.Parts.Add(existingPart);
            await context.SaveChangesAsync();

            var service = new PartService(context);

            existingPart.Description = "Updated";
            existingPart.QuantityOnHand = 8;
            existingPart.LocationCode = "L05";
            existingPart.LastStockTake = DateTime.UtcNow;

            var result = await service.UpdatePartAsync("P4", existingPart);

            Assert.True(result);
            var updated = await context.Parts.FindAsync("P4");
            Assert.Equal("Updated", updated.Description);
            Assert.Equal(8, updated.QuantityOnHand);
            Assert.Equal("L05", updated.LocationCode);
        }

        [Fact]
        public async Task DeletePartAsync_DeletesPart()
        {
            using var context = GetInMemoryDbContext();
            context.Parts.Add(new Part { PartNumber = "P5", Description = "Delete", QuantityOnHand = 1, LocationCode = "L05", LastStockTake = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new PartService(context);
            var result = await service.DeletePartAsync("P5");

            Assert.True(result);
            Assert.False(context.Parts.Any(p => p.PartNumber == "P5"));
        }

        [Fact]
        public async Task PartExistsAsync_ReturnsTrueIfExists()
        {
            using var context = GetInMemoryDbContext();
            context.Parts.Add(new Part { PartNumber = "P6", Description = "Exists", QuantityOnHand = 4, LocationCode = "L06", LastStockTake = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new PartService(context);
            var exists = await service.PartExistsAsync("P6");

            Assert.True(exists);
        }

        [Fact]
        public async Task PartExistsAsync_ReturnsFalseIfNotExists()
        {
            using var context = GetInMemoryDbContext();
            var service = new PartService(context);

            var exists = await service.PartExistsAsync("P999");

            Assert.False(exists);
        }

        // Unit tests for controller methods
        [Fact]
        public async Task GetParts_ReturnsOkWithList()
        {
            var parts = new List<Part> { new Part { PartNumber = "P1", Description = "Test" } };
            _mockService.Setup(s => s.GetAllPartsAsync()).ReturnsAsync(parts);

            var result = await _controller.GetParts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(parts, okResult.Value);
        }

        [Fact]
        public async Task GetPart_ReturnsOk_WhenPartExists()
        {
            var part = new Part { PartNumber = "P1", Description = "Test" };
            _mockService.Setup(s => s.GetPartByNumberAsync("P1")).ReturnsAsync(part);

            var result = await _controller.GetPart("P1");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(part, okResult.Value);
        }

        [Fact]
        public async Task GetPart_ReturnsNotFound_WhenPartMissing()
        {
            _mockService.Setup(s => s.GetPartByNumberAsync("P1")).ReturnsAsync((Part)null);

            var result = await _controller.GetPart("P1");

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePart_ReturnsBadRequest_WhenAlreadyExists()
        {
            var part = new Part { PartNumber = "P1", QuantityOnHand = 10 };
            _mockService.Setup(s => s.PartExistsAsync("P1")).ReturnsAsync(true);

            var result = await _controller.CreatePart(part);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("already exists", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CreatePart_ReturnsBadRequest_WhenQtyIsNegative()
        {
            var part = new Part { PartNumber = "P2", QuantityOnHand = -1 };
            _mockService.Setup(s => s.PartExistsAsync("P2")).ReturnsAsync(false);

            var result = await _controller.CreatePart(part);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("qty < 0", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CreatePart_ReturnsCreated_WhenValid()
        {
            var part = new Part { PartNumber = "P3", QuantityOnHand = 5 };
            _mockService.Setup(s => s.PartExistsAsync("P3")).ReturnsAsync(false);
            _mockService.Setup(s => s.CreatePartAsync(part)).ReturnsAsync(part);

            var result = await _controller.CreatePart(part);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(part, created.Value);
        }

        [Fact]
        public async Task UpdatePart_ReturnsBadRequest_WhenQtyIsNegative()
        {
            var part = new Part { PartNumber = "P1", QuantityOnHand = -5 };
            var result = await _controller.UpdatePart("P1", part);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("qty < 0", badRequest.Value.ToString());
        }

        [Fact]
        public async Task UpdatePart_ReturnsNotFound_WhenServiceFails()
        {
            var part = new Part { PartNumber = "P1", QuantityOnHand = 10 };
            _mockService.Setup(s => s.UpdatePartAsync("P1", part)).ReturnsAsync(false);

            var result = await _controller.UpdatePart("P1", part);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdatePart_Throws_WhenConcurrencyFails()
        {
            var part = new Part { PartNumber = "P1", QuantityOnHand = 10 };
            _mockService.Setup(s => s.UpdatePartAsync("P1", part)).ThrowsAsync(new DbUpdateConcurrencyException());
            _mockService.Setup(s => s.PartExistsAsync("P1")).ReturnsAsync(true);

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _controller.UpdatePart("P1", part));
        }

        [Fact]
        public async Task UpdatePart_ReturnsNoContent_WhenSuccess()
        {
            var part = new Part { PartNumber = "P1", QuantityOnHand = 10 };
            _mockService.Setup(s => s.UpdatePartAsync("P1", part)).ReturnsAsync(true);

            var result = await _controller.UpdatePart("P1", part);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePart_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.DeletePartAsync("P1")).ReturnsAsync(false);

            var result = await _controller.DeletePart("P1");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePart_ReturnsNoContent_WhenSuccess()
        {
            _mockService.Setup(s => s.DeletePartAsync("P1")).ReturnsAsync(true);

            var result = await _controller.DeletePart("P1");

            Assert.IsType<NoContentResult>(result);
        }
    }
}
