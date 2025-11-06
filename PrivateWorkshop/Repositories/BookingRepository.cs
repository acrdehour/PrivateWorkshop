using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;

namespace PrivateWorkshop.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking entity)
        {
            await _context.Bookings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountBookingsAsync(Guid workshopId, DateOnly date, TimeSlot duration)
        {
            return await _context.Bookings
                .Where(b => b.WorkshopId == workshopId
                         && b.Date == date
                         && b.Duration == duration)
                .CountAsync();
        }
        public async Task<IEnumerable<Booking>> GetAllAsync(string sortBy)
        {
            var query = _context.Bookings
        .Include(b => b.Workshop)
        .Include(b => b.Client)
        .AsQueryable();

            query = sortBy.ToLower() switch
            {
                "date" => query.OrderByDescending(b => b.Date),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByClientIdAsync(string clientId, string sortBy)
        {
            var query = _context.Bookings
            .Where(b => b.ClientId == clientId)
            .Include(b => b.Workshop)
            .Include(b => b.Client)
            .AsQueryable();

            query = sortBy.ToLower() switch
            {
                "date" => query.OrderByDescending(b => b.Date),
                _ => query.OrderByDescending(b => b.CreatedAt)
            };

            return await query.ToListAsync();
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
