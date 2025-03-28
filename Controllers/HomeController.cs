using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using network_inventory_system.Data;
using network_inventory_system.Models;
using System.Diagnostics;
using ClosedXML.Excel;
using System.IO;

namespace network_inventory_system.Controllers
{
    public class HomeController : Controller
    {
        private readonly ItemDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(ItemDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            string condition,
            string status,
            string accountableOfficer,
            string division,
            DateTime? fromDate,
            DateTime? toDate,
            int? pageNumber,
            int? pageSize) 
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["SerialNoSortParm"] = sortOrder == "SerialNo" ? "serialno_desc" : "SerialNo";
            ViewData["PropertyNoSortParm"] = sortOrder == "PropertyNo" ? "propertyno_desc" : "PropertyNo";
            ViewData["ControlNoSortParm"] = sortOrder == "ControlNo" ? "controlno_desc" : "ControlNo";
            ViewData["ModelSortParm"] = sortOrder == "Model" ? "model_desc" : "Model";
            ViewData["ConditionSortParm"] = sortOrder == "Condition" ? "condition_desc" : "Condition";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["OfficerSortParm"] = sortOrder == "Officer" ? "officer_desc" : "Officer";
            ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["DivisionSortParm"] = sortOrder == "Division" ? "division_desc" : "Division";

            ViewData["PageSizeOptions"] = new List<int> { 5, 10, 20, 50, 100 };

            int defaultPageSize = 10;
            int currentPageSize = pageSize ?? defaultPageSize;
            ViewData["CurrentPageSize"] = currentPageSize;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["ConditionFilter"] = condition;
            ViewData["StatusFilter"] = status;
            ViewData["OfficerFilter"] = accountableOfficer;
            ViewData["DivisionFilter"] = division;
            ViewData["FromDateFilter"] = fromDate;
            ViewData["ToDateFilter"] = toDate;

            ViewData["Conditions"] = await _context.Items.Select(i => i.Condition).Distinct().ToListAsync();
            ViewData["Statuses"] = await _context.Items.Select(i => i.Status).Distinct().ToListAsync();
            ViewData["Officers"] = await _context.Items.Select(i => i.AccountableOfficer).Distinct().ToListAsync();
            ViewData["Divisions"] = await _context.Items.Select(i => i.Division).Distinct().ToListAsync();

            var items = from i in _context.Items
                        select i;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.SerialNo.Contains(searchString)
                                       || s.PropertyNo.Contains(searchString)
                                       || s.ControlNo.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(condition))
            {
                items = items.Where(s => s.Condition == condition);
            }

            if (!String.IsNullOrEmpty(status))
            {
                items = items.Where(s => s.Status == status);
            }

            if (!String.IsNullOrEmpty(accountableOfficer))
            {
                items = items.Where(s => s.AccountableOfficer == accountableOfficer);
            }

            if (!String.IsNullOrEmpty(division))
            {
                items = items.Where(s => s.Division == division);
            }

