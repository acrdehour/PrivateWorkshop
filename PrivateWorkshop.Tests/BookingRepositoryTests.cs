using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Repositories;

namespace PrivateWorkshop.Tests;

public class BookingRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public BookingRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("PrivateWorkshopDb")
            .Options;
    }
    private ApplicationDbContext CreateDbContext() => new ApplicationDbContext(_options);

    private IdentityUser CreateUser(string id, string email)
    {
        return new IdentityUser
        {
            Id = id,
            UserName = email,
            Email = email
        };
    }

    private Workshop CreateWorkshop(Guid id, string title)
    {
        return new Workshop
        {
            Title = title,
            Description = "Test Description",
            Instructor = "Sir Lewis Hamilton",
            Price = 500,
            MaxSlot = 3
        };
    }

    [Fact]
    public async Task AddAsync_ShouldAddBooking()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var workshop = CreateWorkshop(Guid.NewGuid(), "Test WS");
        var user = CreateUser("user1", "test@test.com");

        db.Workshops.Add(workshop);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshop.Id,
            ClientId = user.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await repo.AddAsync(booking);

        Assert.True(db.Bookings.Any(b => b.Id == booking.Id));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBooking_WithIncludes()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var workshop = CreateWorkshop(Guid.NewGuid(), "Test WS");
        var user = CreateUser("user1", "user@test.com");
        await db.AddAsync(workshop);
        await db.AddAsync(user);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshop.Id,
            ClientId = user.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Duration = TimeSlot.Afternoon,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await db.Bookings.AddAsync(booking);
        await db.SaveChangesAsync();

        var result = await repo.GetByIdAsync(booking.Id);

        Assert.NotNull(result);
        Assert.NotNull(result.Workshop);
        Assert.NotNull(result.Client);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var repo = new BookingRepository(CreateDbContext());

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var user = CreateUser("client1", "email@test.com");
        var workshop = CreateWorkshop(Guid.NewGuid(), "WS");
        await db.Users.AddAsync(user);
        await db.Workshops.AddAsync(workshop);

        await db.Bookings.AddRangeAsync(new[]
        {
        new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = user.Id,
            WorkshopId = workshop.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        },
        new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = user.Id,
            WorkshopId = workshop.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Duration = TimeSlot.Afternoon,
            Status = BookingStatus.Approved,
            CreatedAt = DateTime.UtcNow
        }
    });

        await db.SaveChangesAsync();

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByClientIdAsync_ShouldReturnBookingsOnlyForClient()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var workshop = CreateWorkshop(Guid.NewGuid(), "WS");

        var userA = CreateUser("u1", "a@test.com");
        var userB = CreateUser("u2", "b@test.com");

        await db.AddRangeAsync(workshop, userA, userB);

        await db.Bookings.AddRangeAsync(new[]
        {
        new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = userA.Id,
            WorkshopId = workshop.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        },
        new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = userB.Id,
            WorkshopId = workshop.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Duration = TimeSlot.Afternoon,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        }
    });

        await db.SaveChangesAsync();

        var result = await repo.GetByClientIdAsync(userA.Id);

        Assert.Single(result);
        Assert.Equal(userA.Id, result.First().ClientId);
    }

    [Fact]
    public async Task CountBookingsAsync_ShouldReturnCorrectCount()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var workshopId = Guid.NewGuid();
        var user = CreateUser("u1", "test@test.com");

        db.Users.Add(user);
        db.Workshops.Add(CreateWorkshop(workshopId, "WS"));

        var date = DateOnly.FromDateTime(DateTime.Today);

        await db.Bookings.AddRangeAsync(new[]
        {
        new Booking
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshopId,
            ClientId = user.Id,
            Date = date,
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        },
        new Booking
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshopId,
            ClientId = user.Id,
            Date = date,
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Approved,
            CreatedAt = DateTime.UtcNow
        }
    });

        await db.SaveChangesAsync();

        var count = await repo.CountBookingsAsync(workshopId, date, TimeSlot.Morning);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBooking()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var user = CreateUser("u1", "test@test.com");
        var workshop = CreateWorkshop(Guid.NewGuid(), "WS");

        db.Users.Add(user);
        db.Workshops.Add(workshop);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            WorkshopId = workshop.Id,
            ClientId = user.Id,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await db.Bookings.AddAsync(booking);
        await db.SaveChangesAsync();

        booking.Status = BookingStatus.Approved;

        await repo.UpdateAsync(booking);

        Assert.Equal(BookingStatus.Approved, db.Bookings.First().Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBooking()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ClientId = "user1",
            WorkshopId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            Duration = TimeSlot.Morning,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await db.Bookings.AddAsync(booking);
        await db.SaveChangesAsync();

        await repo.DeleteAsync(booking.Id);

        Assert.Empty(db.Bookings);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenNotFound()
    {
        var db = CreateDbContext();
        var repo = new BookingRepository(db);

        var invalidId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteAsync(invalidId));
    }

}
