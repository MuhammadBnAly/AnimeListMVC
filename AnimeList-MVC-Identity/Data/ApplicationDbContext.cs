using AnimeList_MVC_Identity.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeList_MVC_Identity.Data
{
    //public class ApplicationDbContext : IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // // it will be all over the project database , not Identitiy only
            //builder.HasDefaultSchema("Schema-Name");

            // Ignoring a colmn
            //builder.Entity<IdentityUser>().ToTable("Users", "Security")
            //.Ignore(x => x.PhoneNumberConfirmed);

            // AppUser (class) to add more colmns
            builder.Entity<AppUser>().ToTable("users", "Security");

            // builder.Entity<IdentityUser>().ToTable("users", "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "Security");

            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "Security");

            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", "Security");

            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "Security");



        }

        

    }
}
