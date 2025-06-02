// C:\PET_projects\course_project_2\MN\backend\MoralNavigator.API\Infrastructure\Data\DbInitializer.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // 1) Применяем миграции
            context.Database.Migrate();

            // 2) Сеим пользователей, если их нет
            if (!context.Users.Any())
            {
                Console.WriteLine("Seeding default users...");

                var adminPassword = "12345678";
                using var hmac = new HMACSHA512();
                var salt = hmac.Key;
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(adminPassword));

                var adminUser = new User
                {
                    Username = "ityshkanchiki",
                    PasswordHash = hash,
                    PasswordSalt = salt
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
                Console.WriteLine("Default admin user seeded.");
            }

            // 3) Сеим тест (если таблица Tests пуста)
            if (!context.Tests.Any())
            {
                Console.WriteLine("Seeding a single test with two questions...");

                var singleTest = new Test
                {
                    Title = "Моральные дилеммы",
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "Heinz должен украсть дорогой препарат, чтобы спасти жизнь жены. Что он должен сделать?",
                            Options = new string[]
                            {
                                "Heinz должен украсть препарат, потому что жизнь важнее закона.",
                                "Heinz не должен воровать, потому что это незаконно."
                            },
                            CorrectOption = 0
                        },
                        new Question
                        {
                            Text = "Поезд несётся на пятерых людей, привязанных к путям. Вы переключаете рельс, чтобы перебросить поезд на другую ветку с одним человеком. Что вы делаете?",
                            Options = new string[]
                            {
                                "Переключаю рычаг и жертвую одним, чтобы спасти пятерых.",
                                "Ничего не делаю — тогда погибнут пятеро."
                            },
                            CorrectOption = 0
                        }
                    }
                };

                context.Tests.Add(singleTest);
                context.SaveChanges();

                Console.WriteLine("Single test with two questions seeded.");
            }
        }
    }
}
