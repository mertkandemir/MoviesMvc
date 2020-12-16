using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MoviesMvcBilgeAdam.Entities
{
    public partial class MoviesContext : DbContext
    {
        public MoviesContext()
            : base("name=MoviesContext")
        {
        }

        public virtual DbSet<Directors> Directors { get; set; }
        public virtual DbSet<MovieDirectors> MovieDirectors { get; set; }
        public virtual DbSet<Movies> Movies { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Directors>()
                .HasMany(e => e.MovieDirectors)
                .WithRequired(e => e.Directors)
                .HasForeignKey(e => e.DirectorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Movies>()
                .HasMany(e => e.MovieDirectors)
                .WithRequired(e => e.Movies)
                .HasForeignKey(e => e.MovieId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Movies>()
                .HasMany(e => e.Reviews)
                .WithRequired(e => e.Movies)
                .HasForeignKey(e => e.MovieId)
                .WillCascadeOnDelete(false);
        }
    }
}
