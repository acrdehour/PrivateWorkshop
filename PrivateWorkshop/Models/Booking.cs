using Microsoft.AspNetCore.Identity;
using PrivateWorkshop.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateWorkshop.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        // ✅ Foreign key field
        [Required] 
        public string ClientId { get; set; }

        // ✅ Navigation property
        [ForeignKey(nameof(ClientId))]
        public IdentityUser Client { get; set; }
        [Required]
        public Guid WorkshopId { get; set; }

        [ForeignKey(nameof(WorkshopId))]
        public Workshop Workshop { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeSlot Duration { get; set; }
        [Required]
        public BookingStatus Status { get; set; }
        [Required]

        public DateTime CreatedAt { get; set; }
    }
}
