using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Repositories;
using PrivateWorkshop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PrivateWorkshop.Tests
{
    public class BookingRepositoryTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new ApplicationDbContext(options);
        }

        private BookingCreateViewModel CreateModel(Guid workshopId) =>
            new BookingCreateViewModel
            {
                WorkshopId = workshopId,
                SelectedDate = DateOnly.FromDateTime(DateTime.Today),
                Duration = TimeSlot.Morning
            };

        private Booking CreateBooking(Guid workshopId, string clientId,
            DateOnly? date = null,
            TimeSlot duration = TimeSlot.Morning,
            BookingStatus status = BookingStatus.Pending)
        {
            return new Booking
            {
                Id = Guid.NewGuid(),
                WorkshopId = workshopId,
                ClientId = clientId,
                Date = date ?? DateOnly.FromDateTime(DateTime.Today),
                Duration = duration,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
        }

        private IdentityUser CreateUser(string id, string email) =>
            new IdentityUser
            {
                Id = id,
                UserName = email,
                Email = email
            };

        private Workshop CreateWorkshop(Guid id, string title) =>
            new Workshop
            {
                Id = id,
                Title = title,
                Description = "Test Description",
                Instructor = "Test Instructor",
                Price = 1000,
                MaxSlot = 3
            };

        [Fact]
        public async Task CreateBookingAsync_ShouldFail_WhenWorkshopNotFound()
        {
            var db = CreateDbContext();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var model = CreateModel(Guid.NewGuid());

            workshopRepo
                .Setup(r => r.GetByIdAsync(model.WorkshopId))
                .ReturnsAsync((Workshop?)null);

            var result = await repo.CreateBookingAsync(model, "user1");

            Assert.False(result.Succeeded);
            Assert.Equal("Workshop not found", result.ErrorMessage);
            Assert.Empty(db.Bookings);

            workshopRepo.Verify(r => r.GetByIdAsync(model.WorkshopId), Times.Once);
            slotRepo.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldFail_WhenSlotFull()
        {
            var db = CreateDbContext();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var wsId = Guid.NewGuid();
            var workshop = new Workshop { Id = wsId, MaxSlot = 1 };
            var slot = new WorkshopSlot { WorkshopId = wsId, BookedCount = 1, MaxSlot = 1 };

            workshopRepo.Setup(r => r.GetByIdAsync(wsId)).ReturnsAsync(workshop);
            slotRepo.Setup(r => r.GetOrCreateSlotAsync(wsId,
                                                       It.IsAny<DateOnly>(),
                                                       TimeSlot.Morning,
                                                       workshop.MaxSlot))
                    .ReturnsAsync(slot);

            var model = CreateModel(wsId);

            var result = await repo.CreateBookingAsync(model, "user1");

            Assert.False(result.Succeeded);
            Assert.Equal("Time slot full", result.ErrorMessage);
            Assert.Empty(db.Bookings);

            workshopRepo.Verify(r => r.GetByIdAsync(wsId), Times.Once);
            slotRepo.Verify(r => r.GetOrCreateSlotAsync(wsId,
                                                        model.SelectedDate,
                                                        model.Duration,
                                                        workshop.MaxSlot),
                            Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldAddBookingToDatabase()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var booking = CreateBooking(Guid.NewGuid(), "user1");

            await repo.AddAsync(booking);

            var saved = await db.Bookings.FindAsync(booking.Id);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task CountBookingsAsync_ShouldReturnCorrectCount()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var workshopId = Guid.NewGuid();
            var date = DateOnly.FromDateTime(DateTime.Today);

            db.Bookings.Add(CreateBooking(workshopId, "user1", date));
            db.Bookings.Add(CreateBooking(workshopId, "user2", date));
            db.Bookings.Add(CreateBooking(Guid.NewGuid(), "user3", date)); 
            await db.SaveChangesAsync();

            var result = await repo.CountBookingsAsync(workshopId, date, TimeSlot.Morning);

            Assert.Equal(2, result);
        }


        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBookings_WithIncludes()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var user1 = CreateUser("u1", "u1@test.com");
            var user2 = CreateUser("u2", "u2@test.com");

            var ws1 = CreateWorkshop(Guid.NewGuid(), "WS1");
            var ws2 = CreateWorkshop(Guid.NewGuid(), "WS2");

            await db.Users.AddRangeAsync(user1, user2);
            await db.Workshops.AddRangeAsync(ws1, ws2);

            await db.Bookings.AddRangeAsync(
                CreateBooking(ws1.Id, user1.Id),
                CreateBooking(ws2.Id, user2.Id)
            );

            await db.SaveChangesAsync();

            var result = await repo.GetAllAsync();
            var list = result.ToList();

            Assert.Equal(2, list.Count);
            Assert.All(list, b =>
            {
                Assert.NotNull(b.Client);
                Assert.NotNull(b.Workshop);
            });
        }

        [Fact]
        public async Task GetByClientIdAsync_ShouldReturnBookingsForClient_WithIncludes()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var user1 = CreateUser("client1", "c1@test.com");
            var user2 = CreateUser("client2", "c2@test.com");
            var ws = CreateWorkshop(Guid.NewGuid(), "WS");

            await db.Users.AddRangeAsync(user1, user2);
            await db.Workshops.AddAsync(ws);

            await db.Bookings.AddRangeAsync(
                CreateBooking(ws.Id, user1.Id),
                CreateBooking(ws.Id, user1.Id),
                CreateBooking(ws.Id, user2.Id)
            );

            await db.SaveChangesAsync();

            var result = await repo.GetByClientIdAsync(user1.Id);
            var list = result.ToList();

            Assert.Equal(2, list.Count);
            Assert.All(list, b =>
            {
                Assert.Equal(user1.Id, b.ClientId);
                Assert.NotNull(b.Client);
                Assert.NotNull(b.Workshop);
            });
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectBooking_WithIncludes()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var user = CreateUser("user1", "user@test.com");
            var ws = CreateWorkshop(Guid.NewGuid(), "WS");

            await db.Users.AddAsync(user);
            await db.Workshops.AddAsync(ws);

            var booking = CreateBooking(ws.Id, user.Id);
            await db.Bookings.AddAsync(booking);
            await db.SaveChangesAsync();

            var result = await repo.GetByIdAsync(booking.Id);

            Assert.NotNull(result);
            Assert.Equal(booking.Id, result!.Id);
            Assert.NotNull(result.Client);
            Assert.NotNull(result.Workshop);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var result = await repo.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyBooking()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var booking = CreateBooking(Guid.NewGuid(), "user1");
            await db.Bookings.AddAsync(booking);
            await db.SaveChangesAsync();

            booking.Status = BookingStatus.Approved;

            await repo.UpdateAsync(booking);

            var updated = await db.Bookings.FindAsync(booking.Id);
            Assert.NotNull(updated);
            Assert.Equal(BookingStatus.Approved, updated!.Status);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBooking()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var booking = CreateBooking(Guid.NewGuid(), "user1");
            await db.Bookings.AddAsync(booking);
            await db.SaveChangesAsync();

            await repo.DeleteAsync(booking.Id);

            Assert.False(db.Bookings.Any(b => b.Id == booking.Id));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowKeyNotFound_WhenBookingDoesNotExist()
        {
            var db = CreateDbContext();
            var slotRepo = new Mock<IWorkshopSlotRepository>();
            var workshopRepo = new Mock<IWorkshopRepository>();
            var repo = new BookingRepository(db, slotRepo.Object, workshopRepo.Object);

            var invalidId = Guid.NewGuid();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteAsync(invalidId));
        }
    }
}
