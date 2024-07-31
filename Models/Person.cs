using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IPSMDB.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int? PhoneNumber { get; set; }
        public ICollection<FoodPlace> FoodPlaces { get; set; }
        public ICollection<CustomerReview> ReviewsWritten { get; set; }
        public ICollection<ServiceReview> ReviewsReceived { get; set; }
        public ICollection<PersonFriendship> PersonFriendships { get; set; }
    }
}
