using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPSMDB.Models
{
    public class AppUser : IdentityUser
    {
        [ForeignKey("Person")]
        public int? PersonId { get; set; }
        public Person person;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; }
    }
}