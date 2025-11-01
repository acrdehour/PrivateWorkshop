using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;

namespace PrivateWorkshop.Repositories
{
    public class WorkshopRepository : IRespository<Workshop>
    {
        private readonly ApplicationDbContext _context;

        public WorkshopRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Workshop entity)
        {
            await _context.Workshops.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var jobPosting = await _context.Workshops.FindAsync(id);

            if (jobPosting == null)
            {
                throw new KeyNotFoundException();
            }

            _context.Workshops.Remove(jobPosting);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Workshop>> GetAllAsync()
        {
            return await _context.Workshops.ToListAsync();
        }

        public async Task<Workshop> GetByIdAsync(int id)
        {
            var jobPosting = await _context.Workshops.FindAsync(id);

            if (jobPosting == null)
            {
                throw new KeyNotFoundException();
            }

            return jobPosting;
        }

        public async Task UpdateAsync(Workshop entity)
        {
            _context.Workshops.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
