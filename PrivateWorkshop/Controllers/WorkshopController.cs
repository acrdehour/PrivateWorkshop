using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using PrivateWorkshop.Constants;
using PrivateWorkshop.Models;
using PrivateWorkshop.Repositories;
using PrivateWorkshop.ViewModels;

namespace PrivateWorkshop.Controllers
{
    public class WorkshopController : Controller
    {

        private readonly IRepository<Workshop> _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkshopController(IRepository<Workshop> respoitory, UserManager<IdentityUser> userManager)
        {
            _repository = respoitory;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var workshops = await _repository.GetAllAsync();
            return View(workshops);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var workshop = await _repository.GetByIdAsync(id);
            if (workshop == null)
            {
                return NotFound();
            }
            return View(workshop);
        }

        [HttpGet]
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

                await _repository.AddAsync(workshop);
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var workshop = await _repository.GetByIdAsync(id);

            if (workshop == null)
            {
                return NotFound();
            }

            if (!User.IsInRole(Roles.Admin))
            {
                return Forbid();
            }

            var vm = new WorkshopViewModel
            {
                Id = workshop.Id,
                Title = workshop.Title,
                Description = workshop.Description,
                Instructor = workshop.Instructor,
                Price = workshop.Price,
                MaxSlot = workshop.MaxSlot,
                Duration = workshop.Duration
            };

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Update(WorkshopViewModel WorkshopVm)
        {
            if (!ModelState.IsValid)
            {
                return View(WorkshopVm);
            }

            var workshop = await _repository.GetByIdAsync(WorkshopVm.Id);

            if (workshop == null)
            {
                return NotFound();
            }

            if (!User.IsInRole(Roles.Admin))
            {
                return Forbid();
            }

            workshop.Title = WorkshopVm.Title;
            workshop.Description = WorkshopVm.Description;
            workshop.Instructor = WorkshopVm.Instructor;
            workshop.Price = WorkshopVm.Price;
            workshop.MaxSlot = WorkshopVm.MaxSlot;

            await _repository.UpdateAsync(workshop);

            return RedirectToAction(nameof(Details), new { id = workshop.Id });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {

            var workshop = await _repository.GetByIdAsync(id);

            if (workshop == null)
            {
                return NotFound();
            }

            if (!User.IsInRole(Roles.Admin))
            {
                return Forbid();
            }

            await _repository.DeleteAsync(id);

            return Ok();
        }
    }
}
