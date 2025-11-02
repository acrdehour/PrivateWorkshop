using PrivateWorkshop.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PrivateWorkshop.ViewModels
{
    public class WorkshopViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Instructor { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int MaxSlot { get; set; } = 2;
        [Required]
        public TimeSlot Duration { get; set; }
    }
}
