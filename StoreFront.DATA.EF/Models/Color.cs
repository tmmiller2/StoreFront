using System;
using System.Collections.Generic;

namespace StoreFront.DATA.EF.Models
{
    public partial class Color
    {
        public Color()
        {
            Products = new HashSet<Product>();
        }

        public int PaintedId { get; set; }
        public string ColorName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
