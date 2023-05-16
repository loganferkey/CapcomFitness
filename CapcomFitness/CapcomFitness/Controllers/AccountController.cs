using CapcomFitness.Data;
using CapcomFitness.Models;
using CapcomFitness.Services;
using CapcomFitness.Utility;
using CapcomFitness.ViewModels;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CapcomFitness.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUtility _utility;
        private readonly IPhotoService _photoService;

        public AccountController(ApplicationDbContext context, 
                                 SignInManager<AppUser> signInManager, 
                                 UserManager<AppUser> userManager, 
                                 RoleManager<IdentityRole> roleManager,
                                 IUtility utility,
                                 IPhotoService photoService) 
        {
            _context = context;
            _signInManager= signInManager;
            _userManager= userManager;
            _roleManager = roleManager;
            _utility = utility;
            _photoService= photoService;
        }

        // Profile overview page in the works,
        // Only works if you're logged in and authenticated
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Should be noted since this has the Authorize annotation, if they aren't logged in it will automatically redirect them to the login page
            AppUser loggedInUser = await _utility.getHTTPContextUser();
            return View(new Settings
            {
                Nickname = loggedInUser.Nickname,
                Biography = loggedInUser.Biography,
                Age = loggedInUser.Age,
                Location = loggedInUser.Location,
                Website = loggedInUser.Website
            });
            
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Settings svm)
        {
            if (ModelState.IsValid)
            {
                var user = await _utility.getHTTPContextUser();
                if (user != null)
                {
                    user.Nickname = svm.Nickname;
                    user.Biography = svm.Biography;
                    user.Age = svm.Age;
                    user.Location = svm.Location;
                    user.Website = svm.Website;

                    if (svm.Image != null && svm.Image.Length > 0)
                    {
                        if (user.ProfileImageUrl == null || user.ProfileImageUrl == "")
                        {
                            // If they choose an image upload it and get the result
                            ImageUploadResult photoResult = await _photoService.AddPhotoAsync(svm.Image);
                            // Their profile image url is whatever is returned
                            user.ProfileImageUrl = photoResult.Url.ToString();
                        } else
                        {
                            try
                            {
                                await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                                ImageUploadResult photoResult = await _photoService.AddPhotoAsync(svm.Image);
                                user.ProfileImageUrl = photoResult.Url.ToString();
                            }
                            catch (Exception ex) 
                            {
                                ModelState.AddModelError("", "Couldn't upload your selected photo");
                                return View(svm);
                            }
                        }
                    }

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["Feedback"] = "Profile changes were saved!";
                        return View(svm);
                    }
                    else
                    {
                        TempData["Feedback"] = "There was an error saving your changes...";
                        return View(svm);
                    }
                }
                TempData["Feedback"] = "There was an error saving your settings";
                return View(svm);
            }
            TempData["Feedback"] = "There was an error saving your settings";
            return View(svm);
        }

        /// <summary>
        /// This view being the one that demo's profiles and the index being reserved for logged in user account settings
        /// </summary>
        /// <param name="id">On every link to this page, asp-route-id of the user's username should be passed through</param>
        /// <returns>Returns a page displaying public user information</returns>
        public async Task<IActionResult> Overview(string? id)
        {
            OverviewViewModel ovm = new OverviewViewModel();
            ovm.Profile = await _userManager.FindByNameAsync(id);
            ovm.User = await _utility.getHTTPContextUser();
            // If we for sure have a user
            if (ovm.Profile != null)
            {
                var routinesFromUser = await _context.Routines.Where(r => r.UserId == ovm.Profile.Id).OrderBy(r => r.Id).ToListAsync();
                ovm.Routines = routinesFromUser.Take(16);
                var commentsOnProfile = await _context.Comments.Where(c => c.UserId == ovm.Profile.Id).Include(c => c.Commenter).OrderByDescending(c => c.DateCreated).ToListAsync();
                ovm.Comments = commentsOnProfile.Take(7);
            }
            return View(ovm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel lvm = new LoginViewModel();
            return View(lvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            // If the form is filled out correctly
            if (ModelState.IsValid)
            {
                // Attempts to signin with the given username and password
                var result = await _signInManager.PasswordSignInAsync(lvm.UserName, lvm.Password, lvm.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    // If the signin succeeded return to the index page
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    // If they're locked out from failing too many times return to locked out page
                    return RedirectToAction("LockedOut", "Account");
                }
                else if (result.IsNotAllowed)
                {
                    // If they're not allowed to sign in?
                    ModelState.AddModelError(string.Empty, "You are not allowed to sign in at the moment.");
                    return View(lvm);
                }
                else if (result.RequiresTwoFactor)
                {
                    // If they require 2FA
                    ModelState.AddModelError(string.Empty, "You require 2FA to login.");
                    return View(lvm);
                }
                else
                {
                    // If some other error, let them know what went wrong
                    ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                    return View(lvm);
                }
            }

            // If all else fails return to the login page
            return View(lvm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel rvm = new RegisterViewModel();
            return View(rvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel rvm)
        {
            // If the form is all filled out correctly
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = rvm.UserName,
                    Email = rvm.Email,
                    EmailConfirmed = true,
                    Nickname = rvm.UserName,
                    Joined = DateTime.Now
                };

                // If the username already exists tell them they can't have it
                if (await _userManager.FindByNameAsync(rvm.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "This username is taken, please try another.");
                    return View(rvm);
                }
                // If the email already exists/is registered tell them they can't have it
                if (await _userManager.FindByEmailAsync(rvm.Email) != null)
                {
                    ModelState.AddModelError("Email", "An account with this email already exists, please try another or login.");
                    return View(rvm);
                }
                
                // Create a new user based on viewmodel/form
                var result = await _userManager.CreateAsync(user, rvm.Password);
                if (result.Succeeded)
                {
                    // If it succeeds, add to user role by default and signin
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If the creating user didn't succeed tell the user why and return!
                    ModelState.AddModelError(string.Empty, string.Join(", ", result.Errors));
                    return View(rvm);
                }
            }

            // If all else just return to the view
            return View(rvm);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LockedOut()
        {
            return View();
        }
    }
}
