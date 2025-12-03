using Microsoft.EntityFrameworkCore;
using PrivateWorkshop.Data;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;

namespace PrivateWorkshop.Repositories
{
    public class WorkshopSlotRepository : IWorkshopSlotRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkshopSlotRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkshopSlot> GetOrCreateSlotAsync(Guid workshopId, DateOnly date, TimeSlot duration, int maxSlot)
        {
            var slot = await _context.WorkshopSlots
                .FirstOrDefaultAsync(ws =>
                    ws.WorkshopId == workshopId &&
                    ws.Date == date &&
                    ws.Duration == duration);

            if (slot != null) return slot;

            slot = new WorkshopSlot
            {
                Id = Guid.NewGuid(),
                WorkshopId = workshopId,
                Date = date,
                Duration = duration,
                BookedCount = 0,
                MaxSlot = maxSlot
            };

            _context.WorkshopSlots.Add(slot);
            await _context.SaveChangesAsync();

            return slot;
        }

        public async Task UpdateAsync(WorkshopSlot slot)
        {
            _context.WorkshopSlots.Update(slot);
            await _context.SaveChangesAsync();
        }
    }

}
