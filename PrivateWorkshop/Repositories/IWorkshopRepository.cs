using PrivateWorkshop.Models;

namespace PrivateWorkshop.Repositories
{
    public interface IWorkshopRepository
    {
        Task<IEnumerable<Workshop>> GetAllAsync();
        Task<Workshop?> GetByIdAsync(Guid id);
        Task AddAsync(Workshop entity);
        Task UpdateAsync(Workshop entity);
        Task DeleteAsync(Guid id);
    }
}

