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
        public async Task< IActionResult> Index(string searchTerm) {
            var SenderUser = await UserManager.GetUserAsync(User);
            SearchViewModel svm = new SearchViewModel();
            if(!string.IsNullOrEmpty(searchTerm??"")) {
                svm.users = this.context.Users
          .Where(u => u.IsDeleted == false && ( u.FName.ToLower().Contains(searchTerm.ToLower())
          || u.LName.ToLower().Contains(searchTerm.ToLower())
          || u.UserName.ToLower().Contains(searchTerm.ToLower()) )).Include(b => b.Posts).Include(f=>f.FriendRequests).ToList();

            } else
                svm.users = context.Users.Where(u => u.IsDeleted == false).ToList();
            svm.FriendRequestsSent = context.FriendRequests.Where(a => a.IdSend ==SenderUser.Id).ToList();
            svm.Friends = context.Friends.Where(a => a.User1 == SenderUser).Select(u=>u.User2).ToList();
            svm.FriendRequestsReciev = context.FriendRequests.Where(a => a.Recive == SenderUser).ToList();


            return View(svm);
        }

    
        public IActionResult Profile(string id) {

            if(id == null)
                id = Sswitch.id;
            else
            Sswitch.id = id;


            AddNewPost addNewPost = new AddNewPost();
            addNewPost.mylist = context.Posts.OrderByDescending(a => a.Date).Where(a => a.user.Id == id).Include(a=>a.Likeslist).ThenInclude(x => x.Users).ToList();
            addNewPost.user = context.Users.Where(u => u.Id == id).SingleOrDefault();
            foreach(var item in addNewPost.mylist)
                item.user = addNewPost.user;
            return View(addNewPost);
            
        }

        public async Task<IActionResult> Like(int id) {

            var user = await UserManager.GetUserAsync(User);
            var post = context.Posts.FirstOrDefault(a => a.Id == id);

            if(user == null || post == null) {
                return View("Error");
            }

            var like_before = context.Likes.FirstOrDefault(q => q.Post == post && q.Users == user);
            if(like_before != null) {

                context.Likes.Remove(like_before);
                post.Likes = post.Likes - 1;
                context.Posts.Update(post);
            } else {

                like_before = new Like() {
                    Date = DateTime.Now,
                    IsDeleted = false,
                    Post = post,
                    Users = user,
                    Type = React.like

                };
                post.Likes = post.Likes + 1;
                context.Likes.Add(like_before);
                context.Posts.Update(post);
            }
            await context.SaveChangesAsync();



            return RedirectToAction( "Profile");

        }

        public async Task<IActionResult> Cancel(string id) {

            var ReciverUser = await UserManager.GetUserAsync(User);
            var SenderUser = context.Users.Where(u => u.Id == id).FirstOrDefault();
            if(SenderUser == null || ReciverUser == null) {
                return View("Error");
            }
        

            var entity = context.FriendRequests.FirstOrDefault(s => s.Send == SenderUser && s.Recive == ReciverUser);

            if(entity != null) {

             
                context.FriendRequests.Remove(entity);
                await context.SaveChangesAsync();
            }
           


            return RedirectToAction("Index", "Profile");

        }

        public async Task<IActionResult> remove(string id) {

            var SenderUser = await UserManager.GetUserAsync(User);
            var ReciverUser = context.Users.Where(u => u.Id == id).FirstOrDefault();
            if(SenderUser == null || ReciverUser == null) {
                return View("Error");
            }


            var entity = context.FriendRequests.FirstOrDefault(s => s.Send == SenderUser && s.Recive == ReciverUser);

            if(entity != null) {


                context.FriendRequests.Remove(entity);
                await context.SaveChangesAsync();
            }



            return RedirectToAction("Index", "Profile");

        }

        public async Task<IActionResult> Accept(string id) {

            var ReciverUser = await UserManager.GetUserAsync(User);
            var SenderUser = context.Users.Where(u => u.Id == id).FirstOrDefault();
            if(SenderUser == null || ReciverUser == null) {
                return View("Error");
            }
            //FriendRequest af = new FriendRequest() {
            //    IdRecive = ReciverUser.Id,
            //    IdSend = SenderUser.Id,
            //    Recive = ReciverUser,
            //    Send = SenderUser,
            //    requestState = RequestState.Pending,
            //};
            //context.FriendRequests.Add(af);

            Friends friends = new Friends() {
                User1 = SenderUser,
                User2 = ReciverUser,
                User1Id = SenderUser.Id,
                User2Id = ReciverUser.Id,
                FriendshipStart = DateTime.Now
            };

            Friends friends2 = new Friends() {
                User2 = SenderUser,
                User1 = ReciverUser,
                User2Id = SenderUser.Id,
                User1Id = ReciverUser.Id,
                FriendshipStart = DateTime.Now
            };
            context.Friends.Add(friends);
            context.Friends.Add(friends2);

         var entity = context.FriendRequests.FirstOrDefault(s => s.Send == SenderUser && s.Recive == ReciverUser);

            if(entity != null) {

                entity.requestState = RequestState.Accept;

                context.FriendRequests.Update(entity);
            }
            await context.SaveChangesAsync();


            return RedirectToAction("Index", "Profile");

        }

        public async Task< IActionResult> AddFriend(string id) {

            var SenderUser = await UserManager.GetUserAsync(User);
            var ReciverUser =  context.Users.Where(u=>u.Id==id).FirstOrDefault();
            if(SenderUser==null ||ReciverUser ==null) {
                return View("Error");
            }
            FriendRequest af = new FriendRequest() {
                IdRecive = ReciverUser.Id,
                IdSend= SenderUser.Id,
                Recive=ReciverUser,
                Send=SenderUser,
                requestState=RequestState.Pending,
            };
            context.FriendRequests.Add(af);
            await context.SaveChangesAsync();

           
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> UnFriend(string id) {

            var ReciverUser = await UserManager.GetUserAsync(User);
            var SenderUser = context.Users.Where(u => u.Id == id).FirstOrDefault();
            if(SenderUser == null || ReciverUser == null) {
                return View("Error");
            }
        

            var fr1 = context.Friends.FirstOrDefault(a => a.User1 == SenderUser && a.User2 == ReciverUser);
            if(fr1 != null) context.Friends.Remove(fr1);

             fr1 = context.Friends.FirstOrDefault(a => a.User2 == SenderUser && a.User1 == ReciverUser);
            if(fr1 != null) context.Friends.Remove(fr1);
       

            var entity = context.FriendRequests.FirstOrDefault(s => (s.Send == SenderUser && s.Recive == ReciverUser));
            if(entity != null) context.FriendRequests.Remove(entity);
             entity = context.FriendRequests.FirstOrDefault(s => ( s.Recive == SenderUser && s.Send == ReciverUser ));
            if(entity != null) context.FriendRequests.Remove(entity);


            await context.SaveChangesAsync();


            return RedirectToAction("Index", "Profile");

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