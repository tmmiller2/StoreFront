using Microsoft.AspNetCore.Mvc;
using StoreFront.DATA.EF.Models;//added for access to the context
using Microsoft.AspNetCore.Identity;//added to access userManager
using StoreFront.UI.MVC.Models;//added to access CartItemViewModel
using Newtonsoft.Json;//added to manage shopping cart

namespace StoreFront.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        #region Steps to Implement Session Based Shopping Cart
        /*
            1) Register Session in program.cs ( builder.Services.AddSession... && app.UseSession() )
            2) Create the CartItemViewModel class in [ProjName].UI.MVC/Models folder
            3) Add the 'Add to Cart' button in the Index and/or Details view of your Products
            4) Create the ShoppingCartController (empty controller -> named ShoppingCartController)
                - Add using statemens 
                    - using GadgetStore.DATA.EF.Models;
                    - using Microsoft.AspNetCore.Identity;
                    - using GadgetStore.UI.MVC.Models;
                    - using Newtonsoft.Json;
                - Add props for the GadgetStoreContext && User Manager
                - Create a constructor for the controller - assign values to context && usermanager
                - Code the AddToCart() action ( add using statement before doing this -> [ProjName].DATA.EF.Models )
         */
        #endregion

        //properties
        private readonly StoreFrontContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //ctor 
        public ShoppingCartController(StoreFrontContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //Retrieve the content from the Session shopping cart (styored as JSON) and convert those to C# via Newtonsoft.Json.
            //After converting to C#, we can pass the collection of cart contents back to the strongly-typed view to display.

            //retrieve cart contents
            var sessionCart = HttpContext.Session.GetString("cart");

            //create the shell for the local (C# version) of the cart
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //check to see if the session cart was null
            if (sessionCart == null || sessionCart.Count() == 0)
            {
                //user either hasn't put anything in the cart, or they have removed all items
                //set shoppingCart to an empty object
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;
                //deserialize the cart contents from JSON to C#
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            //return the shopping cart collection back to the view
            return View(shoppingCart);
        }

        public IActionResult AddToCart(int id)
        {
            //Empty shell for the LOCAL shopping cart variable
            //int (key) -> Product ID
            //CartItemViewModel (value) -> Product & Qty
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            #region Session Notes
            /*
             * Session is a tool available on the server-side that can store information for a user while they are actively using your site.
             * Typically the session lasts for 20 minutes (this can be adjusted in Program.cs).
             * Once the 20 minutes is up, the session variable is disposed
             * 
             * Values that we can store in session are limited to: string, int
             * - Because of this, we have to get creative when trying to store complex objects (like CartItemViewModel).
             * To keep the info seperated into properties, we will convert the C# object into JSON string.
             */
            #endregion

            var sessionCart = HttpContext.Session.GetString("cart");

            //Check to see if the session object exists
            //If not, instantiate the new local collection
            if (String.IsNullOrEmpty(sessionCart))
            {
                //if the session didn't exist yet, instantiate the collectin as a new object
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                //Cart already exists - transfer the existing cart items from session into our local shoppingCart
                //DeserializedObject() is a method that converts JSON to C# - we MUST tell this method which C# class to 
                //convert the JSON into (here - Dictionary<int, IVM>)
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            // Add newly selected products to the cart
            // Retrieve product from the DB
            Product product = _context.Products.Find(id);

            // Instantiate the object so we can add to the cart
            CartItemViewModel civm = new CartItemViewModel(1, product);//add 1 of the selected products to the cart

            // If the product was already in the cart, increase the quantity by 1 
            //Otherwise, add the new item to the cart
            if (shoppingCart.ContainsKey(product.ProductId))
            {
                //update qty
                shoppingCart[product.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(product.ProductId, civm);
            }

            //update the session version of the cart
            //Take the local copy, and serialize it as JSON
            //Then assign that value to our session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            //retrieve the cart from session
            var sessionCart = HttpContext.Session.GetString("cart");

            //convert JSON cart to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //remove cart item
            shoppingCart.Remove(id);

            //if there are no remaining items in the cart, remove it from session
            if (shoppingCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            //otherwise, update the session variable with our local cart contents
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }

            return RedirectToAction("Index");

        }

        public IActionResult UpdateCart(int productId, int qty)
        {
            //retrieve the cart
            var sessionCart = HttpContext.Session.GetString("cart");
            //Deserialize from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //update the qty for our specific dictionary key
            shoppingCart[productId].Qty = qty;

            //update session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SubmitOrder()
        {
            //Retrieve current user's Id
            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;

            //Retrieve the UserDetails record
            UserDetail ud = _context.UserDetails.Find(userId);

            //Create the Order object and assign values
            Order o = new Order()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                ShipCity = ud.City,
                ShipState = ud.State,
                ShipToName = ud.FullName,
                ShipZip = ud.Zip,
            };

            //Add the Order to _context
            _context.Orders.Add(o);


            //Retrieve the Session Cart
            var sessionCart = HttpContext.Session.GetString("cart");
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //Create OrderProduct object for each item in the cart
            foreach (var item in shoppingCart)
            {
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Key,
                    ProductPrice = item.Value.Product.ProductPrice,
                    Quantity = (short)item.Value.Qty
                };

                //ONLY need to add items to an existing entity if the items are a related record
                o.OrderProducts.Add(op);

            }

            //Commit save to DB
            _context.SaveChanges();
            return RedirectToAction("Index", "orders");

        }
    }
}