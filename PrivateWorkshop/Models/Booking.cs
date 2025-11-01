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
        public int ClientId { get; set; }

        // ✅ Navigation property
        [ForeignKey(nameof(ClientId))]
        public IdentityUser Client { get; set; }
        [Required]
        public int WorkshopId { get; set; }

        [ForeignKey(nameof(WorkshopId))]
        public Workshop Workshop { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeSlot TimeSlot { get; set; }
        [Required]
        public BookingStatus Status { get; set; }
        [Required]

        public DateTime CreatedAt { get; set; }
    }
}
