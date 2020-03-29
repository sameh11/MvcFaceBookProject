using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Facebook.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;



        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            RoleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
           
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await signInManager.SignOutAsync();
            //signInManager.();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl) {





            LoginViewModel model = new LoginViewModel {
                ReturnUrl = returnUrl,
                ExternalLogins =
                ( await signInManager.GetExternalAuthenticationSchemesAsync() ).ToList()
            };

            return View(model);
        }
        [AllowAnonymous]

        public IActionResult ExternalLogin(string provider, string returnUrl) {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new { ReturnUrl = returnUrl });
            var properties = signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult>
            ExternalLoginCallback(string returnUrl = null, string remoteError = null) {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        ( await signInManager.GetExternalAuthenticationSchemesAsync() ).ToList()
            };

            if(remoteError != null) {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if(info == null) {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if(signInResult.Succeeded) {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if(email != null) {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if(user == null) {
                        user = new ApplicationUser {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Abdelrahman.Gamal.Sayed@Gmail.com";

                return View("Error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl) {


            ///EmailConfirmed
            model.ExternalLogins =
      ( await signInManager.GetExternalAuthenticationSchemesAsync() ).ToList();




            if(ModelState.IsValid) {



             
                var user = await userManager.FindByEmailAsync(model.Email);
                ///EmailConfirmed
                //if(user != null && !user.EmailConfirmed &&
                //            ( await userManager.CheckPasswordAsync(user, model.Password) )) {
                //    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                //    return View(model);
                //}



               // var user = await userManager.FindByEmailAsync(model.Email);
            //    await userManager.AddToRoleAsync(user, "Admin");


                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, true);

                if(result.Succeeded) {
                    if(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    } else {
                        return RedirectToAction("index", "home");
                    }
                }
                // If account is lockedout send the use to AccountLocked view
                if(result.IsLockedOut) {
                    return View("AccountLocked");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() {
            return View();
        }
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email) {
            var user = await userManager.FindByEmailAsync(email);

            if(user == null) {
                return Json(true);
            } else {
                return Json($"Email {email} is already in use.");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if(ModelState.IsValid) {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new ApplicationUser {
                    UserName = model.Email,
                    Email = model.Email,
                     BrithDate=model.BrithDate,
                     FName=model.FName,
                     LName=model.LName
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if(result.Succeeded) {


                    ///email confirmation
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = "email confirmation \n " + Url.Action("ConfirmEmail", "Account",
                 new { userId = user.Id, token = token }, Request.Scheme);

                   SendEmail("ConfirmEmail", confirmationLink, user.Email, $"{user.FName} {user.LName}");
                    //    logger.Log(LogLevel.Warning, confirmationLink);



                    // Saves the role in the underlying AspNetRoles table
                    if(!await RoleManager.RoleExistsAsync("Admin"))
                        await RoleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                    
              

                    // var user = await userManager.FindByEmailAsync(model.Email);
                    await userManager.AddToRoleAsync(user, "Admin");

                    await userManager.AddClaimsAsync(user,
            ClaimsStore.AllClaims.Where(a=>a.Type!=""));


                    // If the user is signed in and in the Admin role, then it is
                    // the Admin user that is creating a new user. So redirect the
                    // Admin user to ListRoles action
                    //if(signInManager.IsSignedIn(User) && User.IsInRole("Admin")) {
                    //    return RedirectToAction("ListUsers", "Administration");
                    //}

                    ///abd0
                    ///email confirmation
                    //ViewBag.ErrorTitle = "Registration successful";
                    //ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                    //        "email, by clicking on the confirmation link we have emailed you";
                    //return View("Error");



                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach(var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddPassword() {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if(userHasPassword) {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model) {
            if(ModelState.IsValid) {
                var user = await userManager.GetUserAsync(User);

                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if(!result.Succeeded) {
                    foreach(var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("AddPasswordConfirmation");
            }

            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) {
            if(ModelState.IsValid) {
                // Find the user by email
                var user = await userManager.FindByEmailAsync(model.Email);
                // If the user is found AND Email is confirmed
                if(user != null && await userManager.IsEmailConfirmedAsync(user)) {
                    // Generate the reset password token
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = "password reset link \n "+ Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    // Log the password reset link
                   // logger.Log(LogLevel.Warning, passwordResetLink);
                    SendEmail("ConfirmEmail", passwordResetLink, user.Email, $"{user.FName} {user.LName}");

                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

       [HttpGet]
        public async Task<IActionResult> ChangePassword() { 
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if(!userHasPassword) {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if(ModelState.IsValid) {
                var user = await userManager.GetUserAsync(User);
                if(user == null) {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

               
                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if(!result.Succeeded) {
                    foreach(var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email) {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if(token == null || email == null) {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {
            if(ModelState.IsValid) {
             
                var user = await userManager.FindByEmailAsync(model.Email);

                if(user != null) {
                    // reset the user password
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if(result.Succeeded) {

                    
                        if(await userManager.IsLockedOutAsync(user)) {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                 

                    foreach(var error in result.Errors) {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token) {
            if(userId == null || token == null) {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if(user == null) {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if(result.Succeeded) {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }












        const string fromPassword = "5478963210";

        public RoleManager<IdentityRole> RoleManager { get; }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() {
            return View();
        }

        public void SendEmail(string emailSubject, string emailBody, string toEmail, string toName) {


            var fromAddress = new MailAddress("zojjof@gmail.com", "Facebook");
            var toAddress = new MailAddress(toEmail, toName);
            string subject = emailSubject;
            string body = emailBody;
            var smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using(var message = new MailMessage(fromAddress, toAddress) {
                Subject = subject,
                Body = body
            }) {
                smtp.Send(message);
            }
        }















    }
}