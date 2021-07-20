
using Lab_04.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Lab_04.Controllers
{
    public class CourseController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();
        // GET: Course
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();

            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {

            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            context.Courses.Add(objCourse);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                          .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance temp in listAttendances)
            {
                Course objCourse = temp.Course;
                objCourse.LecturerName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                        .FindById(objCourse.LecturerId).Name;
                courses.Add(objCourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                          .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var courses = context.Courses.Where(c => c.LecturerId == currentUser.Id && c.Datetime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.LecturerName = currentUser.Name;
            }
            return View(courses);
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();

            var listFollwee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();


            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)

            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LecturerName = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                            .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }

        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course x = context.Courses.FirstOrDefault(m => m.Id == id);
            x.ListCategory = context.Categories.ToList();
            if (x == null)
            {
                return HttpNotFound();
            }
            return View(x);
        }
        [Authorize]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditMine(Course objCourse)
        {

            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Edit", objCourse);

            }

            context.Courses.AddOrUpdate(objCourse);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course x = context.Courses.FirstOrDefault(m => m.Id == id);
            if (x == null)
            {
                return HttpNotFound();
            }
            return View(x);
        }
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMine(int id)
        {
            Course x = context.Courses.FirstOrDefault(m => m.Id == id);
            var y = context.Attendances.Where(m => m.CourseId == x.Id).ToList();
            if (x != null)
            {
                foreach (Attendance att in y)
                {
                    context.Attendances.Remove(att);
                }
                context.Courses.Remove(x);
                context.SaveChanges();
                return RedirectToAction("Mine");

            }
            return View();

        }



    }
}