using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.DATA.EF.Models;
using Microsoft.AspNetCore.Authorization;//added to lock down controller

namespace StoreFront.UI.MVC.Controllers
{

    //When [Authorize] is used at the class level it affects ALL actions inside the class
    //We MUST be an Administrator to access Cats/Index, Cats/Details, Cats/Create, Cats/Edit, and Cats/Delete

    //Alternatively we can lock down individual actions inside the class by applying the [Authorize] attribute
    //just above the Action itself (See ProductsController.cs)
    // [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly StoreFrontContext _context;

        public CategoriesController(StoreFrontContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'StoreFront.Categories'  is null.");
        }
        /*
         * AJAX STEPS 
         * - Steps are listed throughout the files relating to Category
         * - to locate, ctrl + f (entire solution) -> 'AJAX Step #'
         */


        //AJAX Step 9 - code the AjaxDelete Action
        [AcceptVerbs("POST")]
        public JsonResult AjaxDelete(int id)
        {
            Category category = _context.Categories.Find(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();

            string confirmMessage = $"Deleted the category {category.CategoryName} from the database!";
            return Json(new { id = id, message = confirmMessage });
        }

        //AJAX Step 15 - Code the CategoryDetails Action
        public PartialViewResult CategoryDetails(int id)
        {
            var category = _context.Categories.Find(id);
            return PartialView(category);
            //AJAX Step 16 - create the partial view (right click Find and add Razor View)
            // - template: Details
            // - model:Category
            // - check 'Create as partial view'
            // - minor view tweaks
        }

        //AJAX Step 20 - code the AjaxCreate Action -- return the category as JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AjaxCreate(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return Json(category);

            //AJAX Step 21 - Create the partial view (add View, Razor View)
            // - Name: CategoryCreate
            // - template: Create
            // - model: Category
            // - check 'Create as partial view'
        }


        //AJAX Step 24 - code the action that returns the partial view for edit
        [HttpGet]
        public PartialViewResult CategoryEdit(int id)
        {
            Category category = _context.Categories.Find(id);
            return PartialView(category);
            //AJAX Step 25 - create the partial view 
            // - name: CategoryEdit
            // - template: Edit
            // - model: Category
            // - check 'Create as partial view'
            // - copious HTML modification
        }

        //AJAX Step 27 - code the POST AjaxEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AjaxEdit(Category category)
        {
            _context.Update(category);
            _context.SaveChanges();
            return Json(category);
        }


        //AJAX Step 1 - put the original scaffolded actions in a region and comment out (EXCEPT CategoryExists())
        #region Non-Ajax Original Scaffolded EF Actions



        // GET: Categories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Categories == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .FirstOrDefaultAsync(m => m.CategoryId == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        // GET: Categories/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryDescription")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(category);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        // GET: Categories/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Categories == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,CategoryDescription")] Category category)
        //{
        //    if (id != category.CategoryId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(category);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CategoryExists(category.CategoryId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        // GET: Categories/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Categories == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .FirstOrDefaultAsync(m => m.CategoryId == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        // POST: Categories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Categories == null)
        //    {
        //        return Problem("Entity set 'GadgetStore03222022Context.Categories'  is null.");
        //    }
        //    var category = await _context.Categories.FindAsync(id);
        //    if (category != null)
        //    {
        //        _context.Categories.Remove(category);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        #endregion

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
