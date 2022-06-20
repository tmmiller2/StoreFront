using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration; //Added for access to info in appsettings.json

using StoreFront.UI.MVC.Models; //Added for access to models

using Microsoft.Data.SqlClient; //Added for access to the Connected SQL classes

using MimeKit;

using MailKit.Net.Smtp;

namespace StoreFront.UI.MVC.Controllers
{
    public class StronglyTypedDataController : Controller
    {
        //We won't be using an Index Action/View for this Controller, 
        //so we can simply commment out/delete the one that is provided

        //public IActionResult Index()
        //{
        //return View();
        //}

        #region Adding Credentials to appsettings.json

        //Before creating any Actions or Views related to this Controller, 
        //we will add a Credentials section to the appsettings.json file.
        //This lets us store the (sensitive) login information for our email 
        //account in that file so it does not have to be written here.

        //If you are using Source Control, you can then add the following line 
        //to your .gitignore file to prevent the appsettings.json from being 
        //pushed to the remote repo:

        #endregion

        //Add a Field for the Configuration settings in appsettings.json
        private readonly IConfiguration _config;

        //Add a Constructor for our Controller that accepts the config info as a parameter
        public StronglyTypedDataController(IConfiguration config)
        {
            _config = config;
        }

        //Controller Actions are meant to handle certain types of requests. 
        //The most common request is GET, which is used to request info to 
        //load a page. //We will also create Actions to handle POST requests, 
        //which are used to send info to the app for processing.

        //GET is the default request type to be handled, so no extra info is needed here
        public IActionResult Contact()
        {
            //We want the info in our Contact form to use the ContactViewModel 
            //we created. To do this, we can generate (scaffold) the necessary code
            //using the following steps:

            #region Code Generation Steps

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for Microsoft.VisualStudio.Web
            //3. Click Microsoft.VisualStudio.Web.CodeGeneration.Design
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here and right click the Contact action
            //6. Select Add View, then select the Razor View template and click "Add"
            //7. Enter the following settings:
            //      - View Name: Contact
            //      - Template: Create
            //      - Model Class: ContactViewModel
            //8. Leave all other settings as-is and click "Add"

            #endregion

            return View();
        }

        //Now we need to handle what to do when the user submits the form. For this, 
        //we will make another Contact Action, this time intended to handle the POST request.

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            //When a class has validation attributes, that validation should be checked 
            //BEFORE attempting to process any of the data they provided.

            if (!ModelState.IsValid)
            {
                //Send them back to the form. We can pass the object to the View
                //so the form will contain the original information they provided.

                return View(cvm);
            }

            //To handle sending the email, we'll need to install a NuGet Package
            //and add a few using statements. We can do this with the following steps:

            #region Email Setup Steps & Email Info

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for NETCore.MailKit
            //3. Click NETCore.MailKit
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here
            //6. Add the following using statements & comments:
            //      - using MimeKit; //Added for access to MimeMessage class
            //      - using MailKit.Net.Smtp; //Added for access to SmtpClient class
            //7. Once added, return here to continue coding email functionality

            //MIME - Multipurpose Internet Mail Extensions - Allows email to contain
            //information other than ASCII, including audio, video, images, and HTML

            //SMTP - Secure Mail Transfer Protocol - An internet protocol (similar to HTTP)
            //that specializes in the collection & transfer of email data

            #endregion

            //Create the format for the message content we will receive from the contact form
            string message = $"You have received a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />" +
                $"Message: {cvm.Message}";

            //Create a MimeMessage object to assist with storing/transporting the email 
            //information from the contact form
            var mm = new MimeMessage();

            //Even though the user is the one attempting to send a message to us, the 
            //actual sender of the email is the email we set up with our hosting provider.

            //We can access the credentials for this email user from our appsettings.json file 
            //as shown below.

            mm.From.Add(new MailboxAddress("User", _config.GetValue<string>("Credentials:Email:User")));

            //The recipient of this email will be our personal email address, which is also
            //stored in appsettings.json

            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //The subject will be the one provided by the User, which is stored in the cvm object
            mm.Subject = cvm.Subject;

            //The body of the message will be formatted with the string we created above.
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the message as "urgent" so it will be flagged in our mailbox.
            mm.Priority = MessagePriority.Urgent;

            //We can also add the user's provided email address to the list of ReplyTo addresses 
            //so our replies can be sent directly to them (Instead of sending to our own email user).
            mm.ReplyTo.Add(new MailboxAddress("Sender", cvm.Email));

            //The using directive will create the SmtpClient object, which is used to send the email.
            //Once all of the code inside the using directive's scope has been executed, it will 
            //automatically close any open connections and dispose of the object for us.

            using (var client = new SmtpClient())
            {
                //Connect to the mail server using the credentials in our appsettings.json
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"));

                //Log in to mail server using the credentials for our email user
                client.Authenticate(

                    //Username
                    _config.GetValue<string>("Credentials:Email:User"),

                    //Password
                    _config.GetValue<string>("Credentials:Email:Password")

                    );

                //It's possible the mail server may be down when the user attempts to contact us.
                //So, we can "encapsulate" our code to send the message in a try/catch.
                try
                {
                    //Try to send the email.
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there is an issue, we can store an error message in the ViewBag
                    //to be displayed in the View
                    ViewBag.ErrorMessage = $"There was an error processing your request. Please try " +
                        $"again later.<br />Error Message: {ex.StackTrace}";

                    //Return the user to the View with their form information intact
                    return View(cvm);
                }

            }

            //If all goes well, return a View that displays a confirmation to the user that 
            //there email was sent successfully.

            return View("EmailConfirmation", cvm);
        }

        
    }
}
