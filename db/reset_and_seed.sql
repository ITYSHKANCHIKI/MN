-- Очистить таблицы (правильный порядок для ForeignKey)
TRUNCATE TABLE "TestResults" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Tests" RESTART IDENTITY CASCADE;
TRUNCATE TABLE "Users" RESTART IDENTITY CASCADE;

-- Добавить пользователей
INSERT INTO "Users" ("Username", "PasswordHash") VALUES
('user1', 'hash1'),
('user2', 'hash2');

-- Добавить тесты (без вопросов, упрощённо)
INSERT INTO "Tests" ("Title") VALUES
('Тест 1'),
('Тест 2');
