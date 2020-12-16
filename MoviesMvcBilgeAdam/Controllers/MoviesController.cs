using MoviesMvcBilgeAdam.Entities;
using MoviesMvcBilgeAdam.Models.Movies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MoviesMvcBilgeAdam.Controllers
{
    public class MoviesController : Controller
    {
        MoviesContext db = new MoviesContext();
        // GET: Movies
        public ActionResult Index() //Default'u Index()
        {
            List<Movies> movieList = db.Movies.ToList();
            return View(movieList); // Sayfada göstereceğimiz Movies listesini databaseden alıp kullanıcıya göstermemizi sağlıyor...
        }

        [HttpGet] // Get Alma işlemi yapar... // Get işlemi için HttpGet dememize gerek yok 
        public ActionResult Create()
        {
            FillDirectors();
            FillYears();
            return View();
        }

        private void FillDirectors(List<int> selectedDirectorIds = null)
        {
            // DIRECTOR
            List<Directors> directorsFromDb = db.Directors.ToList(); // Veritabanından directors listesini çekiyoruz...
            List<SelectListItem> directorSelectListItems = directorsFromDb.Select(director =>
                new SelectListItem() //SelectListItem dediğimiz şey yönetmen listesi MVC karşılığında bu şekilde oluyor...
                {
                    Value = director.Id.ToString(),
                    Text = director.Name + " " + director.Surname
                }).ToList();

            MultiSelectList directorMultiSelectList; // seçilen filmin director kısmının seçili gelmesi için
            if (selectedDirectorIds == null)
            {
                directorMultiSelectList = new MultiSelectList(directorSelectListItems, "Value", "Text");
            }
            else
            {
                directorMultiSelectList = new MultiSelectList(directorSelectListItems, "Value", "Text", selectedDirectorIds);
            }
            ViewBag.Directors = directorMultiSelectList; //View'a veri göndermemizin yöntemi bu başka çare yok...
        }

        private void FillYears(string selectedYearValue = null)
        {
            // YEARLIST
            List<int>
                yearListFromCode =
                    new List<int>(); // Create kısmında yazmaktansa burada da yazabiliriz... en iyi kısım burada yazılandır...
            for (int i = DateTime.Now.Year; i >= 1950; i--)
            {
                yearListFromCode.Add(i);
            }

            List<SelectListItem> yearSelectListItems = yearListFromCode.Select(year => new SelectListItem()
            {
                Value = year.ToString(),
                Text = year.ToString()
            }).ToList();

            SelectList yearSelectList;
            if (selectedYearValue == null)
            {
                yearSelectList = new SelectList(yearSelectListItems, "Value", "Text");
            }
            else
            {
                yearSelectList = new SelectList(yearSelectListItems, "Value", "Text", selectedYearValue);
            }

            ViewBag.Years = yearSelectList;
        }

        ////POST işlemi alınan değerlerin aksiyona verilmesi işlemi // create.cshtml alanında isimlerine ne verdiysek onları yazmalıyız...

        //[HttpPost] // Post işlemi için mecbur httpPost demeliyiz
        // public ActionResult Create(string Name, double? BoxOfficeReturn, string ProductionYear, int[] DirectorIds)
        // {
        //     Movies movie = new Movies()
        //     {
        //         Name = Name,
        //         BoxOfficeReturn = BoxOfficeReturn,
        //         ProductionYear = ProductionYear
        //     };

        //     if (DirectorIds != null)
        //     {

        //         //movie.MovieDirectors = new List<MovieDirectors>();
        //         //// todo: MovieDirectors null kontrolü yapacak
        //         //foreach (int directorId in DirectorIds)
        //         //{
        //         //    movie.MovieDirectors.Add(new MovieDirectors()
        //         //    {
        //         //        MovieId = movie.Id,
        //         //        DirectorId = directorId
        //         //    });
        //         //}

        //         movie.MovieDirectors = DirectorIds.Select(DirectorId => new MovieDirectors()
        //         {
        //             MovieId = movie.Id,
        //             DirectorId = DirectorId
        //         }).ToList();
        //     }
        //     db.Movies.Add(movie);
        //     db.SaveChanges();
        //     return RedirectToAction("Index"); // RedirectToAction dememiz bize ekleme işleminden sonra hangi sayfanın döneceğini gösterir
        // }

        [HttpPost]
        public ActionResult Create(Movies movie, List<int> DirectorIds)
        {
            if (DirectorIds == null)
            {
                //ViewBag.Message = "Please select at least one Director!";
                ViewData["Message"] = "Please select at least one Director!"; // ViewBag ve ViewData aynı görevi görür...
                FillYears();
                FillDirectors();
                return View(); // Herhangi bir aksiyonda return dersek o aksiyona geri döner... // return view diyerek create aksiyonuna geri dönecek...
            }

            //if (string.IsNullOrWhiteSpace(movie.Name))
            //{
            //    ViewData["Message"] = "Please enter MOVİE NAME!";
            //    FillDirectors();
            //    FillYears();
            //    return View();
            //}

            if (!ModelState.IsValid) // Sadece Class'lara bizim class'ımız da şu an Movies sadece Movies'e bakıyor directorIds'e bakmayacak çünkü o bir class değil...
            {
                ViewData["Message"] = "Please enter fields correctly!";
                FillDirectors();
                FillYears();
                return View();
            }

            movie.MovieDirectors = DirectorIds.Select(directorId => new MovieDirectors()
            {
                MovieId = movie.Id,
                DirectorId = directorId
            }).ToList();
            db.Movies.Add(movie);
            db.SaveChanges();
            TempData["Successful"] = "Movie Added successfully..";
            return RedirectToAction("Index"); // RedirectToAction dememiz bize ekleme işleminden sonra hangi sayfanın döneceğini gösterir
        }

        public ActionResult Details(int? id) // istediğimiz kaydın id'si üzerinden onu getireceğiz details işlemini öyle yapacağız... onun için Id lazım bana
        {
            try
            {
                if (!id.HasValue)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
                }

                Movies movieEntity = db.Movies.Find(id.Value); // id nullable olduğu için value'sunu da istememiz daha şık olur.
                if (movieEntity == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                MoviesModel movieModel = new MoviesModel()
                {
                    Id = movieEntity.Id,
                    Name = movieEntity.Name,
                    ProductionYear = movieEntity.ProductionYear,
                    BoxOfficeReturn = movieEntity.BoxOfficeReturn
                };
                movieModel.DirectorNames = "";
                foreach (var movieDirector in movieEntity.MovieDirectors)
                {
                    movieModel.DirectorNames += movieDirector.Directors.Name + " " + movieDirector.Directors.Surname + ", ";
                }

                movieModel.DirectorNames = movieModel.DirectorNames.Trim(' ', ',');
                return View(movieModel);
            }
            catch (Exception exc)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult Delete(int? id) // nullable vermemizin sebebi mesela 1000 numaralı Id şu an elimizde yok diye geri dönecek bize 
        {
            try
            {
                if (!id.HasValue)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!!!");
                }

                var movieEntity = db.Movies.Find(id.Value);
                if (movieEntity == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                //  önce Movies tablosu ile ilişkili olan tablolardaki veriler Movie Id'ye göre çekilir ve ilişkili tablolardan çekilen bu kayıtlar silinir...

                List<MovieDirectors> movieDirectorsEntity = db.MovieDirectors.Where(MovieDirectors => MovieDirectors.MovieId == movieEntity.Id).ToList();
                foreach (var movieDirectorEntity in movieDirectorsEntity)
                {
                    db.MovieDirectors.Remove(movieDirectorEntity);
                }

                List<Reviews> reviewsEntity = db.Reviews.Where(review => review.MovieId == movieEntity.Id).ToList();
                foreach (var reviewEntity in reviewsEntity)
                {
                    db.Reviews.Remove(reviewEntity);
                }
                // daha sonra movies kaydı silinir...
                db.Movies.Remove(movieEntity);
                db.SaveChanges();
                TempData["Successful"] = "Movie DELETED Successfully...";
                return RedirectToAction("Index");
            }
            catch (Exception exc)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!!");
            }

            Movies moviesEntity = db.Movies.SingleOrDefault(movie => movie.Id == id.Value);

            if (moviesEntity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            FillYears(moviesEntity.ProductionYear); // seçili filmin yapım yılı gelsin diye yapıyoruz bunu..
            FillDirectors(moviesEntity.MovieDirectors.Select(movieDirector => movieDirector.DirectorId).ToList()); // seçili filmin Director'ı gelsin diye yapıyoruz.
            return View(moviesEntity);
        }

        [HttpPost]
        public ActionResult Edit(Movies movie, List<int> DirectorIds)
        {
            Movies movieEntity = db.Movies.Find(movie.Id); // güncellemek istediğimiz kayıt veritabanı kaydı onun için entityden çekmemiz lazım...
            if (movieEntity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            movieEntity.Name = movie.Name;
            movieEntity.ProductionYear = movie.ProductionYear;
            movieEntity.BoxOfficeReturn = movie.BoxOfficeReturn;

            List<MovieDirectors> movieDirectorsEntity = 
                db.MovieDirectors.Where(movieDirector => movieDirector.MovieId == movieEntity.Id).ToList(); // veritabanından filmin yönetmeninin Id'si üzerinden getiriyoruz...
            db.MovieDirectors.RemoveRange(movieDirectorsEntity); // seçtiğimiz filmin yönetmen bilgilerini silecek

            movieEntity.MovieDirectors = DirectorIds.Select(directorId => new MovieDirectors() // sildiğimiz filmin yönetmenlerini update yaparken yeni seçilecek film ve yönetmen Idleri yenilenecek
            {
                MovieId = movieEntity.Id,
                DirectorId = directorId
            }).ToList();

            db.Entry(movieEntity).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Successful"] = "Movie Updated Successfully...";
            return RedirectToAction("Index");
        }
    }
}