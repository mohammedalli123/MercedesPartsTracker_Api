using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MercedesPartsTracker.EntityFramework.Models
{
    public class Part
    {
        [Key]
        [Required]
        [MaxLength(50)]
        public string PartNumber { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int QuantityOnHand { get; set; }

        [Required]
        public string LocationCode { get; set; }

        public DateTime LastStockTake { get; set; }
    }
}
