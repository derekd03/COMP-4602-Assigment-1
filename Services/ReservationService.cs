using Assignment1.Data;

namespace Assignment1.Services
{
    public class ReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool BookIsAvailable(string bookCodeNumber)
        {
            var book = _context.Book.FirstOrDefault(b => b.CodeNumber == bookCodeNumber);
            return book != null && book.Quantity != 0;
        }

        public void DecrementBookQuantity(string bookCodeNumber)
        {
            var book = _context.Book.FirstOrDefault(b => b.CodeNumber == bookCodeNumber);
            if (book != null)
            {
                book.Quantity--;
            }
        }

        public void IncrementBookQuantity(string bookCodeNumber)
        {
            var book = _context.Book.FirstOrDefault(b => b.CodeNumber == bookCodeNumber);
            if (book != null)
            {
                book.Quantity++;
            }
        }
    }
}