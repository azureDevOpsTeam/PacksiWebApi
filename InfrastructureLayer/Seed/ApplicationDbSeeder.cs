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

            if (!await context.Set<Country>().AnyAsync())
            {
                var iran = new Country
                {
                    Name = "Iran",
                    PersianName = "ایران",
                    IsoCode = "IR",
                    Flag = "Iran.png",
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
                        new() { Name = "Rasht", PersianName = "رشت" },
                        new() { Name = "Urmia", PersianName = "ارومیه" },
                        new() { Name = "Yazd", PersianName = "یزد" },
                        new() { Name = "Kerman", PersianName = "کرمان" },
                        new() { Name = "Sari", PersianName = "ساری" }
                    }
                };

                var afghanistan = new Country
                {
                    Name = "Afghanistan",
                    PersianName = "افغانستان",
                    IsoCode = "AF",
                    Flag = "Afghanistan.png",
                    PhonePrefix = "+93",
                    Cities = new List<City>
                    {
                        new() { Name = "Kabul", PersianName = "کابل" },
                        new() { Name = "Kandahar", PersianName = "قندهار" },
                        new() { Name = "Herat", PersianName = "هرات" },
                        new() { Name = "Mazar-i-Sharif", PersianName = "مزار شریف" }
                    }
                };

                var canada = new Country
                {
                    Name = "Canada",
                    PersianName = "کانادا",
                    IsoCode = "CA",
                    Flag = "Canada.png",
                    PhonePrefix = "+1",
                    Cities = new List<City>
                    {
                        new() { Name = "Toronto", PersianName = "تورنتو" },
                        new() { Name = "Vancouver", PersianName = "ونکوور" },
                        new() { Name = "Montreal", PersianName = "مونترال" },
                        new() { Name = "Calgary", PersianName = "کلگری" },
                        new() { Name = "Ottawa", PersianName = "اتاوا" }
                    }
                };

                var china = new Country
                {
                    Name = "China",
                    PersianName = "چین",
                    IsoCode = "CN",
                    Flag = "China.png",
                    PhonePrefix = "+86",
                    Cities = new List<City>
                    {
                        new() { Name = "Beijing", PersianName = "پکن" },
                        new() { Name = "Shanghai", PersianName = "شانگهای" },
                        new() { Name = "Guangzhou", PersianName = "گوانگژو" },
                        new() { Name = "Shenzhen", PersianName = "شنژن" },
                        new() { Name = "Chengdu", PersianName = "چنگدو" }
                    }
                };

                var iraq = new Country
                {
                    Name = "Iraq",
                    PersianName = "عراق",
                    IsoCode = "IQ",
                    Flag = "Iraq.png",
                    PhonePrefix = "+964",
                    Cities = new List<City>
                    {
                        new() { Name = "Baghdad", PersianName = "بغداد" },
                        new() { Name = "Basra", PersianName = "بصره" },
                        new() { Name = "Erbil", PersianName = "اربیل" },
                        new() { Name = "Mosul", PersianName = "موصل" },
                        new() { Name = "Najaf", PersianName = "نجف" },
                        new() { Name = "Karbala", PersianName = "کربلا" }
                    }
                };

                var qatar = new Country
                {
                    Name = "Qatar",
                    PersianName = "قطر",
                    IsoCode = "QA",
                    Flag = "Qatar.png",
                    PhonePrefix = "+974",
                    Cities = new List<City>
                    {
                        new() { Name = "Doha", PersianName = "دوحه" },
                        new() { Name = "Al Rayyan", PersianName = "الریان" },
                        new() { Name = "Al Wakrah", PersianName = "الوکرة" }
                    }
                };

                var japan = new Country
                {
                    Name = "Japan",
                    PersianName = "ژاپن",
                    IsoCode = "JP",
                    Flag = "Japan.png",
                    PhonePrefix = "+81",
                    Cities = new List<City>
                    {
                        new() { Name = "Tokyo", PersianName = "توکیو" },
                        new() { Name = "Osaka", PersianName = "اوساکا" },
                        new() { Name = "Kyoto", PersianName = "کیوتو" },
                        new() { Name = "Yokohama", PersianName = "یوکوهاما" },
                        new() { Name = "Hiroshima", PersianName = "هیروشیما" }
                    }
                };

                var netherlands = new Country
                {
                    Name = "Netherlands",
                    PersianName = "هلند",
                    IsoCode = "NL",
                    Flag = "Netherlands.png",
                    PhonePrefix = "+31",
                    Cities = new List<City>
                    {
                        new() { Name = "Amsterdam", PersianName = "آمستردام" },
                        new() { Name = "Rotterdam", PersianName = "روتردام" },
                        new() { Name = "The Hague", PersianName = "لاهه" },
                        new() { Name = "Utrecht", PersianName = "اوترخت" }
                    }
                };

                var saudiArabia = new Country
                {
                    Name = "Saudi Arabia",
                    PersianName = "عربستان سعودی",
                    IsoCode = "SA",
                    Flag = "SaudiArabia.png",
                    PhonePrefix = "+966",
                    Cities = new List<City>
                    {
                        new() { Name = "Riyadh", PersianName = "ریاض" },
                        new() { Name = "Jeddah", PersianName = "جده" },
                        new() { Name = "Mecca", PersianName = "مکه" },
                        new() { Name = "Medina", PersianName = "مدینه" },
                        new() { Name = "Dammam", PersianName = "دمام" }
                    }
                };

                var southKorea = new Country
                {
                    Name = "South Korea",
                    PersianName = "کره جنوبی",
                    IsoCode = "KR",
                    Flag = "SouthKorea.png",
                    PhonePrefix = "+82",
                    Cities = new List<City>
                    {
                        new() { Name = "Seoul", PersianName = "سئول" },
                        new() { Name = "Busan", PersianName = "بوسان" },
                        new() { Name = "Incheon", PersianName = "اینچئون" },
                        new() { Name = "Daegu", PersianName = "دائگو" },
                        new() { Name = "Daejeon", PersianName = "دژون" }
                    }
                };

                var spain = new Country
                {
                    Name = "Spain",
                    PersianName = "اسپانیا",
                    IsoCode = "ES",
                    Flag = "Spain.png",
                    PhonePrefix = "+34",
                    Cities = new List<City>
                    {
                        new() { Name = "Madrid", PersianName = "مادرید" },
                        new() { Name = "Barcelona", PersianName = "بارسلونا" },
                        new() { Name = "Valencia", PersianName = "والنسیا" },
                        new() { Name = "Seville", PersianName = "سویا" },
                        new() { Name = "Bilbao", PersianName = "بیلبائو" }
                    }
                };

                var turkey = new Country
                {
                    Name = "Turkey",
                    PersianName = "ترکیه",
                    IsoCode = "TR",
                    Flag = "Turkey.png",
                    PhonePrefix = "+90",
                    Cities = new List<City>
                    {
                        new() { Name = "Ankara", PersianName = "آنکارا" },
                        new() { Name = "Istanbul", PersianName = "استانبول" },
                        new() { Name = "Izmir", PersianName = "ازمیر" },
                        new() { Name = "Antalya", PersianName = "آنتالیا" },
                        new() { Name = "Bursa", PersianName = "بورسا" }
                    }
                };

                var uae = new Country
                {
                    Name = "United Arab Emirates",
                    PersianName = "امارات متحده عربی",
                    IsoCode = "AE",
                    Flag = "UnitedArabEmirates.png",
                    PhonePrefix = "+971",
                    Cities = new List<City>
                    {
                        new() { Name = "Dubai", PersianName = "دبی" },
                        new() { Name = "Abu Dhabi", PersianName = "ابوظبی" },
                        new() { Name = "Sharjah", PersianName = "شارجه" },
                        new() { Name = "Ajman", PersianName = "عجمان" }
                    }
                };

                var uk = new Country
                {
                    Name = "United Kingdom",
                    PersianName = "بریتانیا",
                    IsoCode = "GB",
                    Flag = "UnitedKingdom.png",
                    PhonePrefix = "+44",
                    Cities = new List<City>
                    {
                        new() { Name = "London", PersianName = "لندن" },
                        new() { Name = "Manchester", PersianName = "منچستر" },
                        new() { Name = "Birmingham", PersianName = "بیرمنگام" },
                        new() { Name = "Liverpool", PersianName = "لیورپول" },
                        new() { Name = "Edinburgh", PersianName = "ادینبرو" }
                    }
                };

                var usa = new Country
                {
                    Name = "United States",
                    PersianName = "ایالات متحده",
                    IsoCode = "US",
                    Flag = "UnitedStates.png",
                    PhonePrefix = "+1",
                    Cities = new List<City>
                    {
                        new() { Name = "New York", PersianName = "نیویورک" },
                        new() { Name = "Los Angeles", PersianName = "لس آنجلس" },
                        new() { Name = "Chicago", PersianName = "شیکاگو" },
                        new() { Name = "Houston", PersianName = "هیوستون" },
                        new() { Name = "San Francisco", PersianName = "سانفرانسیسکو" }
                    }
                };

                var norway = new Country
                {
                    Name = "Norway",
                    PersianName = "نروژ",
                    IsoCode = "NO",
                    Flag = "Norway.png",
                    PhonePrefix = "+47",
                    Cities = new List<City>
                    {
                        new() { Name = "Oslo", PersianName = "اسلو" },
                        new() { Name = "Bergen", PersianName = "برگن" },
                        new() { Name = "Stavanger", PersianName = "استاوانگر" },
                        new() { Name = "Trondheim", PersianName = "تروندهایم" }
                    }
                };

                var austria = new Country
                {
                    Name = "Austria",
                    PersianName = "اتریش",
                    IsoCode = "AT",
                    Flag = "Austria.png",
                    PhonePrefix = "+43",
                    Cities = new List<City>
                    {
                        new() { Name = "Vienna", PersianName = "وین" },
                        new() { Name = "Salzburg", PersianName = "سالزبورگ" },
                        new() { Name = "Graz", PersianName = "گراز" },
                        new() { Name = "Innsbruck", PersianName = "اینسبروک" }
                    }
                };

                var denmark = new Country
                {
                    Name = "Denmark",
                    PersianName = "دانمارک",
                    IsoCode = "DK",
                    Flag = "Denmark.png",
                    PhonePrefix = "+45",
                    Cities = new List<City>
                    {
                        new() { Name = "Copenhagen", PersianName = "کپنهاگ" },
                        new() { Name = "Aarhus", PersianName = "آرهوس" },
                        new() { Name = "Odense", PersianName = "اودنسه" },
                        new() { Name = "Aalborg", PersianName = "آلبورگ" }
                    }
                };

                var belgium = new Country
                {
                    Name = "Belgium",
                    PersianName = "بلژیک",
                    IsoCode = "BE",
                    Flag = "Belgium.png",
                    PhonePrefix = "+32",
                    Cities = new List<City>
                    {
                        new() { Name = "Brussels", PersianName = "بروکسل" },
                        new() { Name = "Antwerp", PersianName = "آنتورپ" },
                        new() { Name = "Ghent", PersianName = "خنت" },
                        new() { Name = "Bruges", PersianName = "بروژ" }
                    }
                };

                var sweden = new Country
                {
                    Name = "Sweden",
                    PersianName = "سوئد",
                    IsoCode = "SE",
                    Flag = "Sweden.png",
                    PhonePrefix = "+46",
                    Cities = new List<City>
                    {
                        new() { Name = "Stockholm", PersianName = "استکهلم" },
                        new() { Name = "Gothenburg", PersianName = "گوتنبرگ" },
                        new() { Name = "Malmö", PersianName = "مالمو" },
                        new() { Name = "Uppsala", PersianName = "اوپسالا" }
                    }
                };

                var switzerland = new Country
                {
                    Name = "Switzerland",
                    PersianName = "سوئیس",
                    IsoCode = "CH",
                    Flag = "Switzerland.png",
                    PhonePrefix = "+41",
                    Cities = new List<City>
                    {
                        new() { Name = "Zurich", PersianName = "زوریخ" },
                        new() { Name = "Geneva", PersianName = "ژنو" },
                        new() { Name = "Bern", PersianName = "برن" },
                        new() { Name = "Basel", PersianName = "بازل" }
                    }
                };

                var italy = new Country
                {
                    Name = "Italy",
                    PersianName = "ایتالیا",
                    IsoCode = "IT",
                    Flag = "Italy.png",
                    PhonePrefix = "+39",
                    Cities = new List<City>
                    {
                        new() { Name = "Rome", PersianName = "رم" },
                        new() { Name = "Milan", PersianName = "میلان" },
                        new() { Name = "Naples", PersianName = "ناپل" },
                        new() { Name = "Turin", PersianName = "تورین" },
                        new() { Name = "Florence", PersianName = "فلورانس" }
                    }
                };

                var france = new Country
                {
                    Name = "France",
                    PersianName = "فرانسه",
                    IsoCode = "FR",
                    Flag = "France.png",
                    PhonePrefix = "+33",
                    Cities = new List<City>
                    {
                        new() { Name = "Paris", PersianName = "پاریس" },
                        new() { Name = "Marseille", PersianName = "مارسی" },
                        new() { Name = "Lyon", PersianName = "لیون" },
                        new() { Name = "Toulouse", PersianName = "تولوز" },
                        new() { Name = "Nice", PersianName = "نیس" }
                    }
                };

                var portugal = new Country
                {
                    Name = "Portugal",
                    PersianName = "پرتغال",
                    IsoCode = "PT",
                    Flag = "Portugal.png",
                    PhonePrefix = "+351",
                    Cities = new List<City>
                    {
                        new() { Name = "Lisbon", PersianName = "لیسبون" },
                        new() { Name = "Porto", PersianName = "پورتو" },
                        new() { Name = "Braga", PersianName = "براگا" },
                        new() { Name = "Coimbra", PersianName = "کوییمبرا" }
                    }
                };

                var germany = new Country
                {
                    Name = "Germany",
                    PersianName = "آلمان",
                    IsoCode = "DE",
                    Flag = "Germany.png",
                    PhonePrefix = "+49",
                    Cities = new List<City>
                    {
                        new() { Name = "Berlin", PersianName = "برلین" },
                        new() { Name = "Munich", PersianName = "مونیخ" },
                        new() { Name = "Hamburg", PersianName = "هامبورگ" },
                        new() { Name = "Frankfurt", PersianName = "فرانکفورت" },
                        new() { Name = "Cologne", PersianName = "کلن" }
                    }
                };

                await context.Set<Country>().AddRangeAsync(iran, germany, portugal, france, italy, switzerland, sweden, belgium, denmark, austria, norway, afghanistan, canada, turkey, china, iraq, japan, qatar, netherlands, saudiArabia, southKorea, spain, uae, uk, usa);
                await context.SaveChangesAsync();
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