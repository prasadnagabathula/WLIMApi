using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WLFSystem.Models
{
    [Table("WarehouseItems")]
    public class WareHouseItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Category { get; set; }
        public string? FilePath { get; set; }
        public string? WarehouseLocation { get; set; }
        public string? Status { get; set; }
        public string? Tags { get; set; }
        public string? ItemDescription { get; set; }
        public string? CreatedBy { get; set; }
       public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Comments { get; set; }

    }

    public class WareHouseItemViewModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Category { get; set; }
        public string? Tags { get; set;}
        public string? ItemDescription { get; set; }
        public string? WarehouseLocation { get; set; }
        public string? Comments { get; set; }


    }
}
