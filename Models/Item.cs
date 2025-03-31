using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace network_inventory_system.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        public string? Photo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [MaxLength(50)]
        public string SerialNo { get; set; }

        [MaxLength(50)]
        public string PropertyNo { get; set; }

        [MaxLength(50)]
        public string ControlNo { get; set; }

        [MaxLength(50)]
        public string Model { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        public DateTime DateOfPurchase { get; set; }

        [MaxLength(50)]
        public string Condition { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(100)]
        public string AccountableOfficer { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(50)]
        public string Division { get; set; }
    }
}
