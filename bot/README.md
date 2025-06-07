# Telegram Bot Template

Шаблон Telegram бота на Node.js с TypeScript, PostgreSQL, Kafka и вебхуками.

## Особенности

- ✅ Telegram Bot API с вебхуками
- ✅ TypeScript
- ✅ PostgreSQL с Drizzle ORM
- ✅ Kafka для обработки сообщений
- ✅ Winston для логирования
- ✅ ESLint + Prettier
- ✅ Jest для тестирования
- ✅ Docker
- ✅ Модульная архитектура

## Требования

- Node.js 22+
- PostgreSQL
- Kafka (опционально)

## Установка

1. Клонируйте репозиторий
2. Установите зависимости:
   ```bash
   npm install
   ```

3. Скопируйте файл с переменными окружения:
   ```bash
   cp env.example .env
   ```

4. Настройте переменные в `.env` файле

5. Сгенерируйте и примените миграции:
   ```bash
   npm run db:generate
   npm run db:migrate
   ```

## Разработка

```bash
# Запуск в режиме разработки
npm run dev

# Сборка проекта
npm run build

# Запуск продакшн версии
npm start
```

## Тестирование

```bash
# Запуск тестов
npm test

# Запуск тестов в watch режиме
npm run test:watch

# Покрытие кода
npm run test:coverage
```

## Линтинг и форматирование

```bash
# Проверка кода
npm run lint

# Исправление ошибок
npm run lint:fix

# Форматирование кода
npm run format

# Проверка форматирования
npm run format:check
```

## База данных

```bash
# Генерация схемы
npm run db:generate

# Применение миграций
npm run db:migrate

# Push схемы в БД
npm run db:push

# Открыть Drizzle Studio
npm run db:studio
```

## Docker

```bash
# Сборка образа
docker build -t telegram-bot .

# Запуск контейнера
docker run -p 3000:3000 telegram-bot
```

## Структура проекта

```
src/
├── config/           # Конфигурация
├── database/         # База данных
│   ├── schemas/      # Схемы таблиц
│   └── connection.ts # Подключение к БД
├── kafka/            # Kafka конфигурация
├── services/         # Бизнес-логика
├── types/            # TypeScript типы
├── utils/            # Утилиты
├── __tests__/        # Тесты
└── index.ts          # Точка входа
```

## Переменные окружения

См. `env.example` для полного списка переменных.

## API Endpoints

- `POST /webhook` - Webhook для Telegram
- `GET /health` - Health check

## Логирование

Логи сохраняются в папку `logs/`:
- `error.log` - ошибки
- `combined.log` - все логи

В режиме разработки логи также выводятся в консоль. 