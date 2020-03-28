using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Controllers
{
    public class UsersController : Controller
    {
        private const int PageSize = 5;
        private readonly AppDbContext context;
      //  private readonly AppDbContext
        //  private readonly IUserService userService;

        public UsersController(UserManager<ApplicationUser> userManager, AppDbContext context, IHostingEnvironment hostingEnvironment) {
            UserManager = userManager;
            this.context = context;
        }

        public UserManager<ApplicationUser> UserManager { get; }
    
        //[HttpGet]

        //public IActionResult Index() {

        //    SearchViewModel svm = new SearchViewModel();
        
        //       svm. users = context.Users.Where(u => u.IsDeleted == false).Include(b => b.Posts).ToList();
        //    return View(svm);
        //}
        [HttpGet]

        public IActionResult Index(string searchTerm) {

            SearchViewModel svm = new SearchViewModel();
            if(!string.IsNullOrEmpty(searchTerm??"")) {
                svm.users = this.context.Users
          .Where(u => u.IsDeleted == false && ( u.FName.ToLower().Contains(searchTerm.ToLower())
          || u.LName.ToLower().Contains(searchTerm.ToLower())
          || u.UserName.ToLower().Contains(searchTerm.ToLower()) )).Include(b => b.Posts).ToList();

            } else
                svm.users = context.Users.Where(u => u.IsDeleted == false).Include(b => b.Posts).ToList();
            return View(svm);
        }

        //public IActionResult AccountDetails(string id, int? page) {
        //    //could be optimized
        //    if(User.GetUserId() == id) {
        //        ViewData[GlobalConstants.Authorization] = GlobalConstants.FullAuthorization;
        //    } else if(this.userService.CheckIfFriends(User.GetUserId(), id)) {
        //        ViewData[GlobalConstants.Authorization] = GlobalConstants.FriendAuthorization;
        //    } else {
        //        ViewData[GlobalConstants.Authorization] = GlobalConstants.NoAuthorization;
        //    }

        //    UserAccountModel user = this.userService.UserDetails(id, page ?? 1, PageSize);

        //    return this.ViewOrNotFound(user);
        //}
        public IActionResult Profile(string id) {

            
            AddNewPost addNewPost = new AddNewPost();
            addNewPost.mylist = context.Posts.OrderByDescending(a => a.Date).Where(a => a.user.Id == id).ToList();
            addNewPost.user = context.Users.Where(u => u.Id == id).SingleOrDefault();
            foreach(var item in addNewPost.mylist)
                item.user = addNewPost.user;
            return View(addNewPost);
            
        }

        public IActionResult Search(string searchTerm, int? page) {
            // ViewData[GlobalConstants.SearchTerm] = searchTerm;

            //if(string.IsNullOrEmpty(searchTerm)) {
            //    var users = this.userService.All(page ?? 1, PageSize);
            //    return this.ViewOrNotFound(users);
            //} else {
            //    var users = this.userService.UsersBySearchTerm(searchTerm, page ?? 1, PageSize);
            //    return this.ViewOrNotFound(users);
            //}

            var users = this.context.Users
               .Where(u => u.IsDeleted == false && ( u.FName.ToLower().Contains(searchTerm.ToLower())
               || u.LName.ToLower().Contains(searchTerm.ToLower())
               || u.UserName.ToLower().Contains(searchTerm.ToLower()) ) );
            //var users=context.Users


                return View(users.ToList());
        }

    }
}