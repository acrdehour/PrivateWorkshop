using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Models.Services;
using PrivateWorkshop.ViewModels;

namespace PrivateWorkshop.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkshopSlotRepository _slotRepo;
        private readonly IWorkshopRepository _workshopRepo;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(ApplicationDbContext context, IWorkshopSlotRepository slotRepo, IWorkshopRepository workshopRepo,
    ILogger<BookingRepository> logger)
        {
            _context = context;
            _slotRepo = slotRepo;
            _workshopRepo = workshopRepo;
            _logger = logger;
        }

        public async Task AddAsync(Booking entity)
        {
            await _context.Bookings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Result> CreateBookingAsync(BookingCreateViewModel model, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var workshop = await _workshopRepo.GetByIdAsync(model.WorkshopId);
                if (workshop == null)
                    return Result.Fail("Workshop not found");

                var slot = await _slotRepo.GetOrCreateSlotAsync(workshop.Id, model.SelectedDate,
                                                                model.Duration, workshop.MaxSlot);

                if (slot.BookedCount >= slot.MaxSlot)
                    return Result.Fail("Time slot full");

                slot.BookedCount++;

                _context.WorkshopSlots.Update(slot);

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    WorkshopId = workshop.Id,
                    ClientId = userId,
                    Date = model.SelectedDate,
                    Duration = model.Duration,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await AddAsync(booking);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
    "User {UserId} booked Workshop {WorkshopId} on {Date} ({Slot})",
    userId,
    workshop.Id,
    model.SelectedDate.ToString("yyyy-MM-dd"),
    model.Duration);

                return Result.Ok();
                

            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Result.Fail("Concurrent booking occurred. Please try again.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return Result.Fail("Unexpected error occurred.");
            }
        }

        public async Task<int> CountBookingsAsync(Guid workshopId, DateOnly date, TimeSlot duration)
        {
            return await _context.Bookings
                .Where(b => b.WorkshopId == workshopId
                         && b.Date == date
                         && b.Duration == duration)
                .CountAsync();
        }
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Workshop)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByClientIdAsync(string clientId)
        {
            return await _context.Bookings
                .Where(b => b.ClientId == clientId)
                .Include(b => b.Client)
                .Include(b => b.Workshop)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.Workshop)   
                .Include(b => b.Client)    
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found.");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }




        public async Task UpdateAsync(Booking entity)
        {
            _context.Bookings.Update(entity);
            await _context.SaveChangesAsync();
        }

    }
}
