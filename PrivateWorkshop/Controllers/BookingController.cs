using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Repositories;
using PrivateWorkshop.ViewModels;

namespace PrivateWorkshop.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IWorkshopRepository _workshopRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingController(
            IBookingRepository bookingRepository,
            IWorkshopRepository workshopRepository,
            UserManager<IdentityUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _workshopRepository = workshopRepository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please select both date and time slot.";
                return RedirectToAction("Details", "Workshop", new { id = model.WorkshopId });
            }

            var workshop = await _workshopRepository.GetByIdAsync(model.WorkshopId);
            if (workshop == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                WorkshopId = model.WorkshopId,
                ClientId = userId,
                Duration = model.Duration,
                Date = model.SelectedDate,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);

            TempData["Success"] = "Booking created successfully!";
            return RedirectToAction("MyBookings");
        }
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(Guid workshopId, DateOnly date, TimeSlot duration)
        {
            var workshop = await _workshopRepository.GetByIdAsync(workshopId);
            if (workshop == null) return NotFound();

            var bookedCount = await _bookingRepository.CountBookingsAsync(workshopId, date, duration);

            var remaining = workshop.MaxSlot - bookedCount;

            return Json(new { remaining });
        }

    }
}
