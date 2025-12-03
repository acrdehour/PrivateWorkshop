using PrivateWorkshop.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PrivateWorkshop.Models
{
    public class WorkshopSlot
    {
        public Guid Id { get; set; }

        public Guid WorkshopId { get; set; }
        public Workshop Workshop { get; set; }   

        public DateOnly Date { get; set; }
        public TimeSlot Duration { get; set; }

        public int BookedCount { get; set; }
        public int MaxSlot { get; set; } 

        [Timestamp]
        public byte[] RowVersion { get; set; }   // Concurrency Token
    }

}
