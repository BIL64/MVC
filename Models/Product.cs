using System.ComponentModel.DataAnnotations;

namespace StorageV1.Models
{
    public class Product
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public int Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime Orderdate { get; set; }

        [StringLength(50), Required]
        public string Category { get; set; } = string.Empty;

        [StringLength(50)]
        public string Shelf { get; set; } = string.Empty;

        public int Count { get; set; }

        [StringLength(50)]
        public string Description { get; set; } = string.Empty;

        public Cat Cat { get; set; }

        public string Printsum(string sumcat, int sumprice, int sumcount)
        {
            return $"{sumcat}: Price: {sumprice} Count: {sumcount}\n";
        }
    }
}
