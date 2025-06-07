# Multi-stage build для оптимизации размера образа
FROM node:22-alpine AS builder

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы зависимостей
COPY package*.json ./

# Устанавливаем все зависимости
RUN npm ci --frozen-lockfile

# Копируем исходный код
COPY src/ ./src/
COPY tsconfig.json ./

# Компилируем TypeScript
RUN npm run build

# Production stage
FROM node:22-alpine AS production

# Устанавливаем curl для healthcheck и обновляем пакеты
RUN apk add --no-cache curl && \
    apk upgrade --no-cache

# Создаем пользователя для безопасности
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы зависимостей
COPY package*.json ./

# Устанавливаем только production зависимости
RUN npm ci --frozen-lockfile --only=production && \
    npm cache clean --force

# Копируем скомпилированный код из builder stage
COPY --from=builder /app/dist ./dist

# Создаем директорию для логов с правильными правами
RUN mkdir -p logs && \
    chown -R nodejs:nodejs /app

# Переключаемся на непривилегированного пользователя
USER nodejs

# Открываем порт
EXPOSE 3000

# Проверка здоровья с улучшенными параметрами
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:3000/health || exit 1

# Запускаем приложение
CMD ["node", "dist/index.js"] 