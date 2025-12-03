using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;

namespace PrivateWorkshop.Repositories
{
    public interface IWorkshopSlotRepository
    {
        Task<WorkshopSlot> GetOrCreateSlotAsync(Guid workshopId, DateOnly date, TimeSlot duration, int maxSlot);
        Task UpdateAsync(WorkshopSlot slot);
    }

}
