using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StorageV1.Data;
using StorageV1.Models;

namespace StorageV1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StorageV1Context _context;

        public ProductsController(StorageV1Context context)
        {
            _context = context;
        }

        // GET: Filter - Sökningen fungerrar inte som den ska.
        public async Task<IActionResult> Filter(string str, int? catory)
        {
            var model = string.IsNullOrWhiteSpace(str) ?
                _context.Product :
                _context.Product.Where(m => m.Name.StartsWith(str));

            model = catory is null ?
                model :
                model.Where(m => (int)m.Cat == catory);

            return View(nameof(Index), await model.ToListAsync());
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return View(await _context.Product.ToListAsync());
        }

        // GET: ProductViewModel (extra)
        public async Task<IActionResult> Summa() // Sorterar produkter efter kategori och summerar pris och antal.
        {
            var viewModels = new List<ProductViewModel>();
            var allProducts = await _context.Product.ToListAsync();

            int pris;
            int antal;
            var otrimmad = new List<string>();
            var kategorier = new List<string>();
            otrimmad.Clear();
            kategorier.Clear();

            if (allProducts.Count > 0)
            {
                foreach (var item in allProducts) // Listar kategorier.
                {
                    if (item.Category != "") otrimmad.Add(item.Category);
                }

                kategorier = otrimmad.Distinct().ToList(); // Tar bort kategorier som förekommer mer än en gång.

                foreach (var kategori in kategorier) // För varje kategori:
                {
                    pris = 0;
                    antal = 0;

                    foreach (var product in allProducts)
                    {
                        if (kategori == product.Category) // Hittas kategorin så summeras pris och antal med andra av samma.
                        {
                            pris += product.Price;
                            antal += product.Count;
                        }
                    }
                    var pvm = new ProductViewModel(); // Instansiering av PVM.
                    pvm.Category = kategori;
                    pvm.Sumprice = pris;
                    pvm.Sumcount = antal;
                    viewModels.Add(pvm);
                }
            }
            return View(viewModels);
        }

        // GET: ProductViewModel (extra) - Denna funktion är ofullständig. Ingen lösning i sikte...
        public async Task<IActionResult> Idsum(int catout) // Summerar pris och antal för en kategori.
        {
            var viewModels = new ProductViewModel();
            var allProducts = await _context.Product.ToListAsync();

            int pris;
            int antal;
            string katorier = "";
            var otrimmad = new List<string>();
            var kategorier = new List<string>();
            otrimmad.Clear();
            kategorier.Clear();

            if (catout == 0) katorier = "Motstånd";
            if (catout == 1) katorier = "Kondensator";
            if (catout == 2) katorier = "Spole";
            if (catout == 3) katorier = "Diod";
            if (catout == 4) katorier = "Transistor";
            if (catout == 5) katorier = "IC";

            if (allProducts.Count > 0)
            {
                foreach (var item in allProducts) // Listar kategorier.
                {
                    if (item.Category != "") otrimmad.Add(item.Category);
                }

                kategorier = otrimmad.Distinct().ToList(); // Tar bort kategorier som förekommer mer än en gång.

                foreach (var kategori in kategorier) // För varje kategori:
                {
                    pris = 0;
                    antal = 0;

                    foreach (var product in allProducts)
                    {
                        if (product.Category == katorier) // Hittas kategorin så summeras pris och antal med andra av samma.
                        {
                            pris += product.Price;
                            antal += product.Count;
                        }
                    }
                    viewModels.Category = katorier;
                    viewModels.Sumprice = pris;
                    viewModels.Sumcount = antal;
                }
            }
            return View(viewModels);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description,Cat")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description,Cat")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'StorageV1Context.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.Product.Any(e => e.Id == id);
        }
    }
}
