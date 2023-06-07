# MicroReddit 15063

## Решение
Предлагается решение, основанное на разработке клиента Reddit UWP, которое позволяет просматривать те сообщения, которые вызвали наибольший резонанс среди пользователей социальной сети.

## Пара скрин-фоток
![](/Images/shot1.png)
![](/Images/shot2.png)

## Особенности
- **Поддержка Windows Mobile ARM**
- *Целевая версия*: Windows 10 [v1903] (10.0; сборка 18362)
- *Минимальная версия*: Windows 10 [v1709] (10.0; сборка 15063)
- тип Reddit App, использующего API: Script 
- при аутенфикации задействуется передача User-Agent

## Технологии
- UWP
- С#

## Функциональные возможности
- Аутентификация посредством логина-пароля
- Использование Reddit API.
- Глобальная визуализация основных сообщений.
- Подробный просмотр основного поста.

## Интересующие данные
- Использование шаблона Master/Detail с видимостью "бок о бок".
- Адаптивный пользовательский интерфейс в сравнении с максимизацией или минимизацией.
- Анимация пользовательского интерфейса.
- Обработка заголовков в соответствии с длиной.
- Использование API с использованием класса HttpClient.
- Переменные среды как средство хранения и извлечения учетных данных Reddit.

## Важные переменные, касаемые вашего аккаунта Reddit и API credentials 
- **REDDIT_USER**: * соответствует имени нашего пользователя Reddit.*
- **REDDIT_PASSWORD**: * пароль нашего пользователя Reddit.*
- **REDDIT_CLIENT_ID**:* буквенно-цифровая комбинация, которую Reddit присваивает нашему приложению в качестве идентификатора. *
- ** REDDIT_CLIENT_SECRET**: * буквенно-цифровая комбинация, которую Reddit присваивает нашему приложению в качестве секретного ключа.*

### Ресурсы
- **Reddit API* * (http://www.reddit.com/dev/api)
- **Документация Windows* * (https://docs.microsoft.com/en-us/windows/uwp/)

## Как есть
- Без моей поддержки. "Сделай сам" :)

## Исследователь Медиа
- MediaExplorer 2023


