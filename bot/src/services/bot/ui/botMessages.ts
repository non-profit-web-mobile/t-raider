export const BotMessages = {
  // Приветственные сообщения
  WELCOME: 'Привет, хомячок! 🐹\n\nДобро пожаловать в бота для трейдеров. Я буду присылать тебе обработанные новости для трейдинга.',
  
  // Выбор источников
  SOURCE_SELECTION: '📊 Выберите источники новостей:\n\n' +
    '🔹 Базовые - бесплатные источники (мы еще тренируемся)\n' +
    '💎 Платные - премиум источники с лучшим качеством',
  
  // Выбор риск-профиля
  RISK_SELECTION: '⚖️ Теперь выберите риск-профиль:\n\n' +
    '🛡️ Консерватор - минимальные риски\n' +
    '⚖️ Смешанный - сбалансированный подход\n' +
    '🚀 На максимум - высокие риски, высокая доходность',
  
  // Тикеры
  TICKER_SELECTION: '🎯 Хотите добавить конкретные тикеры для отслеживания?\n\n' +
    'Например: GAZP, LKOH, ROSN, SBER',
  TICKER_INPUT_PROMPT: '📝 Введите тикеры через запятую или пробел:\n\n' +
    'Например: GAZP, LKOH, ROSN, SBER\n' +
    'или: TATN, VTBR, OZON',
  
  // Завершение настройки
  SETUP_COMPLETE: '🎉 Поздравляем! Настройка завершена!\n\n' +
    '✓ Стрим новостей включен\n' +
    '✓ Уведомления активированы\n\n' +
    'Теперь вы будете получать актуальные новости для трейдинга!\n\n',
  
  // Ошибки
  ERROR_GENERIC: 'Произошла ошибка. Попробуйте позже.',
  ERROR_TICKER_ADD: 'Ошибка при добавлении тикеров. Попробуйте еще раз.',
  ERROR_PORTFOLIO: 'Ошибка при добавлении тикеров из портфеля. Попробуйте еще раз.',
  ERROR_SETUP: 'Ошибок при завершении настройки. Попробуйте позже.',
  ERROR_TICKERS_GET: 'Ошибка при получении списка тикеров.',
  ERROR_AVAILABLE_TICKERS: 'Ошибка при получении списка доступных тикеров.',
  ERROR_CLEAR_TICKERS: 'Ошибка при удалении тикеров.',
  ERROR_STATS: 'Ошибка при получении статистики.',
  
  // Команды
  USE_START_COMMAND: 'Используйте команду /start для начала работы с ботом.',
  ENTER_AT_LEAST_ONE_TICKER: 'Пожалуйста, введите хотя бы один тикер.',
  
  // Тикеры - статусы
  NO_TICKERS: '📊 У вас пока нет отслеживаемых тикеров.\n\n' +
    'Добавьте их с помощью кнопки "➕ Добавить тикеры"',
  NO_AVAILABLE_TICKERS: '📋 В базе данных пока нет доступных тикеров.\n\n' +
    'Обратитесь к администратору для добавления тикеров.',
  NO_TICKERS_TO_CLEAR: 'У вас нет тикеров для удаления.',
  AVAILABLE_TICKERS_TITLE: '📋 Доступные тикеры для отслеживания:\n\n',
  
  // Кнопки
  BUTTONS: {
    START: '🚀 Поехали!',
    BASIC_SOURCES: '🔹 Базовые',
    PREMIUM_SOURCES: '💎 Платные',
    CONSERVATIVE: '🛡️ Консерватор',
    MIXED: '⚖️ Смешанный',
    MAXIMUM: '🚀 На максимум',
    PORTFOLIO_TICKERS: '📋 Взять из портфеля',
    ADD_TICKERS: '➕ Добавить тикеры',
    SKIP: '⏭️ Пропустить',
    START_STREAM: '🚀 Запустить стрим',
    CHANGE_SETTINGS: '⚙️ Изменить настройки',
    MY_TICKERS: '📊 Мои тикеры',
  },
  
  // Форматирование
  FORMATTERS: {
    sourceSelected: (sourceText: string) => `✓ Выбраны ${sourceText} источники\n\n`,
    riskSelected: (riskText: string) => `✓ Выбран ${riskText} риск-профиль\n\n`,
    tickersAdded: (tickers: string[]) => `✓ Добавлены тикеры: ${tickers.join(', ')}\n\n`,
    tickersFromPortfolio: (tickers: string[]) => 
      `✓ Добавлены тикеры из портфеля: ${tickers.join(', ')}\n\n🎉 Настройка завершена!`,
    invalidTickers: (invalid: string[], available: string[]) => 
      `⚠️ Неизвестные тикеры (пропущены): ${invalid.join(', ')}\n\n` +
      `Доступные тикеры: ${available.slice(0, 10).join(', ')}...\n\n`,
    userTickersList: (count: number, tickers: string[]) => 
      `📊 Ваши тикеры (${count}):\n\n${tickers.join(', ')}`,
    tickersCleared: (count: number, tickers: string[]) => 
      `🗑️ Удалены все тикеры (${count}): ${tickers.join(', ')}`,
    totalAvailable: (count: number) => `\nВсего доступно: ${count} тикеров`,
    stats: (totalTickers: number, totalAssociations: number) => 
      '📈 Статистика по тикерам:\n\n' +
      `📊 Всего тикеров в базе: ${totalTickers}\n` +
      `👥 Всего подписок пользователей: ${totalAssociations}\n` +
      `📈 Среднее тикеров на пользователя: ${totalAssociations > 0 ? 
        (totalAssociations / Math.max(1, totalTickers)).toFixed(1) : '0'}`
  }
};

export const RiskProfiles = {
  conservative: 'консервативный',
  mixed: 'смешанный', 
  maximum: 'максимальный'
} as const;

export const SourceTypes = {
  basic: 'базовые',
  premium: 'платные'
} as const;

export const ConfidenceValues = {
  conservative: 0.75,
  mixed: 0.5,
  maximum: 0.3
} as const; 