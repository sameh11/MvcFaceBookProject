using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Facebook.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        public ProfileController(UserManager<ApplicationUser> userManager, AppDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
        }

        // GET: Profile
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Index()
        {
            var u = await userManager.GetUserAsync(User);
            AddNewPost addNewPost = new AddNewPost();
            addNewPost.mylist = _context.Posts.OrderByDescending(a=>a.Date).Where( a =>a.user==u).ToList();
            addNewPost.user = u;
            foreach (var item in addNewPost.mylist)
                    item.user = await userManager.GetUserAsync(User);
            return View(addNewPost);
        }

        // GET: Profile/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Profile/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeNewPost(AddNewPost post)
        {
            string uniqueFileName = null;
            if (ModelState.IsValid)
            {
                if (post.Photo != null)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + post.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    post.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                _context.Posts.Add(new Post
                {
                    Date = DateTime.Now,
                    IsDeleted = false,
                    Likes = 0,
                    Photo = uniqueFileName,
                    Text = post.Text,
                    user = await userManager.GetUserAsync(User),
                    feel = post.Feel
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoto(IFormFile photo)
        {
            string uniqueFileName = null;
            if (photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images/UserProfileImage");
                // To make sure the file name is unique we are appending a new
                // GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                photo.CopyTo(new FileStream(filePath, FileMode.Create));

                var id = await userManager.GetUserAsync(User);
                //error here
                var currentUser = await _context.Users.FindAsync(id.Id);
                currentUser.Photo = uniqueFileName;
                _context.SaveChanges();
            }
           return RedirectToAction("Index");
        }
        // GET: Profile/Edit/5
        public async Task< ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return PartialView(post);
        }

        // POST: Profile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult Edit(int id, AddNewPost collection)
        {
            try
            {
                // TODO: Add update logic here
                var post =  _context.Posts.Find(id);
                if (post == null)
                {
                    return NotFound();
                }
                string uniqueFileName = null;
                post.Text = collection.Text;
                if (collection.Photo != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + collection.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    collection.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    post.Photo = uniqueFileName;
                }
                if (collection.Text != null)
                    post.feel = collection.Feel;

                _context.Entry(post).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> EditUserInfo(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var u = await userManager.GetUserAsync(User);
            if (u.Id != id)
            {
                return NotFound();
            }
            return PartialView(u);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserInfo(int id,ApplicationUser collection)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (collection.FName != null)
                    user.FName = collection.FName;
                if (collection.LName != null)
                    user.LName = collection.LName;
                if (collection.BrithDate != null)
                    user.BrithDate = collection.BrithDate;
                if (collection.Bio != null)
                    user.Bio = collection.Bio;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            post.IsDeleted = true;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}