using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrivateWorkshop.Constants;
using PrivateWorkshop.Models;
using PrivateWorkshop.Repositories;
using PrivateWorkshop.ViewModels;

namespace PrivateWorkshop.Controllers
{
    public class WorkshopController : Controller
    {

        private readonly IRespository<Workshop> _respository;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkshopController(IRespository<Workshop> respository, UserManager<IdentityUser> userManager)
        {
            _respository = respository;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var workshops = await _respository.GetAllAsync();
            return View(workshops);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkshopViewModel WorkshopVm)
        {

            if (ModelState.IsValid)
            {
                Workshop workshop = new Workshop
                {
                    Title = WorkshopVm.Title,
                    Description = WorkshopVm.Description,
                    Instructor = WorkshopVm.Instructor,
                    Price = WorkshopVm.Price,
                    MaxSlot = WorkshopVm.MaxSlot,
                    Duration = WorkshopVm.Duration
                };

                await _respository.AddAsync(workshop);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var jobPosting = await _respository.GetByIdAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            if (!User.IsInRole(Roles.Admin))
            {
                return Forbid();
            }

            await _respository.DeleteAsync(id);

            return Ok();
        }
    }
}
