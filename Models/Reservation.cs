using System;
using Microsoft.AspNetCore.Identity;

namespace Assignment1.Models
{
    public class Reservation
    {
        public required string Id { get; set; }
        public required string ReserverId { get; set; }
        public required string BookCodeNumber { get; set; }
        public required DateTime ReservationDate { get; set; }
        public required DateTime EstimatedDeliveryDate { get; set; }
        public required DateTime ReturnDate { get; set; }
        public decimal Fees { get; set; }
        public string? AdminComment { get; set; }
    }
}
