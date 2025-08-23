using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureLayer.Seed
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            List<Role> AllRole = new();
            if (!await context.Set<Role>().AnyAsync())
            {
                var roles = new List<Role>
                {
                    new() { RoleName = "Administrator" },
                    new() { RoleName = "Manager" },
                    new() { RoleName = "User" }
                };

                await context.Set<Role>().AddRangeAsync(roles);
                await context.SaveChangesAsync();

                AllRole = roles;
            }

            // Seed Countries and Cities
            var PreferredIranId = 0;
            var PreferredCanadaId = 0;
            if (!await context.Set<Country>().AnyAsync())
            {
                var iran = new Country
                {
                    Name = "Iran",
                    PersianName = "ایران",
                    IsoCode = "IR",
                    Flag = "IranFlag.png",
                    PhonePrefix = "+98",
                    Cities = new List<City>
                    {
                        new() { Name = "Tehran", PersianName = "تهران" },
                        new() { Name = "Isfahan", PersianName = "اصفهان" },
                        new() { Name = "Shiraz", PersianName = "شیراز" },
                        new() { Name = "Tabriz", PersianName = "تبریز" },
                        new() { Name = "Mashhad", PersianName = "مشهد" },
                        new() { Name = "Ahvaz", PersianName = "اهواز" },
                        new() { Name = "Qom", PersianName = "قم" },
                        new() { Name = "Kermanshah", PersianName = "کرمانشاه" },
                        new() { Name = "Karaj", PersianName = "کرج" },
                        new() { Name = "Rasht", PersianName = "رشت" }
                    }
                };

                var canada = new Country
                {
                    Name = "Canada",
                    PersianName = "کانادا",
                    IsoCode = "CA",
                    Flag = "CanadaFlag.png",
                    PhonePrefix = "+1",
                    Cities = new List<City>
                    {
                        new() { Name = "Ontario", PersianName = "آنتاریو" },
                        new() { Name = "Quebec", PersianName = "کبک" },
                        new() { Name = "British Columbia", PersianName = "بریتیش کلمبیا" },
                        new() { Name = "Alberta", PersianName = "آلبرتا" },
                        new() { Name = "Manitoba", PersianName = "مانیتوبا" },
                        new() { Name = "Saskatchewan", PersianName = "ساسکاچوان" },
                        new() { Name = "Nova Scotia", PersianName = "نوا اسکوشیا" },
                        new() { Name = "New Brunswick", PersianName = "نیو برانزویک" },
                        new() { Name = "Newfoundland and Labrador", PersianName = "نیوفاندلند و لابرادور" },
                        new() { Name = "Prince Edward Island", PersianName = "جزیره پرنس ادوارد" }
                    }
                };

                await context.Set<Country>().AddRangeAsync(iran, canada);
                await context.SaveChangesAsync();

                PreferredIranId = iran.Id;
                PreferredCanadaId = canada.Id;
            }

            // Seed User
            //if (!await context.Set<UserAccount>().AnyAsync())
            //{
            //    var securityStamp = Guid.NewGuid().ToString();
            //    UserAccount userAccount = new()
            //    {
            //        UserName = "+989190693530",
            //        PhonePrefix = "+98",
            //        PhoneNumber = "9190693530",
            //        ConfirmEmail = true,
            //        ConfirmPhoneNumber = true,
            //        SecurityStamp = securityStamp,
            //        Password = HashGenerator.GenerateHashChangePassword("Aa123456@", securityStamp),
            //    };
            //
            //    await context.Set<UserAccount>().AddAsync(userAccount);
            //    await context.SaveChangesAsync();
            //
            //    List<UserRole> userRoles = new();
            //    foreach (var role in AllRole)
            //    {
            //        userRoles.Add(new UserRole
            //        {
            //            RoleId = role.Id,
            //            UserAccount = userAccount
            //        });
            //    }
            //    await context.Set<UserRole>().AddRangeAsync(userRoles);
            //
            //    UserProfile userProfile = new()
            //    {
            //        UserAccount = userAccount,
            //        DisplayName = "شهرام",
            //        FirstName = "شهرام",
            //        LastName = "اویسی",
            //        Gender = GenderEnum.Male,
            //        CountryOfResidenceId = PreferredIranId
            //    };
            //
            //    await context.Set<UserProfile>().AddAsync(userProfile);
            //
            //    UserPreferredLocation userPreferredLocation = new()
            //    {
            //        UserAccount = userAccount,
            //        CountryId = PreferredCanadaId
            //    };
            //
            //    await context.Set<UserPreferredLocation>().AddAsync(userPreferredLocation);
            //    await context.SaveChangesAsync();
            //}
        }
    }
}