using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace CRM.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var projects = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                .Where(p => p.Client.UserId == userId)
                .ToListAsync();
            return View(projects);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Client = new SelectList(
                await _context.Clients.Where(c => c.UserId == userId).ToListAsync(),
                "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            var userId = _userManager.GetUserId(User);
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == project.ClientId && c.UserId == userId);
            if(client == null)
            {
                ModelState.AddModelError("ClientId", "Invalid client.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = new SelectList(
                await _context.Clients.Where(c => c.UserId == userId).ToListAsync(),
                "Id", "Name", project.ClientId);
            return View(project);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id && p.Client.UserId == userId);
            if (project == null) return NotFound();

            return View(project);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == id && p.Client.UserId == userId);
            if (project == null) return NotFound();
            ViewBag.Clients = new SelectList(
                await _context.Clients.Where(c => c.UserId == userId).ToListAsync(),
                "Id", "Name", project.ClientId);
            return View(project);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var existingProject = await _context.Projects
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == id && p.Client.UserId == userId);

            if (existingProject == null) return NotFound();
            if (ModelState.IsValid)
            {
                existingProject.Name = project.Name;
                existingProject.Description = project.Description;
                existingProject.StartDate = project.StartDate;
                existingProject.EndDate = project.EndDate;
                existingProject.Status = project.Status;
                existingProject.ClientId = project.ClientId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = new SelectList(
                await _context.Clients.Where(c => c.UserId == userId).ToListAsync(),
                "Id", "Name", project.ClientId);
            return View(project);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == id && p.Client.UserId == userId);

            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
