using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCartDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartDataLayer.ConfigurationFile
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(
                new User
                {
                    Id = "e215ed86-87d2-443a-9d10-d8cd761a0a5e",
                    UserName = "admin01@gmail.com",
                    PhoneNumber = "8989898989",
                    Email = "admin01@gmail.com",
                    NormalizedEmail = "admin01@gmail.com",
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin@123")
                }
                );
        }
    }
}
