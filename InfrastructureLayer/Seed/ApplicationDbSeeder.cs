using DomainLayer.Entities;
using InfrastructureLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;

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
        //     if (!await context.Set<UserAccount>().AnyAsync())
        //     {
        //         var securityStamp1 = Guid.NewGuid().ToString();
        //         UserAccount userAccount1 = new()
        //         {
        //             UserName = "+989190693530",
        //             PhonePrefix = "+98",
        //             PhoneNumber = "9190693530",
        //             ConfirmEmail = true,
        //             ConfirmPhoneNumber = true,
        //             SecurityStamp = securityStamp1,
        //             Password = HashGenerator.GenerateHashChangePassword("Aa123456@", securityStamp1),
        //             TelegramId = 123456789 // Added TelegramId for testing
        //         };

        //         var securityStamp2 = Guid.NewGuid().ToString();
        //         UserAccount userAccount2 = new()
        //         {
        //             UserName = "+989190693531",
        //             PhonePrefix = "+98",
        //             PhoneNumber = "9190693531",
        //             ConfirmEmail = true,
        //             ConfirmPhoneNumber = true,
        //             SecurityStamp = securityStamp2,
        //             Password = HashGenerator.GenerateHashChangePassword("Aa123456@", securityStamp2),
        //             TelegramId = 1030212127 // Added TelegramId for Shahram's test
        //         };

        //         await context.Set<UserAccount>().AddRangeAsync(userAccount1, userAccount2);
        //         await context.SaveChangesAsync();

        //         List<UserRole> userRoles = new();
        //         foreach (var role in AllRole)
        //         {
        //             userRoles.Add(new UserRole
        //             {
        //                 RoleId = role.Id,
        //                 UserAccount = userAccount1
        //             });
        //             userRoles.Add(new UserRole
        //             {
        //                 RoleId = role.Id,
        //                 UserAccount = userAccount2
        //             });
        //         }
        //         await context.Set<UserRole>().AddRangeAsync(userRoles);

        //         UserProfile userProfile1 = new()
        //         {
        //             UserAccount = userAccount1,
        //             DisplayName = "تست کاربر",
        //             FirstName = "تست",
        //             LastName = "کاربر",
        //             Gender = GenderEnum.Male,
        //             CountryOfResidenceId = PreferredIranId
        //         };

        //         UserProfile userProfile2 = new()
        //         {
        //             UserAccount = userAccount2,
        //             DisplayName = "شهرام",
        //             FirstName = "شهرام",
        //             LastName = "اویسی",
        //             Gender = GenderEnum.Male,
        //             CountryOfResidenceId = PreferredIranId
        //         };

        //         await context.Set<UserProfile>().AddRangeAsync(userProfile1, userProfile2);

        //         UserPreferredLocation userPreferredLocation1 = new()
        //         {
        //             UserAccount = userAccount1,
        //             CountryId = PreferredCanadaId
        //         };

        //         UserPreferredLocation userPreferredLocation2 = new()
        //         {
        //             UserAccount = userAccount2,
        //             CountryId = PreferredCanadaId
        //         };

        //         await context.Set<UserPreferredLocation>().AddRangeAsync(userPreferredLocation1, userPreferredLocation2);
        //         await context.SaveChangesAsync();
        //     }
        }
    }
}