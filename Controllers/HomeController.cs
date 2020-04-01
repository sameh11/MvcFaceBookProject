using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Facebook.Models;
using Microsoft.AspNetCore.Identity;
using Facebook.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Controllers
{
    public class HomeController :Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        public HomeController(UserManager<ApplicationUser> userManager, AppDbContext context, ILogger<HomeController> logger) {
            _logger = logger;
            _context = context;
            this.userManager = userManager;
        }

        //public IActionResult Index() {
        //    return View();
        //}

        public async Task<IActionResult> Index() {


           // _context.Database.Migrate();
            var u = await userManager.GetUserAsync(User);
            AddNewPost addNewPost = new AddNewPost();
            List<Post> friendsPost = new List<Post>();


            addNewPost.mylist = _context.Posts.OrderByDescending(a => a.Date).Where(a => a.user == u).Include(a => a.Likeslist).ToList();
            addNewPost.user = u;
            addNewPost.friends = _context.Friends.Where(a => a.User1 == u).Select(a => a.User2).ToList();
            addNewPost.friendRequests = _context.FriendRequests.Where(a => a.Recive == u && a.requestState == RequestState.Pending).Select(a => a.Send).ToList();

            foreach(var item in addNewPost.mylist)
                item.user = await userManager.GetUserAsync(User);

            foreach(var item in addNewPost.friends) {
                friendsPost = _context.Posts.Where(f => f.user.Id == item.Id).Include(a => a.Likeslist).ToList();
                if(friendsPost != null)
                    addNewPost.mylist = addNewPost.mylist.Concat(friendsPost).OrderByDescending(a => a.Date);
            }

            return View(addNewPost);
        }
        public async Task<IActionResult> Like(int id) {

            var user = await userManager.GetUserAsync(User);
            var post = _context.Posts.FirstOrDefault(a => a.Id == id);

            if(user == null || post == null) {
                return View("Error");
            }

            var like_before = _context.Likes.FirstOrDefault(q => q.Post == post && q.Users == user);
            if(like_before != null) {

                _context.Likes.Remove(like_before);
                post.Likes = post.Likes - 1;
                _context.Posts.Update(post);
            } else {

                like_before = new Like() {
                    Date = DateTime.Now,
                    IsDeleted = false,
                    Post = post,
                    Users = user,
                    Type = React.like

                };
                post.Likes = post.Likes + 1;
                _context.Likes.Add(like_before);
                _context.Posts.Update(post);
            }
            await _context.SaveChangesAsync();



            return RedirectToAction("Index", "Home");

        }


        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
