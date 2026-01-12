using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? projectId)
        {
            var userId = _userManager.GetUserId(User);
            var query = _context.ProjectTasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Client)
                .Where(t => t.Project.Client.UserId == userId);

            if (projectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == projectId.Value);
                var project = await _context.Projects.FindAsync(projectId.Value);
                ViewBag.ProjectName = project?.Name;
            }

            var tasks = await query.OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .ToListAsync();

            ViewBag.ProjectId = projectId;
            return View(tasks);
        }

        public async Task<IActionResult> Create(int? projectId)
        {
            var userId = _userManager.GetUserId(User);
            var projects = await _context.Projects
                .Include(p => p.Client)
                .Where(p => p.Client.UserId == userId)
                .ToListAsync();

            ViewBag.Projects = new SelectList(projects, "Id", "Name", projectId);

            var task = new ProjectTask();
            if (projectId.HasValue)
            {
                task.ProjectId = projectId.Value;
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTask task)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.Id == task.ProjectId && p.Client.UserId == userId);

            if (project == null)
            {
                ModelState.AddModelError("ProjectId", "Invalid project selected.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            var projects = await _context.Projects
                .Include(p => p.Client)
                .Where(p => p.Client.UserId == userId)
                .ToListAsync();
            ViewBag.Projects = new SelectList(projects, "Id", "Name", task.ProjectId);

            return View(task);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Client)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Client.UserId == userId);

            if (task == null) return NotFound();

            var projects = await _context.Projects
                .Include(p => p.Client)
                .Where(p => p.Client.UserId == userId)
                .ToListAsync();
            ViewBag.Projects = new SelectList(projects, "Id", "Name", task.ProjectId);

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectTask task)
        {
            if (id != task.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var existingTask = await _context.ProjectTasks
                .Include(t => t.Project)
                .ThenInclude(p => p.Client)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Client.UserId == userId);

            if (existingTask == null) return NotFound();

            if (ModelState.IsValid)
            {
                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.Priority = task.Priority;
                existingTask.IsCompleted = task.IsCompleted;
                existingTask.ProjectId = task.ProjectId;

                if (task.IsCompleted && !existingTask.CompletedDate.HasValue)
                {
                    existingTask.CompletedDate = DateTime.Now;
                }
                else if (!task.IsCompleted)
                {
                    existingTask.CompletedDate = null;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
            }

            var projects = await _context.Projects
                .Include(p => p.Client)
                .Where(p => p.Client.UserId == userId)
                .ToListAsync();
            ViewBag.Projects = new SelectList(projects, "Id", "Name", task.ProjectId);

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Client)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Client.UserId == userId);

            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                task.CompletedDate = task.IsCompleted ? DateTime.Now : null;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { projectId = task?.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Client)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Client.UserId == userId);

            if (task != null)
            {
                var projectId = task.ProjectId;
                _context.ProjectTasks.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projectId });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}