using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Repositories;

namespace PrivateWorkshop.Tests
{
    public class WorkshopRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public WorkshopRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("PrivateWorkshopDb")
                .Options;
        }
        private ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);

        [Fact]
        public async Task AddAsync_ShouldAddWorkshop()
        {
            // db context
            var db = CreateDbContext();

            // create WorkshopRespository
            var repository = new WorkshopRepository(db);

            // create workshop
            var workshop = new Workshop
            {
                Title = "Test Title",
                Description = "Test Description",
                Instructor = "Sir Lewis Hamilton",
                Price = 500,
                MaxSlot = 3
            };

            //execute
            await repository.AddAsync(workshop);

            //result
            var result = db.Workshops.Find(workshop.Id);

            //assert
            Assert.NotNull(result);
            Assert.Equal("Test Description", result.Description);
        }
        [Fact]
        public async Task GetAllAsync_ShouldGetAllWorkshop()
        {
            var db = CreateDbContext();

            var repository = new WorkshopRepository(db);

            var workshop1 = new Workshop
            {
                Title = "Test Title1",
                Description = "Test Description1",
                Instructor = "Test Instuctor1",
                Price = 500,
                MaxSlot = 3
            };
            var workshop2 = new Workshop
            {
                Title = "Test Title2",
                Description = "Test Description2",
                Instructor = "Test Instuctor2",
                Price = 500,
                MaxSlot = 3
            };

            await db.Workshops.AddRangeAsync(workshop1, workshop2);
            await db.SaveChangesAsync();

            var result = await repository.GetAllAsync();
            Assert.NotNull(result);
            Assert.True(result.Count() >= 2);
        }
        [Fact]
        public async Task GetByIdAsync_ShouldGetWorkshopById()
        {
            var db = CreateDbContext();

            var repository = new WorkshopRepository(db);

            var workshop = new Workshop
            {
                Title = "Test Title Get by Id",
                Description = "Test Description Get by Id",
                Instructor = "Test Instuctor Get by Id",
                Price = 500,
                MaxSlot = 3
            };

            await db.Workshops.AddAsync(workshop);
            await db.SaveChangesAsync();

            var result = await repository.GetByIdAsync(workshop.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Title Get by Id", result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowKeyNotFoundException()
        {
            var db = CreateDbContext();

            var repository = new WorkshopRepository(db);

            var invalidId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.GetByIdAsync(invalidId)
            );
        }
        [Fact]
        public async Task UpdateAsync_ShouldUpdateWorkshop()
        {
            var db = CreateDbContext();

            var respository = new WorkshopRepository(db);

            var workshop = new Workshop
            {
                Title = "Test Title",
                Description = "Test Description",
                Instructor = "Sir Lewis Hamilton",
                Price = 500,
                MaxSlot = 3
            };

            await db.Workshops.AddAsync(workshop);
            await db.SaveChangesAsync();

            workshop.Description = "Test Description Update";

            await respository.UpdateAsync(workshop);

            var result = await db.Workshops.FindAsync(workshop.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Description Update", result.Description);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteWorkshop()
        {
            var db = CreateDbContext();

            var respository = new WorkshopRepository(db);

            var workshop = new Workshop
            {
                Title = "Test Title",
                Description = "Test Description",
                Instructor = "Sir Lewis Hamilton",
                Price = 500,
                MaxSlot = 3
            };

            await db.Workshops.AddAsync(workshop);
            await db.SaveChangesAsync();

            await respository.DeleteAsync(workshop.Id);
            var result = db.Workshops.Find(workshop.Id);

            Assert.Null(result);
        }
    }
}
