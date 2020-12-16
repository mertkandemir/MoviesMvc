using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MoviesMvcBilgeAdam.Entities;
using MoviesMvcBilgeAdam.Models.DirectorsModels;

namespace MoviesMvcBilgeAdam.Controllers
{
    public class DirectorsController : Controller
    {
        private MoviesContext db = new MoviesContext();

        private string _detailTitle = "Director's Details"; // const kullanımı için örnek

        // GET: Directors
        public ActionResult Index()
        {
            var viewModel = new DirectorsIndexViewModel()
            {
                Directors = db.Directors.Select(director => new DirectorsModel()
                {
                    Id = director.Id,
                    Name = director.Name,
                    Surname = director.Surname,
                    RetiredText = director.Retired ? "Yes" : "No"
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Directors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Directors directors = db.Directors.Find(id);
            if (directors == null)
            {
                return HttpNotFound();
            }
            var model = new DirectorsModel()
            {
                Id = directors.Id,
                Name = directors.Name,
                Surname = directors.Surname,
                RetiredText = directors.Retired ? "Yes" : "No"
            };
            // _detailsTitle = "Mert"; // const kullanımı için örnek: Değişken const olarak tanımlandığı için hiç bir şekilde değiştirilemez...
            var viewModel = new DirectorsDetailsViewModel()
            {
                Director = model,
                Title = _detailTitle // const kullanımı için örnek
            };
            return View(viewModel);
        }

        // GET: Directors/Create
        public ActionResult Create()
        {
            List<Movies> moviesEntity = db.Movies.ToList();
            MultiSelectList moviesMultiSelectList = new MultiSelectList(moviesEntity, "Id", "Name");
            DirectorsCreateViewModel viewModel = new DirectorsCreateViewModel()
            {
                DirectorEntity = new Directors(),
                MoviesMultiSelectList = moviesMultiSelectList
            };

            return View(viewModel);
        }

        // POST: Directors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Surname,Retired")] Directors directorEntity, List<int> MovieIds) // directorEntity parametresinin adı DirectorsCreateViewModel'deki Directors
        //// tipindeki özellikle (DirectorEntity) aynı olmalı (büyük küçük harf duyarsız)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (MovieIds != null)
        //        {
        //            directorEntity.MovieDirectors = MovieIds.Select(movieId => new MovieDirectors()
        //            {
        //                DirectorId = directorEntity.Id,
        //                MovieId = movieId
        //            }).ToList();
        //        }
        //        db.Directors.Add(directorEntity);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    List<Movies> moviesEntity = db.Movies.ToList();
        //    MultiSelectList moviesMultiSelectList = new MultiSelectList(moviesEntity, "Id", "Name");
        //    DirectorsCreateViewModel viewModel = new DirectorsCreateViewModel()
        //    {
        //        DirectorEntity = new Directors(),
        //        MoviesMultiSelectList = moviesMultiSelectList
        //    };

        //    return View(viewModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DirectorsCreateViewModel directorsCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                if (directorsCreateViewModel != null)
                {
                    directorsCreateViewModel.DirectorEntity.MovieDirectors = directorsCreateViewModel.MovieIds.Select(movieId => new MovieDirectors()
                    {
                        DirectorId = directorsCreateViewModel.DirectorEntity.Id,
                        MovieId = movieId
                    }).ToList();
                }
                db.Directors.Add(directorsCreateViewModel.DirectorEntity);
                db.SaveChanges();
                TempData["Successful"] = "Director created successfully";
                return RedirectToAction("Index");
            }

            List<Movies> moviesEntity = db.Movies.ToList();
            MultiSelectList moviesMultiSelectList = new MultiSelectList(moviesEntity, "Id", "Name");
            DirectorsCreateViewModel viewModel = new DirectorsCreateViewModel()
            {
                DirectorEntity = new Directors(),
                MoviesMultiSelectList = moviesMultiSelectList
            };

            return View(directorsCreateViewModel);
        }

        // GET: Directors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Directors directors = db.Directors.Find(id);
            if (directors == null)
            {
                return HttpNotFound();
            }

            var moviesEntity = db.Movies.ToList();
            List<int> savedMovieIds = directors.MovieDirectors.Select(movieDirector => movieDirector.MovieId).ToList();
            var viewModel = new DirectorsEditViewModel()
            {
                Directors = directors,
                MoviesMultiSelectList = new MultiSelectList(moviesEntity, "Id", "Name",
                    savedMovieIds)
            };
            return View(viewModel);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Retired")] Directors directors, List<int> MovieIds)
        {
            if (ModelState.IsValid)
            {
                // veritabanından parametre olarak gelen id (directors.Id) üzerinden kaydın çekilip bu kaydın güncellenmesi en garanti yöntem
                Directors directorsEntity = db.Directors.Find(directors.Id);

                if (directorsEntity == null)
                {
                    return HttpNotFound();
                }
                directorsEntity.Name = directors.Name;
                directorsEntity.Surname = directors.Surname;
                directorsEntity.Retired = directors.Retired;

                // ilişkili tablolarda Director Id'ye göre silme işlemi yapılır...

                List<MovieDirectors> movieDirectorsEntity = db.MovieDirectors
                    .Where(movieDirector => movieDirector.DirectorId == directors.Id).ToList();
                db.MovieDirectors.RemoveRange(movieDirectorsEntity); 

                // sonra sayfadan gelen verilere göre (MovieIds) ilişkili tablolara bu director için veriler dönüştürülerek ekleme işlemi yapılır...
                directorsEntity.MovieDirectors = MovieIds.Select(movieId => new MovieDirectors()
                {
                    DirectorId = directorsEntity.Id,
                    MovieId = movieId
                }).ToList();

                db.Entry(directorsEntity).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Successful"] = "Director updated successfully";
                return RedirectToAction("Index");
            }
            var moviesEntity = db.Movies.ToList();
            List<int> savedMovieIds = directors.MovieDirectors.Select(movieDirector => movieDirector.MovieId).ToList();
            var viewModel = new DirectorsEditViewModel()
            {
                Directors = directors,
                MoviesMultiSelectList = new MultiSelectList(moviesEntity, "Id", "Name",
                    MovieIds)
            };
            return View(viewModel);
        }

        // GET: Directors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Directors directors = db.Directors.Find(id);
            if (directors == null)
            {
                return HttpNotFound();
            }
            var model = new DirectorsModel()
            {
                Id = directors.Id,
                Name = directors.Name,
                Surname = directors.Surname,
                RetiredText = directors.Retired ? "Yes" : "No"
            };
            
            var viewModel = new DirectorsDetailsViewModel()
            {
                Director = model,
                Title = _detailTitle 
            };
            return View(viewModel);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")] // ActionName'e Delete diyerek get methodunu çağırıyor ve burada kullanılmasını sağlıyor...
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) // imzalar aynı olduğu için sadece Delete diyemiyoruz. Onun yerine DeleteConfirmed ismi verilmiş
        {
            // önce foreign Key'lerin bulunduğu ilişkili tablolardaki veriler silinir.
            List<MovieDirectors> movieDirectorsEntity =
                db.MovieDirectors.Where(movieDirector => movieDirector.DirectorId == id).ToList();
            db.MovieDirectors.RemoveRange(movieDirectorsEntity);

            // sonra ana tablodaki kayıt (directors) kaydı silinir...
            Directors directors = db.Directors.Find(id);
            db.Directors.Remove(directors);

            db.SaveChanges();
            TempData["Successful"] = "Director deleted successfully";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
