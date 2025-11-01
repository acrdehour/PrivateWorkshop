using Microsoft.AspNetCore.Identity;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateWorkshop.ViewModels
{
    public class BookingViewModel
    {
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
