using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
            Painteds = new HashSet<Color>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public int CategoryId { get; set; }
        public int StatusId { get; set; }
        public string ProductCertified { get; set; } = null!;
        public string ProductQuality { get; set; } = null!;
        public string? ProductImage { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual StockStatus Status { get; set; } = null!;
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public virtual ICollection<Color> Painteds { get; set; }
    }
}
