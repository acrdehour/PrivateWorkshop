using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;

namespace PrivateWorkshop.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByClientIdAsync(string clientId);
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking entity);
        Task<int> CountBookingsAsync(Guid workshopId, DateOnly date, TimeSlot duration);

        Task UpdateAsync(Booking entity);
        Task DeleteAsync(Guid id);
    }
}