            if (fromDate.HasValue)
            {
                items = items.Where(s => s.DateOfPurchase >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                items = items.Where(s => s.DateOfPurchase <= toDate.Value);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.Price);
                    break;
                case "Date":
                    items = items.OrderBy(s => s.DateOfPurchase);
                    break;
                case "date_desc":
                    items = items.OrderByDescending(s => s.DateOfPurchase);
                    break;
                case "SerialNo":
                    items = items.OrderBy(s => s.SerialNo);
                    break;
                case "serialno_desc":
                    items = items.OrderByDescending(s => s.SerialNo);
                    break;
                case "PropertyNo":
                    items = items.OrderBy(s => s.PropertyNo);
                    break;
                case "propertyno_desc":
                    items = items.OrderByDescending(s => s.PropertyNo);
                    break;
                case "ControlNo":
                    items = items.OrderBy(s => s.ControlNo);
                    break;
                case "controlno_desc":
                    items = items.OrderByDescending(s => s.ControlNo);
                    break;
                case "Model":
                    items = items.OrderBy(s => s.Model);
                    break;
                case "model_desc":
                    items = items.OrderByDescending(s => s.Model);
                    break;
                case "Condition":
                    items = items.OrderBy(s => s.Condition);
                    break;
                case "condition_desc":
                    items = items.OrderByDescending(s => s.Condition);
                    break;
                case "Status":
                    items = items.OrderBy(s => s.Status);
                    break;
                case "status_desc":
                    items = items.OrderByDescending(s => s.Status);
                    break;
                case "Officer":
                    items = items.OrderBy(s => s.AccountableOfficer);
                    break;
                case "officer_desc":
                    items = items.OrderByDescending(s => s.AccountableOfficer);
                    break;
                case "Category":
                    items = items.OrderBy(s => s.Category);
                    break;
                case "category_desc":
                    items = items.OrderByDescending(s => s.Category);
                    break;
                case "Division":
                    items = items.OrderBy(s => s.Division);
                    break;
                case "division_desc":
                    items = items.OrderByDescending(s => s.Division);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            return View(await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, currentPageSize));
        }

        public IActionResult Create()
        {
            return View(new Item { DateOfPurchase = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item, IFormFile? photoFile)
        {
            try
            {
                if (photoFile != null && photoFile.Length > 0 && ModelState.ContainsKey("Photo"))
                {
                    ModelState.Remove("Photo");
                }

                if (ModelState.IsValid)
                {
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "items");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(fileStream);
                        }

                        item.Photo = "/images/items/" + uniqueFileName;
                    }
                    else
                    {
                        item.Photo = "/images/items/placeholder.png";
                    }

                    if (item.DateOfPurchase == default)
                    {
                        item.DateOfPurchase = DateTime.Now;
                    }

                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var modelStateKey in ModelState.Keys)
                    {
                        var modelStateVal = ModelState[modelStateKey];
                        foreach (var error in modelStateVal.Errors)
                        {
                            Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }

            return View(item);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item, IFormFile? photoFile)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "items");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        if (!string.IsNullOrEmpty(item.Photo))
                        {
                            string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, item.Photo.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(fileStream);
                        }

                        item.Photo = "/images/items/" + uniqueFileName;
                    }

                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            return View(item);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                if (!string.IsNullOrEmpty(item.Photo))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, item.Photo.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return PartialView("_ItemDetails", item);
        }

        public async Task<IActionResult> ExportToExcel(
            string sortOrder,
            string searchString,
            string condition,
            string status,
            string accountableOfficer,
            string division,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var items = from i in _context.Items
                        select i;

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.SerialNo.Contains(searchString)
                                       || s.PropertyNo.Contains(searchString)
                                       || s.ControlNo.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(condition))
            {
                items = items.Where(s => s.Condition == condition);
            }

            if (!String.IsNullOrEmpty(status))
            {
                items = items.Where(s => s.Status == status);
            }

            if (!String.IsNullOrEmpty(accountableOfficer))
            {
                items = items.Where(s => s.AccountableOfficer == accountableOfficer);
            }

            if (!String.IsNullOrEmpty(division))
            {
                items = items.Where(s => s.Division == division);
            }

            if (fromDate.HasValue)
            {
                items = items.Where(s => s.DateOfPurchase >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                items = items.Where(s => s.DateOfPurchase <= toDate.Value);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.Price);
                    break;
                case "Date":
                    items = items.OrderBy(s => s.DateOfPurchase);
                    break;
                case "date_desc":
                    items = items.OrderByDescending(s => s.DateOfPurchase);
                    break;
                default:
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            var itemsList = await items.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Inventory");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Name";
                worksheet.Cell(currentRow, 3).Value = "Description";
                worksheet.Cell(currentRow, 4).Value = "Serial No";
                worksheet.Cell(currentRow, 5).Value = "Property No";
                worksheet.Cell(currentRow, 6).Value = "Control No";
                worksheet.Cell(currentRow, 7).Value = "Model";
                worksheet.Cell(currentRow, 8).Value = "Price";
                worksheet.Cell(currentRow, 9).Value = "Count";
                worksheet.Cell(currentRow, 10).Value = "Date of Purchase";
                worksheet.Cell(currentRow, 11).Value = "Condition";
                worksheet.Cell(currentRow, 12).Value = "Status";
                worksheet.Cell(currentRow, 13).Value = "Accountable Officer";
                worksheet.Cell(currentRow, 14).Value = "Category";
                worksheet.Cell(currentRow, 15).Value = "Division";

                var headerRange = worksheet.Range(1, 1, 1, 15);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var item in itemsList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Id;
                    worksheet.Cell(currentRow, 2).Value = item.Name;
                    worksheet.Cell(currentRow, 3).Value = item.Description;
                    worksheet.Cell(currentRow, 4).Value = item.SerialNo;
                    worksheet.Cell(currentRow, 5).Value = item.PropertyNo;
                    worksheet.Cell(currentRow, 6).Value = item.ControlNo;
                    worksheet.Cell(currentRow, 7).Value = item.Model;
                    worksheet.Cell(currentRow, 8).Value = item.Price;
                    worksheet.Cell(currentRow, 10).Value = item.DateOfPurchase.ToString("yyyy-MM-dd");
                    worksheet.Cell(currentRow, 11).Value = item.Condition;
                    worksheet.Cell(currentRow, 12).Value = item.Status;
                    worksheet.Cell(currentRow, 13).Value = item.AccountableOfficer;
                    worksheet.Cell(currentRow, 14).Value = item.Category;
                    worksheet.Cell(currentRow, 15).Value = item.Division;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventory_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                }
            }
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}