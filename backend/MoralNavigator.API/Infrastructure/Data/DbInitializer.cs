using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Сиды юзеров (если нужно)
            if (!context.Users.Any())
            {
                var admin = new User { Username = "admin", PasswordHash = new byte[0] };
                context.Users.Add(admin);
            }

            // Сиды тестов
            if (!context.Tests.Any())
            {
                // Ваш тест «Нравственный выбор»
                var moralTest = new Test
                {
                    Title = "Нравственный выбор",
                    Questions = new[]
                    {
                        new Question
                        {
                            Text = "Дилемма Хайнца: украсть дорогое лекарство, чтобы спасти жизнь жены, или нет?",
                            Options = new[] { "Украсть лекарство", "Не красть лекарство" },
                            CorrectOption = 0
                        }
                    }
                };

                context.Tests.Add(moralTest);
            }

            context.SaveChanges();
        }
    }
}
