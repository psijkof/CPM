using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConstructionManagement.Data;
using ConstructionManagement.Models;

namespace ConstructionManagement.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly ConstructionContext _context;

        public DocumentsController(ConstructionContext context)
        {
            _context = context;
        }

        // GET: Documents
        public async Task<IActionResult> Index()
        {
            //eager load related projects
            var documents = _context.Documents.Include(d => d.Project);
            return View(await documents.ToListAsync());
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Project)
                .SingleOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            PrepareProjectDropDownList();
 //           ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "Project.Name");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocumentId,Name,Description,Type,Status,StartDate,EndDate,WarrantyTillDate,ProjectId")] Document document)
        {
            if (ModelState.IsValid)
            {
                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            PrepareProjectDropDownList(document.ProjectId);

//            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", document.ProjectId);

            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.SingleOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }
            PrepareProjectDropDownList(document.ProjectId);
            return View(document);
        }

        private void PrepareProjectDropDownList(object selectedProject = null)
        {
            var projectsQuery = from p in _context.Projects
                                select p;

            ViewBag.ProjectId = new SelectList(projectsQuery.AsNoTracking(), "ProjectId", "Name", selectedProject);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,Name,Description,Type,Status,StartDate,EndDate,WarrantyTillDate,ProjectId")] Document document)
        {
            if (id != document.DocumentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(document);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.DocumentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            PrepareProjectDropDownList(document.ProjectId);

            //ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", document.ProjectId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Project)
                .SingleOrDefaultAsync(m => m.DocumentId == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _context.Documents.SingleOrDefaultAsync(m => m.DocumentId == id);
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.DocumentId == id);
        }
    }
}
