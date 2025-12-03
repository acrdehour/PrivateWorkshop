using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Models.Services;
using PrivateWorkshop.ViewModels;

namespace PrivateWorkshop.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByClientIdAsync(string clientId);
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking entity);
        Task<Result> CreateBookingAsync(BookingCreateViewModel model, string userId);
        Task<int> CountBookingsAsync(Guid workshopId, DateOnly date, TimeSlot duration);

        Task UpdateAsync(Booking entity);
        Task DeleteAsync(Guid id);
    }
}
