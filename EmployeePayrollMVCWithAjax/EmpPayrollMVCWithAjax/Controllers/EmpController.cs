using EmpPayrollMVCWithAjax.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EmpPayrollMVCWithAjax.Controllers
{

    public class EmpController : Controller
    {
        private readonly EmpContext _context;

        public EmpController(EmpContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetAllEmployee()
        {
            return View(await _context.EmployeeData.ToListAsync());
        }

        // GET: Transaction/AddOrEdit(Insert)
        // GET: Transaction/AddOrEdit/5(Update)
        public async Task<IActionResult> AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new EmpModel());
            else
            {
                var EmpModel = await _context.EmployeeData.FindAsync(id);
                if (EmpModel == null)
                {
                    return NotFound();
                }
                return View(EmpModel);
            }
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, [Bind("EmpID,EmpName,ProfileImg,Gender,Department,Salary,StartDate,Notes ") ] EmpModel employee)
        {
            if (ModelState.IsValid)
            {

                //Insert
                if (id == 0)
                {
                   
                    _context.Add(employee);
                    await _context.SaveChangesAsync();

                }
                //Update
                else
                {
                    try
                    {
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmpModelExists(employee.EmpID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.EmployeeData.ToList()) });
        }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", employee) });
        }


      

        private bool EmpModelExists(int id)
        {
            return _context.EmployeeData.Any(e => e.EmpID == id);
        }

        //for delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empModel = await _context.EmployeeData.FindAsync(id);
            _context.EmployeeData.Remove(empModel);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.EmployeeData.ToList()) });
        }
    }
}
