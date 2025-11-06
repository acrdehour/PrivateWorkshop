using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrivateWorkshop.Constants;
using PrivateWorkshop.Models;
using PrivateWorkshop.Models.Enums;
using PrivateWorkshop.Repositories;
using PrivateWorkshop.ViewModels;
using System.Globalization;

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
                Status = BookingStatus.Pending,
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

        public async Task<IActionResult> MyBookings(string sortBy = "created")
        {
            IEnumerable<Booking> bookings;

            if (User.IsInRole(Roles.Admin))
            {
                bookings = await _bookingRepository.GetAllAsync(sortBy);
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                bookings = await _bookingRepository.GetByClientIdAsync(userId, sortBy);
            }

            ViewBag.SortBy = sortBy.ToLower(); // ส่งค่าให้ dropdown จำ state

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.Status = BookingStatus.Approved;
            await _bookingRepository.UpdateAsync(booking);

            TempData["Success"] = "Booking approved successfully.";
            return RedirectToAction("MyBookings");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.Status = BookingStatus.Rejected;
            await _bookingRepository.UpdateAsync(booking);

            TempData["Warning"] = "Booking rejected.";
            return RedirectToAction("MyBookings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            // ✅ ป้องกัน Client ยกเลิกของคนอื่น
            if (booking.ClientId != userId && !User.IsInRole(Roles.Admin))
                return Forbid();

            // ✅ อนุญาตให้ยกเลิกเฉพาะ Pending
            if (booking.Status != BookingStatus.Pending)
            {
                TempData["Warning"] = "This booking cannot be cancelled because it is not pending.";
                return RedirectToAction("MyBookings");
            }

            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.UpdateAsync(booking);

            TempData["Success"] = "Your booking has been cancelled.";
            return RedirectToAction("MyBookings");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            // ✅ Admin Only
            if (!User.IsInRole(Roles.Admin))
                return Forbid();

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound();

            await _bookingRepository.DeleteAsync(id);

            TempData["Success"] = "Booking deleted successfully.";
            return RedirectToAction("MyBookings");
        }


    }
}
