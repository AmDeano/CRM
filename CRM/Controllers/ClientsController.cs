using CRM.Data;
using CRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CRM.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var clients = await _context.Clients
                .Where(c => c.UserId == userId)
                .Include(c => c.Projects)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
            return View(clients);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                client.UserId = _userManager.GetUserId(User);
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = await _context.Clients
                .Include(c => c.Projects)
                .ThenInclude(p => p.Tasks)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null || client.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            return View(client);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client == null)
                return NotFound();
            return View(client);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();
            var userId = _userManager.GetUserId(User);
            var existingClient = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (existingClient == null) return NotFound();
            if (ModelState.IsValid)
            {
                existingClient.Name = client.Name;
                existingClient.EmailAddress = client.EmailAddress;
                existingClient.Phone = client.Phone;
                existingClient.Address = client.Address;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}