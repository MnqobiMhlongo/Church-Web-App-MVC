using ChurchSystem.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchSystem.Controllers
{
    public class QuizzesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quizzes
        public ActionResult Index()
        {
            return View(db.Quizzes.ToList());
        }

        public ActionResult students()
        {
            var students = db.StudentQuizResult.Select(a => a.user).Distinct().ToList();

            ViewBag.Students = students;

            return View();
        }


        public ActionResult userReport(string username)
        {

            var attempts = db.StudentQuizResult.Where(a => a.user == username).ToList();

            List<StudentQuizResult> allattempts = db.StudentQuizResult.Where(c => c.user == username).ToList();
            int allPassed = db.StudentQuizResult.Where(b => b.Passed == "true").Count();
            int completed = db.StudentQuizResult.Where(b => b.Passed == "true").Select(a=>a.QuizId).FirstOrDefault();
            int quizcourse = db.Quizzes.Where(a => a.QuizID == completed).Select(a => a.CourseId).FirstOrDefault();
            string course = db.Courses.Where(b => b.CourseId == quizcourse).Select(a => a.CourseName).FirstOrDefault();
            //int allQuizzes = db.Quizzes.Where(a => a.CourseId == id).Count();
            int passedattempts = 0;






            ViewBag.comp = course;
            foreach (var item in allattempts.Where(m => m.Passed == "true"))
            {
                passedattempts++;
            }

           /* if (passedattempts >= allQuizzes)
            {
                ViewBag.comp = true;
            }*/



            return View(attempts);         
        }

        public ActionResult Create(int CourseId)
        {
            ViewBag.CID = CourseId;
            return View();
        }

        // POST: Quizzes/Create
        // Controllers/QuizzesController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Quiz quiz)
        {          
            if (ModelState.IsValid)
            {
                quiz.CreatedAt = DateTime.Now;

                db.Quizzes.Add(quiz);
                db.SaveChanges();

                // Redirect to the AddQuestions method with the newly created quiz ID
                return RedirectToAction("AddQuestions", new { QuizId = quiz.QuizID });
            }
            return View(quiz);
        }

        public ActionResult AddQuestions(int QuizId)
        {
            var quiz = db.Quizzes.Find(QuizId);
            if (quiz == null)
            {
                return HttpNotFound(); // Return a 404 error if the quiz does not exist
            }
            return View(new Question { QuizId = QuizId }); // Pass the quiz ID to the view
        }

        // Controllers/QuizzesController.cs

        // Controllers/QuizzesController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestions(Question question)
        {
            if (ModelState.IsValid)
            {

                // Check if the QuizId is valid
                var quiz = db.Quizzes.Find(question.QuizId);
                if (quiz == null)
                {
                    ModelState.AddModelError("", "The specified quiz does not exist.");
                    return View(question); // Return the same view if the quiz does not exist
                }

                // Now we can safely add the question
                db.Question.Add(question);
                db.SaveChanges();
                return RedirectToAction("Finish", new { id = question.QuizId }); // Redirect after successful addition
            }
            return View(question); // Return the same view if model state is invalid
        }

        public ActionResult Finish(int id)
        {
            var questions = db.Question.Where(a => a.QuizId == id).ToList();
            var courseid = db.Quizzes.Where(a => a.QuizID == id).Select(a=> a.CourseId).FirstOrDefault();

            ViewBag.Id = id;
            ViewBag.QS = questions;
            ViewBag.CID = courseid;
            return View();
        }

        public ActionResult DeleteConfirmed(int id)
        {
            Question itemDonations = db.Question.Find(id);
            int Qid = itemDonations.QuizId;
            db.Question.Remove(itemDonations);
            db.SaveChanges();
            return RedirectToAction("Finish", new { id = Qid});
        }

        public ActionResult QDeleteConfirmed(int id)
        {
            Quiz itemDonations = db.Quizzes.Find(id);
            int Qid = itemDonations.CourseId;
            db.Quizzes.Remove(itemDonations);
            db.SaveChanges();
            return RedirectToAction("quizIndex", new { id = Qid, successMessage = "Quiz Removed" });
        }



        // GET: Quizzes/Take/5
        [Authorize]
        public ActionResult Take(int id)
        {
            var quiz = db.Quizzes.Include("Questions").FirstOrDefault(q => q.QuizID == id);
            if (quiz == null)
            {
                return HttpNotFound();
            }

            // Shuffle answers for each question
            foreach (var question in quiz.Questions)
            {
                var answers = new List<string> { question.CorrectAnswer, question.Answer, question.Answer2 };
                question.ShuffledAnswers = answers.OrderBy(a => Guid.NewGuid()).ToList(); // Shuffling the answers
            }

            return View(quiz);
        }

        // POST: Quizzes/Take/5
        [HttpPost]
        public ActionResult Take(int id, FormCollection form)
        {
            var quiz = db.Quizzes.Include("Questions").FirstOrDefault(q => q.QuizID == id);
            if (quiz == null)
            {
                return HttpNotFound();
            }

            var questionsList = quiz.Questions.ToList();
            int score = 0;

            var studentId = User.Identity.GetUserId(); // Get the current user's ID
            var studentResult = new StudentQuizResult
            {
                StudentId = studentId,
                QuizId = quiz.QuizID,
                Score = score,
                user = User.Identity.GetUserName(),
                Passed = "true"
            };

            db.StudentQuizResult.Add(studentResult);
            db.SaveChanges(); // Ensure studentResult is saved to get the StudentQuizResultId

            for (int i = 0; i < questionsList.Count; i++)
            {
                var questionId = questionsList[i].Id.ToString();
                var submittedAnswer = form["answers_" + questionId];

                bool isCorrect = (submittedAnswer == questionsList[i].CorrectAnswer);
                if (isCorrect)
                {
                    score++;
                }

                var attempt = new QuizAttempt()
                {
                    StudentQuizResultId = studentResult.StudentQuizResultId,
                    QuestionId = questionsList[i].Id,
                    SubmittedAnswer = submittedAnswer,
                    CorrectAnswer = questionsList[i].CorrectAnswer,
                    IsCorrect = isCorrect,
                    user = User.Identity.GetUserName()
                };

                db.QuizAttempt.Add(attempt);
            }

            studentResult.Score = score;

            db.SaveChanges();

            return RedirectToAction("AttemptReview", new { id = studentResult.StudentQuizResultId });
        }

        public ActionResult Details(int id)
        {
            var title = db.Quizzes.Where(a=>a.QuizID==id).Select(a=>a.Title).FirstOrDefault();  
            var Date = db.Quizzes.Where(a => a.QuizID == id).Select(a => a.CreatedAt).FirstOrDefault();
            var qCount = db.Question.Where(a=>a.QuizId == id).Count();
            
            ViewBag.ID = id;
            ViewBag.T = title;
            ViewBag.QC = qCount;
            ViewBag.Date = Date;

            return View();
        }

        public ActionResult ViewResults(int id)
        {
            var attempts = db.StudentQuizResult.Where(a=> a.Quiz.CourseId == id).ToList();

            return View(attempts);
        }

        public ActionResult quizIndex(int id)
        {
            var qz = db.Quizzes.Where(a => a.CourseId == id).ToList();
            ViewBag.ID = id;

            ViewBag.qz = qz;
            return View();
        }

        public ActionResult AdminAttemptReview(int id)
        {
            var quizattempt = db.QuizAttempt.Where(a => a.StudentQuizResultId == id).ToList();
            var user = db.QuizAttempt.Where(a => a.StudentQuizResultId == id).Select(a=>a.user).FirstOrDefault();
            var courseid = db.QuizAttempt.Where(a => a.StudentQuizResultId == id).Select(a=>a.Question.Quiz.CourseId).FirstOrDefault();

            ViewBag.CID = courseid;
            ViewBag.QA = quizattempt;
            ViewBag.user = user;
            return View();
        }


        // GET: Quizzes/Review/5
        public ActionResult Review(int id)
        {
            var result = db.StudentQuizResult.Include("Quiz").FirstOrDefault(r => r.StudentQuizResultId == id);
            return View(result);
        }

        public ActionResult AttemptReview(int id)
        {
            var quizattempt = db.QuizAttempt.Where(a => a.StudentQuizResultId == id).ToList();
            var CID = db.StudentQuizResult.Include("Quiz").Where(a => a.StudentQuizResultId == id).Select(a=>a.Quiz.CourseId).FirstOrDefault();


            ViewBag.QA = quizattempt;
            ViewBag.CID = CID;


            return View();
        }
    }
}