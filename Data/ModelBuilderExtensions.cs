using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Assignment1.Models;

namespace Assignment1.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            var pwd = "P@$$w0rd";
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            // Seed Roles
            var adminRole = new IdentityRole("Admin");
            adminRole.NormalizedName = adminRole.Name!.ToUpper();
            var memberRole = new IdentityRole("Member");
            memberRole.NormalizedName = memberRole.Name!.ToUpper();
            List<IdentityRole> roles = new List<IdentityRole>() {
                adminRole,
                memberRole
                };
            builder.Entity<IdentityRole>().HasData(roles);

            // Seed Users
            var adminUser = new ApplicationUser
            {
                UserName = "aa@aa.aa",
                Email = "aa@aa.aa",
                EmailConfirmed = true,
                FirstName = "Adam",
                LastName = "Atkins",
                City = "Victoria",
                Province = "BC",
                Address = "1234 Elm St",
                PostalCode = "V8V3A4",
                PhoneNumber = "250-123-4567",
                ReservationId = "",
            };
            adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
            adminUser.NormalizedEmail = adminUser.Email.ToUpper();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, pwd);

            var memberUser = new ApplicationUser
            {
                UserName = "mm@mm.mm",
                Email = "mm@mm.mm",
                EmailConfirmed = true,
                FirstName = "Mike",
                LastName = "Moore",
                City = "Victoria",
                Province = "BC",
                Address = "5678 Oak St",
                PostalCode = "V8V3A4",
                PhoneNumber = "250-123-4567",
                ReservationId = "",
            };
            memberUser.NormalizedUserName = memberUser.UserName.ToUpper();
            memberUser.NormalizedEmail = memberUser.Email.ToUpper();
            memberUser.PasswordHash = passwordHasher.HashPassword(memberUser, pwd);

            List<ApplicationUser> users = new List<ApplicationUser>() {
                adminUser,
                memberUser,
            };

            builder.Entity<ApplicationUser>().HasData(users);

            // Seed UserRoles
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            userRoles.Add(new IdentityUserRole<string>
            {
                UserId = users[0].Id,
                RoleId = roles.First(q => q.Name == "Admin").Id
            });

            userRoles.Add(new IdentityUserRole<string>
            {
                UserId = users[1].Id,
                RoleId = roles.First(q => q.Name == "Member").Id
            });


            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            // Seed Reservations
            List<Reservation> reservations = new List<Reservation>() {
                new Reservation {
                    Id = "1",
                    ReserverId = users[0].Id,
                    BookCodeNumber = "1",
                    ReservationDate = DateTime.Now,
                    EstimatedDeliveryDate = DateTime.Now.AddDays(1),
                    ReturnDate = DateTime.Now.AddMonths(1),
                    Fees = 0,
                    AdminComment = ""
                },
                // Sample late reservation
                new Reservation {
                    Id = "2",
                    ReserverId = users[1].Id,
                    BookCodeNumber = "2",
                    ReservationDate = DateTime.Now.AddMonths(-2),
                    EstimatedDeliveryDate = DateTime.Now.AddMonths(-2).AddDays(1),
                    ReturnDate = DateTime.Now.AddMonths(-1),
                    Fees = 0,
                    AdminComment = ""
                }
            };

            // Seed Books
            List<Book> books = new List<Book>() {
                new Book {
                    CodeNumber = "1",
                    Author = "Andrew Chevallier",
                    Title = "Encyclopedia of Herbal Medicine: 550 Herbs and Remedies for Common Ailments",
                    YearPublished = 2016,
                    Quantity = 0
                },
                new Book {
                    CodeNumber = "2",
                    Author = "Michael T. Murray M.D. and Joseph Pizzorno",
                    Title = "The Encyclopedia of Natural Medicine Third Edition",
                    YearPublished = 2012,
                    Quantity = 2
                },
                new Book {
                    CodeNumber = "3",
                    Author = "Thomas Easley and Steven Horne",
                    Title = "The Modern Herbal Dispensatory: A Medicine-Making Guide",
                    YearPublished = 2016,
                    Quantity = 1
                },
                new Book {
                    CodeNumber = "4",
                    Author = "Cat Ellis",
                    Title = "Prepper's Natural Medicine: Life-Saving Herbs, Essential Oils and Natural Remedies for When There is No Doctor",
                    YearPublished = 2015,
                    Quantity = 2
                },
                new Book {
                    CodeNumber = "5",
                    Author = "Rosemary Gladstar",
                    Title = "Rosemary Gladstar's Medicinal Herbs: A Beginner's Guide: 33 Healing Herbs to Know, Grow, and Use",
                    YearPublished = 2012,
                    Quantity = 1
                }

            };

            builder.Entity<Reservation>().HasData(reservations);
            builder.Entity<Book>().HasData(books);
        }
    }

}