using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LogReg.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace LogReg.Controllers
{
    public class LogRegController : Controller
    {
        private UserContext  dbContext;

        public LogRegController(UserContext context)
        {
            dbContext = context;
        }   

        // GET: /Home/
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/registration")]

        // register user for resgistration
        public IActionResult RegistrationProcess(User user)
        {
            System.Console.WriteLine("*************************************************************************");

            if (ModelState.IsValid)
            {
                if(dbContext.UsersTable.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!!!");
                    return View("Index");
                }
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);

                    dbContext.Add(user);
                    dbContext.SaveChanges();

                    //setting user's first name in session
                    HttpContext.Session.SetString("firstName", user.FirstName);
                    HttpContext.Session.SetString("lastName", user.LastName);

                    return RedirectToAction("Success");
            }
            return View("Index");
            
        }

        //login method and route
        [HttpGet("/login")]
        public IActionResult Login()
        {
            //displaying all the users
            List<User> All = dbContext.UsersTable.ToList();
            ViewBag.AllUsers = All;
            return View();
        }

        //login process
        [HttpPost]
        [Route("/loginProcess")]
        public IActionResult LoginProcess(LoginUser userSubmission)
        {
            List<User> All = dbContext.UsersTable.ToList();
            ViewBag.AllUsers = All;

            if(ModelState.IsValid)
            {
                System.Console.WriteLine("========================================");
                System.Console.WriteLine("Form is Valid");
                System.Console.WriteLine("========================================");
                
                var userInDb= dbContext.UsersTable.SingleOrDefault(u => u.Email == userSubmission.Email);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View("Login");
                }
                HttpContext.Session.SetString("firstName", userInDb.FirstName);
                HttpContext.Session.SetString("lastName", userInDb.LastName);

                return RedirectToAction("Success");
            }
                System.Console.WriteLine("========================================");
                System.Console.WriteLine("Form is Invalid");
                System.Console.WriteLine("========================================");

            return View("Login");
        }

        //renders the Success method and Success.cshtml page
        [HttpGet]
        [Route("/success")]
        public IActionResult Success()
        {
            string username = HttpContext.Session.GetString("firstName");
            if (username is null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.firstname = username;
            ViewBag.lastname = HttpContext.Session.GetString("lastName");
            return View("Success");
        }

        //delete users
        [HttpGet]
        [Route("/delete/{deleteId}")]
        public IActionResult DeleteUser(int deleteId)
        {
            User deleteOne=dbContext.UsersTable.SingleOrDefault(r => r.UserId == deleteId);
            dbContext.UsersTable.Remove(deleteOne);
            dbContext.SaveChanges();
            return RedirectToAction("Login");
        }

        //logout process
        [HttpGet("/logout")]
        public IActionResult Logout()
        {
            // List<User> All = dbContext.UsersTable.ToList();
            // ViewBag.AllUsers = All;

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    
    }
}
