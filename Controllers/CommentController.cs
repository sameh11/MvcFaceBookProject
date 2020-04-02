using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models;
using Facebook.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Facebook.Controllers
{
    public class CommentController : Controller
    {

        static int? postID;
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;

        public IHostingEnvironment HostingEnvironment { get; }

        public CommentController(UserManager<ApplicationUser> userManager, AppDbContext context, ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment) {
            _logger = logger;
            HostingEnvironment = hostingEnvironment;
            _context = context;
            this.userManager = userManager;
        }

        public IActionResult Index(int? id)
        {

            if(id == null) id = postID; else postID = id;

            AddNewComment model = new AddNewComment();

            model.post = _context.Posts.Where(q => q.Id == id).Include(q => q.user).Include(q => q.Comments).ThenInclude(q => q.user).FirstOrDefault();
          
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeNewComment(AddNewComment comment) {
            string uniqueFileName = null;
            if(ModelState.IsValid) {
                if(comment.Photo != null) {
                string uploadsFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + comment.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    comment.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                var us = await userManager.GetUserAsync(User);
                _context.Comments.Add(new Comment {
                    Date = DateTime.Now,
                    IsDeleted = false,
                    Photo = uniqueFileName,
                    Text = comment.Text,
                    user = await userManager.GetUserAsync(User),
                    Post = _context.Posts.Where(i => i.Id == postID).FirstOrDefault(),
                   
                }) ;

                
                await _context.SaveChangesAsync();
                return RedirectToAction("index");

            }
            return View(comment);
        }



        public async Task<IActionResult> Create(int id, Comment NewComment) {
            var user = await userManager.GetUserAsync(User);
            var post = _context.Posts.FirstOrDefault(a => a.Id == id);

            if(user == null || post == null) {
                return View("Error");
            }
            _context.Comments.Add(new Comment {
                Date = DateTime.Now,
                Text = NewComment.Text,
                IsDeleted = false,
                user = user,
                Post = post,


            });

            return View();
        }

        public ActionResult Edit(int id, int OldCommentId, Comment NewComment) {
            var post = _context.Posts.Find(id);
            if(post == null) {
                return NotFound();
            }
            var AllComment = _context.Comments.Where(p => p.Post == post);
            var OldCommentThatWillBeEdited = AllComment.FirstOrDefault(c => c.Id == OldCommentId);
            if(OldCommentThatWillBeEdited == null) {
                return NotFound();
            } else {
                OldCommentThatWillBeEdited.Text = NewComment.Text;

                _context.Entry(OldCommentThatWillBeEdited).State = EntityState.Modified;
                _context.SaveChanges();
            }


            return View();
        }

        public ActionResult Delete(int id, int CommentId) {
            var post = _context.Posts.Find(id);
            if(post == null) {
                return NotFound();
            }
            var AllComment = _context.Comments.Where(p => p.Post == post);
            var CommentThatWillBeDeleted = AllComment.FirstOrDefault(c => c.Id == CommentId);
            if(CommentThatWillBeDeleted == null) {
                return NotFound();
            } else {
                CommentThatWillBeDeleted.IsDeleted = true;
                _context.Entry(CommentThatWillBeDeleted).State = EntityState.Modified;
                _context.SaveChanges();
            }

            return View();
        }
    }
}