using PrivateWorkshop.Models;

namespace PrivateWorkshop.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking entity);
        Task UpdateAsync(Booking entity);
        Task DeleteAsync(Guid id);
    }
}
