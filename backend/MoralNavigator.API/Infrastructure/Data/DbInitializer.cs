// File: backend/MoralNavigator.API/Infrastructure/Data/DbInitializer.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using MoralNavigator.API.Domain.Entities;

namespace MoralNavigator.API.Infrastructure.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Выполняет миграции и безопасно инициализирует базу данных:
        /// 1) Применяет все миграции (и игнорирует ошибку, если таблица уже существует)
        /// 2) Добавляет администратора, если его ещё нет
        /// 3) Синхронизирует тест «Моральные дилеммы» (добавляет, обновляет и удаляет вопросы)
        /// </summary>
        public static void Seed(AppDbContext context)
        {
            // 1) Применяем миграции, но игнорируем ошибку “relation already exists” (код 42P07)
            try
            {
                context.Database.Migrate();
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "42P07")
            {
                // Таблица уже существует — безопасно пропускаем
                Console.WriteLine("⚠️ Миграции пропущены: таблицы уже существуют.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при применении миграций: " + ex.Message, ex);
            }

            // 2) Создаём администратора, если его ещё нет
            if (!context.Users.Any())
            {
                const string adminUsername = "ityshkanchiki";
                const string adminPassword = "12345678";
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

                context.Users.Add(new User
                {
                    Username     = adminUsername,
                    PasswordHash = passwordHash
                });
                context.SaveChanges();
                Console.WriteLine($"✔ Пользователь «{adminUsername}» добавлен");
            }

            // 3) Определяем тест «Моральные дилеммы» и синхронизируем его
            var desiredTest = new Test
            {
                Title = "Моральные дилеммы",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Жена Хайнца была тяжело больна раком. Один местный аптекарь изобрел лекарство от этой болезни, но установил цену в 10 раз больше, чем оно стоило на самом деле. Хайнцу лекарство было не по карману. Он попытался собрать нужную сумму, но смог найти лишь половину. Тогда он пошел к аптекарю просить продать ему лекарство за полцены или разрешить заплатить оставшуюся сумму позже. Но тот не согласился, так как он был первым, кто изобрел лекарство и намеревался заработать на своем изобретении. Тогда мужчина отважился на отчаянный поступок. Он стал обдумывать хищение лекарства.",
                        Options = new[] {
                            "Хайнц должен украсть препарат.",
                            "Хайнц не должен воровать лекарство."
                        },
                        CorrectOption = 0
                    },
                    new Question
                    {
                        Text = "Вы пилот небольшого частного самолета, который терпит крушение, но все еще находится в воздухе. Он летит прямо на большое офисное здание. Парашютов хватает равно на всех, кто находится на борту",
                        Options = new[] {
                            "Спасти всех и себя, выпрыгнув с парашютом. Но тогда погибнут люди в здании",
                            "Увести самолет в сторону, чтобы спасти людей в здании, но тогда вы и пассажиры погибнете"
                        },
                        CorrectOption = 0
                    },
                    new Question
                    {
                        Text = "Вы - врач. В больницу поступила беременная женщина, которая больна агрессивным раком матки. Гистерэктомия спасёт жизнь матери, но плод погибнет. Как поступить?",
                        Options = new[] {
                            "Провести операцию, спасти мать, плод погибнет.",
                            "Отказаться: плод выживет, но мать умрёт."
                        },
                        CorrectOption = 0
                    },
                    new Question
                    {
                        Text = "Вы и группа спелеологов застряла: выход из пещеры перегородил мужчина крупного телосложения, а вода поднимается. У группы есть динамит, которым можно взорвать толстяка и выбраться. Что делать?",
                        Options = new[] {
                            "Взорвать одного, спасая остальных.",
                            "Ничего не делать — все утонут вместе."
                        },
                        CorrectOption = 0
                    },
                    new Question
                    {
                        Text = "Неуправляемый трамвай мчится по рельсам, на основном пути находятся пятеро человек, а на запасном — один. Вы стоите у стрелки. Что делать?",
                        Options = new[] {
                            "Перевести стрелку, убив одного человека ради спасения пятерых.",
                            "Не вмешиваться — дать трамваю продолжить путь, чтобы погибли пятеро."
                        },
                        CorrectOption = 0
                    }
                }
            };

            SyncTest(context, desiredTest);
        }

        /// <summary>
        /// Синхронизирует тест по Title:
        /// ▸ создаёт его, если его ещё нет
        /// ▸ обновляет существующие вопросы (Options и CorrectOption)
        /// ▸ добавляет новые вопросы
        /// ▸ удаляет вопросы, которых нет в новом наборе
        /// </summary>
        private static void SyncTest(AppDbContext context, Test desired)
        {
            // Загружаем существующий тест вместе с вопросами (если он есть)
            var existingTest = context.Tests
                                      .Include(t => t.Questions)
                                      .FirstOrDefault(t => t.Title == desired.Title);

            // Если теста нет в БД, добавляем его целиком
            if (existingTest == null)
            {
                context.Tests.Add(desired);
                context.SaveChanges();
                Console.WriteLine($"✔ Тест «{desired.Title}» создан ({desired.Questions.Count} вопросов)");
                return;
            }

            // Обновляем поля самого теста (заголовок)
            existingTest.Title = desired.Title;

            // Строим словарь существующих вопросов по полю Text
            var existingMap = existingTest.Questions.ToDictionary(q => q.Text);

            // Проходим по каждому новому (желаемому) вопросу
            foreach (var q in desired.Questions)
            {
                if (existingMap.TryGetValue(q.Text, out var existingQuestion))
                {
                    // Вопрос уже есть — обновляем варианты ответа и правильный вариант
                    existingQuestion.Options = q.Options;
                    existingQuestion.CorrectOption = q.CorrectOption;
                }
                else
                {
                    // Вопрос новый — добавляем
                    existingTest.Questions.Add(q);
                }
            }

            // Удаляем вопросы, которые присутствуют в базе, но отсутствуют в желаемом списке
            var desiredTexts = desired.Questions.Select(q => q.Text).ToHashSet();
            var toRemove = existingTest.Questions
                                       .Where(q => !desiredTexts.Contains(q.Text))
                                       .ToList();

            if (toRemove.Any())
            {
                foreach (var obsolete in toRemove)
                {
                    existingTest.Questions.Remove(obsolete);
                }
            }

            // Сохраняем изменения
            context.SaveChanges();
            Console.WriteLine($"✔ Тест «{desired.Title}» синхронизирован ({desired.Questions.Count} вопросов)");
        }
    }
}
