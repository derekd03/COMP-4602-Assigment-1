using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment1.Data;
using Assignment1.Models;
using Assignment1.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Assignment1.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ReservationService _reservationService;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
            _reservationService = new ReservationService(context);
        }

        [Authorize(Roles = "Admin")]
        // GET: Reservation
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reservation.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [Authorize(Roles = "Admin, Member")]
        // GET: Reservation/Current/5
        public async Task<IActionResult> Current()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                Console.WriteLine("User ID not found");
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(r => r.ReserverId == userId);

            if (reservation == null)
            {
                Console.WriteLine("Reservation is null");
                return NotFound();
            }

            return View(reservation);
        }

        [Authorize(Roles = "Admin, Member")]
        // GET: Reservation/Confirmation
        public IActionResult Confirmation(string? bookCodeNumber)
        {
            // Get the last reservation id and increment it by 1
            var lastReservation = _context.Reservation.OrderByDescending(r => r.Id).FirstOrDefault();
            var id = lastReservation != null ? (Convert.ToInt32(lastReservation.Id) + 1).ToString() : "1";

            // Get the reserver id from the claims
            var reserverId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Check if the user has a current reservation
            var hasCurrentReservation = _context.Reservation.Any(r => r.ReserverId == reserverId);

            // Get the book code number from the query string
            if (reserverId == null || bookCodeNumber == null || hasCurrentReservation || !_reservationService.BookIsAvailable(bookCodeNumber))
            {
                return NotFound();
            }

            // Calculate dates
            var currentDate = DateTime.Now.Date;
            var estimatedDeliveryDate = currentDate.AddDays(1);
            var returnDate = estimatedDeliveryDate.AddMonths(1);

            // Create a new reservation
            var reservation = new Reservation
            {
                Id = id,
                ReserverId = reserverId,
                BookCodeNumber = bookCodeNumber,
                ReservationDate = currentDate,
                EstimatedDeliveryDate = estimatedDeliveryDate,
                ReturnDate = returnDate,
                Fees = 0,
                AdminComment = ""
            };

            return View(reservation);
        }

        // POST: Reservation/Confirmation
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> Confirmation([Bind("Id,ReserverId,BookCodeNumber,ReservationDate,EstimatedDeliveryDate,ReturnDate,Fees,AdminComment")] Reservation reservation)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                return View(reservation); // Return the view with the model if it's not valid
            }

            // Check if the book is available
            if (!_reservationService.BookIsAvailable(reservation.BookCodeNumber))
            {
                return NotFound("Book is not available"); // Return a not found result if the book is not available
            }

            // Decrement the quantity of the booked item
            _reservationService.DecrementBookQuantity(reservation.BookCodeNumber);

            // Add the reservation to the database
            _context.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        // POST: Reservation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,ReserverId,BookCodeNumber,ReservationDate,EstimatedDeliveryDate,ReturnDate,Fees,AdminComment")] Reservation reservation)
        {
            // Check if the model is valid and the book is available
            if (ModelState.IsValid && _reservationService.BookIsAvailable(reservation.BookCodeNumber))
            {
                _context.Add(reservation);

                // Decrement the quantity of the booked item
                _reservationService.DecrementBookQuantity(reservation.BookCodeNumber);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservation/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Reservation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ReserverId,BookCodeNumber,ReservationDate,EstimatedDeliveryDate,ReturnDate,Fees,AdminComment")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            return View(reservation);
        }

        // GET: Reservation/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservation/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
                _reservationService.IncrementBookQuantity(reservation.BookCodeNumber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Return(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservation/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(string id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
                _reservationService.IncrementBookQuantity(reservation.BookCodeNumber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool ReservationExists(string id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
