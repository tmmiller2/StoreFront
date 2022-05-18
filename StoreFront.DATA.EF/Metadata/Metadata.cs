using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; //Added for access to required, display, etc.
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreFront.DATA.EF.Metadata
{
    //internal class Metadata
    //{
    //}
    #region Category
    public class CategoryMetadata
    {
        //PK
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "* Category name is required")]
        [StringLength(200)]
        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;

        [StringLength(500)]
        public string? CategoryDescription { get; set; }
    }
    #endregion

    #region Color
    public class ColorMetadata
    {
        //PK
        public int PaintedId { get; set; }

        [StringLength(50)]
        [Display(Name = "Color")]
        public string ColorName { get; set; } = null!;
    }
    #endregion

    #region Order
    public class OrderMetadata
    {
        //PK
        public int OrderId { get; set; }

        //FK
        public string UserId { get; set; } = null!;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0,d}")]//MM/dd/yyyy
        [Display(Name = "Order Date")]
        [Required]
        public DateTime OrderDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Ship To")]
        [Required]
        public string ShipToName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "City")]
        [Required]
        public string ShipCity { get; set; } = null!;

        [StringLength(2)]
        [Display(Name = "State")]
        public string? ShipState { get; set; }

        [StringLength(5)]
        [Display(Name = "Zip")]
        [Required]
        public string ShipZip { get; set; } = null!;
    }
    #endregion

    #region Product
    public class ProductMetadata
    {
        //PK
        public int ProductId { get; set; }

        [StringLength(200)]
        [Display(Name = "Product")]
        [Required]
        public string ProductName { get; set; } = null!;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        [Display(Name = "Price")]
        [Required]
        public decimal ProductPrice { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? ProductDescription { get; set; }

        //FK
        public int CategoryId { get; set; }

        //FK
        public int StatusId { get; set; }

        [StringLength(200)]
        [Display(Name = "Certified")]
        [Required]
        public string ProductCertified { get; set; } = null!;

        [StringLength(200)]
        [Display(Name = "Quality")]
        [Required]
        public string ProductQuality { get; set; } = null!;

        [StringLength (50)]
        [Display(Name = "Image")]
        public string? ProductImage { get; set; }
    }
    #endregion

    #region StockStatus
    public class StockStatusMetadata
    {
        //PK
        public int StatusId { get; set; }

        [StringLength(50)]
        [Display(Name = "Status")]
        public string StatusName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Description")]
        public string? StatusDescription { get; set; }
    }
    #endregion

    #region MyRegion
    public class UserDetailMetadata
    {
        //PK
        public string UserId { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; } = null!;

        [StringLength(150)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(50)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(2)]
        [Display(Name = "State")]
        public string? State { get; set; }

        [StringLength(5)]
        [Display(Name = "Zip")]
        public string? Zip { get; set; }

        [StringLength(24)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
    }
    #endregion

}
