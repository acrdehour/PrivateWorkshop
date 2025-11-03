using Microsoft.AspNetCore.Identity;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateWorkshop.ViewModels
{
    public class BookingCreateViewModel
    {
        [Required]
        public Guid WorkshopId { get; set; }

        [Required]
        public TimeSlot Duration { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly SelectedDate { get; set; }
    }
}
