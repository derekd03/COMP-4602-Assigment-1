using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1.Models
{
    public class Book
    {
        [Key]
        [Column("CodeNumber")]
        public required string CodeNumber { get; set; } // PK
        public required string Author { get; set; }
        public required string Title { get; set; }
        public required int YearPublished { get; set; }
        public required int Quantity { get; set; }

        public Book()
        {
        }

        public Book(string codeNumber, string author, string title, int yearPublished, int quantity)
        {
            CodeNumber = codeNumber;
            Author = author;
            Title = title;
            YearPublished = yearPublished;
            Quantity = quantity;
        }

        public static implicit operator Book(Task<Book?> v)
        {
            throw new NotImplementedException();
        }
    }
}
