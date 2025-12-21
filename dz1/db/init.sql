CREATE TABLE IF NOT EXISTS profiles (
    id SERIAL PRIMARY KEY,
    full_name TEXT NOT NULL,
    email TEXT NOT NULL
);

INSERT INTO profiles(full_name, email)
SELECT 'Иван Иванов', 'ivan@example.com'
WHERE NOT EXISTS (SELECT 1 FROM profiles);

INSERT INTO profiles(full_name, email)
SELECT 'Мария Петрова', 'maria@example.com'
WHERE (SELECT COUNT(*) FROM profiles) = 1;

INSERT INTO profiles(full_name, email)
SELECT 'Алексей Смирнов', 'alex@example.com'
WHERE (SELECT COUNT(*) FROM profiles) = 2;
