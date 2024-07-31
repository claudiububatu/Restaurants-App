using IPSMDB.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPSMDB.AbstractClasses
{
    public abstract class Review
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        [ForeignKey("FoodPlace")]
        public int FoodPlaceId { get; set; }
        public FoodPlace FoodPlace { get; set; }
        [ForeignKey("Person")]
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}