using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Book.Data;
using Book.Models;
using static NuGet.Packaging.PackagingConstants;
using Microsoft.Data.SqlClient;

namespace Book.Controllers
{
    public class OrdersController : Controller
    {
        private readonly BookContext _context;

        public OrdersController(BookContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
              return _context.Orders != null ? 
                          View(await _context.Orders.ToListAsync()) :
                          Problem("Entity set 'BookContext.Orders'  is null.");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create(int? id)
        {
            var book = await _context.book.FindAsync(id);

            return View(book);
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int bookId,int quantity)
        {

            Orders order = new Orders();
            order.bookId = bookId;
            order.quantity = quantity;

            order.userid = Convert.ToInt32(HttpContext.Session.GetString("userid")); ;
            order.orderdate = DateTime.Today;
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\USERS\\OLEGB\\ONEDRIVE\\ДОКУМЕНТИ\\MYNEWDB3.MDF;Integrated Security=True;Connect Timeout=30");
            string sql;
            sql = "UPDATE book  SET bookquantity  = bookquantity   - '" + order.quantity + "'  where (id ='" + order.bookId + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();



            _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(myorders));
        
        }

        public async Task<IActionResult> myorders()
        {
           int id = Convert.ToInt32(HttpContext.Session.GetString("userid")); ;

            var orItems = await _context.Orders.FromSqlRaw("select *  from orders where  userid = '" + id + "'  ").ToListAsync();
            return View(orItems);

        }

        public async Task<IActionResult> customerOrders(int? id)
        {


            var orItems = await _context.Orders.FromSqlRaw("select *  from orders where  userid = '" + id + "'  ").ToListAsync();
            return View(orItems);

        }



        public async Task<IActionResult> customerreport()
        {
            var orItems = await _context.report.FromSqlRaw("select usersaccounts.id as Id, name as customername, sum (quantity * Price)  as total from book, orders,usersaccounts  where usersaccounts.id = orders.userid  and bookid= book.Id group by name,usersaccounts.id ").ToListAsync();
            return View(orItems);

        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("bookId,quantity")] Orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.Id))
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
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'BookContext.Orders'  is null.");
            }
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
