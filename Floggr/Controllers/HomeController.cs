using Floggr.Models;
using HouseWrenDevelopment.Models.Contact;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Floggr.Controllers
{
    public class HomeController : Controller
    {
        private EmailAddress FromAndToEmailAddress;
        private IEmailService EmailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(EmailAddress _fromAddress,
            IEmailService _emailService, ILogger<HomeController> logger)
        {
            FromAndToEmailAddress = _fromAddress;
            EmailService = _emailService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Contact(ContactForm model)
        {
            if (ModelState.IsValid)
            {
                EmailMessage msgToSend = new EmailMessage
                {
                    FromAddresses = new List<EmailAddress> { FromAndToEmailAddress },
                    ToAddresses = new List<EmailAddress> { FromAndToEmailAddress },
                    Content = $"New message from:\n" +
                    $"Name: {model.Name}, " + $"Email: {model.Email} \nMessage: {model.Message}",
                    Subject = "MealMaker Contact - " + model.Subject
                };

                EmailService.Send(msgToSend);
                return RedirectToAction("ThankYou");
            }
            else
            {
                return View();
            }
        }
        public IActionResult ThankYou()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}