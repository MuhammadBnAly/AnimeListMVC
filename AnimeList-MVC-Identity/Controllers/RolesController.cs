using AnimeList_MVC_Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AnimeList_MVC_Identity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this._roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel model)
        {
            // check is there a value added
            if (!ModelState.IsValid)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                return View(nameof(Index), roles);
            }

            // check role when it is already exist
            //var roleExist = await _roleManager.RoleExistsAsync(model.Name);
            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "This Role is already exist!");
                var roles = await _roleManager.Roles.ToListAsync();
                //return RedirectToAction(nameof(Index), roles);
                return View("Index", roles);
            }
            await _roleManager.CreateAsync(new IdentityRole(model.Name.Trim()));
            return RedirectToAction(nameof(Index));
        }
    }
}
