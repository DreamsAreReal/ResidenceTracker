# 🏠 Здание
Создать программу для учета жильцов в многоквартирном доме. Она
должна включать сущности 
- «Люди» (People), 
- «Квартиры» (Flats), 
- «Жильцы по квартирам» (Setting), 
- «Коммунальные платежи» (Bills). 

Обеспечить возможность добавления и редакирования сущностей, вселения и выселения людей из
квартир, а также подсчет суммарных коммунальных платеже по дому



# 🚀 1. Запуск и остановка приложения с использованием Docker и Docker Compose
## Запуск приложения:
Откройте терминал в корневой папке проекта.

Выполните следующую команду для сборки Docker-образов:

```
docker-compose build
```
Запустите контейнеры в фоновом режиме:
```
docker-compose up -d
```
## Остановка приложения:
В терминале выполните следующую команду:
```
docker-compose down
```
Эта команда остановит и удалит все запущенные контейнеры.

## Доступ к приложению:
После успешного запуска, веб-приложение будет доступно по адресу http://localhost:8002/.

### Важно:
При выполнении `docker-compose down` данные в базе данных будут **удалены**. Это действие полезно для получения чистого окружения при повторном запуске.

# 🛠️ 2. Технологии используемые
Проект построен с использованием следующих технологий и инструментов:

- ASP.NET Core 8.0: Основной фреймворк для веб-приложения.
- Entity Framework Core: ORM для работы с базой данных PostgreSQL.
- Docker: Контейнеризация и управление зависимостями.
- FluentValidation: Библиотека для валидации данных.
- MudBlazor: Компонентный фреймворк для Blazor.
- Dependency Injection: Внедрение зависимостей осуществляется с использованием встроенного механизма Dependency Injection (DI) в .NET Core.
- Blazor: Построения интерактивного пользовательского интерфейса на языке C# и Razor.

# 🏛️ 3. Архитектура
## Общий Обзор
Проект разработан, соблюдая архитектурный шаблон Clean Architecture с использованием технологии Blazor. Clean Architecture помогает добиться высокой гибкости, тестируемости и поддерживаемости приложения, а Blazor обеспечивает создание интерактивных веб-приложений, работая напрямую в браузере.

## Слои
- ResidenceTracker.Frontend: Этот слой содержит код для представления и веб-части приложения, построенные с использованием Blazor. Взаимодействие с пользователем, компоненты интерфейса и клиентская логика размещены здесь.

- ResidenceTracker.UseCases: Здесь сосредоточена бизнес-логика приложения и слой UseCases. В этом слое определены варианты использования, представляющие бизнес-процессы.

- ResidenceTracker.Infrastructure: В этом слое расположены реализации репозиториев и другие службы, отвечающие за взаимодействие с базой данных.

# 🛢️ 4. Схема базы данных (в текстовом формате)
Проект использует PostgreSQL в качестве базы данных. Ниже представлена краткая схема базы данных:

## Таблицы:
`Members`: Информация о жильцах.
- Id (GUID) - идентификатор жильца.
- Name (VARCHAR) - имя жильца.
```sql
CREATE TABLE Members (
                         Id UUID PRIMARY KEY,
                         Name VARCHAR(255) NOT NULL
);
```
`Houses`: Информация о домах.
- Id (GUID) - идентификатор дома.
- Number (INTEGER) - номер дома.
```sql
CREATE TABLE Houses (
                        Id UUID PRIMARY KEY,
                        Number INTEGER NOT NULL
);
```
`Flats`: Информация о квартирах.
- Id (GUID) - идентификатор квартиры.
- Number (INTEGER) - номер квартиры.
- HouseId (UUID) - Id дома
```sql
CREATE TABLE Flats (
                       Id UUID PRIMARY KEY,
                       Number INTEGER NOT NULL,
                       HouseId UUID REFERENCES Houses(Id)
);
```
`Bills`: Информация о счетах.
-  Id (GUID) - идентификатор счета.
- AmountInRubles (DECIMAL) - сумма счета в рублях.
- FlatId (UUID ) - Id квартиры.
```sql
CREATE TABLE Bills (
                       Id UUID PRIMARY KEY,
                       AmountInRubles DECIMAL NOT NULL,
                       FlatId UUID REFERENCES Flats(Id)
);
```
`ResidencyEventLogs`: Информация о событиях проживания.
- Id (GUID) - идентификатор события.
- EventType (VARCHAR) - тип события (например, "Заселение", "Выселение").
- MemberId (UUID) - идентификатор жильца.
- FlatId (UUID) - идентификатор квартиры.
```sql
CREATE TABLE ResidencyEventLogs (
                                    Id UUID PRIMARY KEY,
                                    EventType VARCHAR(255) NOT NULL,
                                    MemberId UUID REFERENCES Members(Id),
                                    FlatId UUID REFERENCES Flats(Id)
);
```
Это лишь краткое описание схемы базы данных. Для подробной информации о типах данных, ключах, и ограничениях, обратитесь к соответствующим сущностям в коде моделей.

# 🔄 5. Работа с Репозиториями
Репозитории в проекте играют важную роль в обеспечении доступа к данным. Каждый репозиторий реализует общий интерфейс `IRepository<T>`, предоставляя методы для добавления, удаления, обновления и получения данных. Ниже представлен пример регистрации и использования репозитория `MemberRepository`.

```csharp
// Регистрация репозитория в ResidenceTracker.UseCases
builder.Services.AddTransient<IRepository<Member>, MemberRepository>();
```
```csharp
// Использование репозитория в коде
var memberRepository = serviceProvider.GetRequiredService<IRepository<Member>>();
var members = await memberRepository.GetPagedAsync(1, 10, cancellationToken);
```

# 🛡️ 6. Валидация данных
## Введение
Валидация данных в проекте Residence Tracker играет ключевую роль в обеспечении целостности и правильности введенной информации. Для достижения этой цели применяются валидаторы, базирующиеся на библиотеке FluentValidation.

## FluentValidation
FluentValidation предоставляет удобный и гибкий механизм валидации объектов в стиле "цепочки вызовов". Каждый валидатор строится по принципу Fluent Interface, что делает код более читаемым и легко поддерживаемым.

### Пример валидатора
Пример валидатора сущности `Flat`:

```csharp
// ResidenceTracker.UseCases.Validation.Implementation.FlatValidator

public class FlatValidator : AbstractFormValidator<Flat>
{
    public FlatValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage($"{nameof(Flat.Number)} не может быть пустым");
    }
}
```
*В этом примере валидатор проверяет, чтобы номер квартиры (Number) был непустым. В случае нарушения правила, генерируется сообщение об ошибке.*

## Применение в проекте
Валидаторы используются в слое `UseCases` для проверки данных перед их сохранением в базе данных. Например, регистрация валидатора `Flat`:

```csharp
// Пример регистрации валидатора в ResidenceTracker.UseCases

builder.Services.AddTransient<AbstractFormValidator<Flat>, FlatValidator>();
```

Такая регистрация позволяет прозрачно интегрировать валидацию в бизнес-процессы приложения.

# ⚙️ 7. Конфигурация и Использование БД
## Подключение к Базе Данных:
Проект использует Entity Framework Core для взаимодействия с базой данных PostgreSQL. Конфигурация подключения определена в классе `ApplicationDbContext`. Изменения в настройках подключения можно внести в файле `appsettings.json` или через переменные окружения.

## Пример конфигурации в appsettings.json:

```json
"ConnectionStrings": {
"DefaultConnection": "Host=residence-tracker-database;Port=5432;Database=ResidenceTrackerDb;User Id=admin;Password=admin;"
}
```
## Использование Контекста БД:
В коде приложения контекст базы данных `ApplicationDbContext` используется для выполнения операций CRUD и обновления временных меток.

```csharp
// Регистрация контекста в ResidenceTracker.UseCases
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
```

```csharp
// Использование контекста в коде
var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
var members = await dbContext.Members.ToListAsync();
```



# 📄 AbstractCrudBasePage
## Обзор
`AbstractCrudBasePage` представляет собой абстрактный компонент страницы для выполнения базовых CRUD-операций *(Create, Read, Update, Delete)* сущностей типа `T`. Этот компонент используется в клиентской части приложения, где осуществляется взаимодействие с данными.

## Основные Характеристики
- `CancellationToken`: Компонент обеспечивает доступ к токену отмены, который может использоваться для отмены асинхронных операций.

- `ChangeableItem`: Представляет текущую сущность, с которой в данный момент ведется работа. Инициализируется новым экземпляром T.

- `DisplayOrder`: Определяет порядок отображения полей в таблице. Предоставляется наследниками.

- `Form`: Ссылка на компонент MudBlazor MudForm, представляющий форму для ввода данных.

- `IsFormHidden`: Состояние видимости формы.

- `IsLoading`: Индикатор загрузки данных или выполнения операции.

- `Logger`: Сервис логирования для отслеживания событий.

- `NavigationManager`: Сервис для управления навигацией в приложении.

- `Repository`: Репозиторий, предоставляющий методы для взаимодействия с данными в базе.

- `Snackbar`: Сервис для отображения уведомлений.

- `Table`: Ссылка на компонент таблицы, отображающей данные.

- `Validator`: Валидатор данных для текущей сущности.

## Основные Методы
- `ChangeFormState(bool isHidden)`: Метод изменения состояния видимости формы.

- `HandleAddNewItem()`: Метод для обработки добавления новой сущности.

- `HandleChangeItem(T selectedItem)`: Метод для обработки изменения существующей сущности.

- `HandleLoadingChange(bool loading)`: Метод для обработки изменения состояния загрузки.

- `OnInitializedAsync()`: Метод инициализации, где настраиваются параметры Snackbar.

- `Submit()`: Метод для обработки отправки данных формы. Выполняет валидацию, сохранение данных и обновление таблицы.

## Использование в Проекте
Данный компонент является базовым для страниц, осуществляющих работу с конкретными типами данных. При использовании в проекте следует унаследовать новую страницу от `AbstractCrudBasePage<T>,` указав тип сущности `T`. Также необходимо реализовать методы для взаимодействия с конкретной бизнес-логикой.

# 🧩 Компонент Table<T>
## Обзор
Компонент `Table<T>` предназначен для отображения данных сущностей типа `T` в виде таблицы. Он обеспечивает возможность выполнения различных операций, таких как добавление, изменение, удаление и поиск данных.

## Основные Характеристики
- `CancellationToken`: Токен отмены для асинхронных операций.

- `DisplayOrder`: Порядок отображения полей в таблице.

- `IsLoading`: Индикатор загрузки данных или выполнения операции.

- `MaxItemsPerPage`: Максимальное количество элементов на странице.

- `OnAddNewItem`: Метод обработки добавления новой сущности.

- `OnChangeItem`: Метод обработки изменения существующей сущности.

- `OnChangeLoadingState`: Callback для уведомления об изменении состояния загрузки.

- `Repository`: Репозиторий, предоставляющий методы для взаимодействия с данными в базе.

- `SelectedItems`: Множество выбранных элементов.

- `ShowButtons`: Флаг отображения кнопок управления данными.

- `Snackbar`: Сервис для отображения уведомлений.

- `TableTitle`: Заголовок таблицы.

## Основные Методы
- `ChangeLoadingState(bool isLoading)`: Метод для изменения состояния загрузки.

- `UpdateTableData()`: Метод для обновления данных таблицы.

## Использование в Проекте
Для использования компонента `Table<T>`, необходимо включить его в разметку страницы, предоставив необходимые параметры. Кроме того, следует реализовать методы для обработки добавления, изменения и удаления данных.

```csharp
<Table CancellationToken="CancellationToken"
       DisplayOrder="@DisplayOrder"
       IsLoading="IsLoading"
       MaxItemsPerPage="20"
       OnAddNewItem="HandleAddNewItem"
       OnChangeItem="HandleChangeItem"
       OnChangeLoadingState="HandleLoadingChange"
       @ref="Table"
       Repository="Repository"
       Snackbar="Snackbar"
       TableTitle="Коммунальные счета"
       TModel="Bill"/>
```
## Генерация Заголовка и Значений
Компонент Table<T> использует параметры DisplayOrder и TableTitle для определения порядка отображения полей в таблице и заголовка таблицы соответственно.

### DisplayOrder
Параметр `DisplayOrder` представляет собой последовательность полей, определяющую порядок отображения. В компоненте `Table<T>` это используется при создании заголовка и ячеек таблицы. Например, следующий код показывает, как использовать этот параметр для отображения столбцов в заданном порядке:

```html
<Table ... DisplayOrder="new[] { "Name", "Age", "Address" }" ... />
```
### TableTitle
Параметр `TableTitle` представляет заголовок таблицы. Он используется для отображения заголовка над таблицей. Пример использования:

```html
<Table ... TableTitle="Список Пользователей" ... />
```

#### Пример Реализации
Приведенный ниже фрагмент кода показывает, как компонент `Table<T>` использует `DisplayOrder` и `TableTitle ` для формирования заголовка и ячеек таблицы:

```html
<HeaderContent>
    @foreach (string propertyName in DisplayOrder)
    {
        <MudTh>@GetDisplayName(propertyName)</MudTh>
    }
</HeaderContent>
<RowTemplate>
    @foreach (string propertyName in DisplayOrder)
    {
        @if (GetPropertyValue(context, propertyName) is IEnumerable<object?>)
        {
            <!-- Обработка коллекций -->
        }
        else
        {
            <MudTd DataLabel="@propertyName">@GetPropertyValue(context, propertyName)</MudTd>
        }
    }
</RowTemplate>
```

### GetDisplayName и GetPropertyValue
Для получения отображаемых имен и значений используются методы `GetDisplayName` и `GetPropertyValue`.

- `GetDisplayName`: Метод возвращает отображаемое имя поля, учитывая возможные вложенности. Например, если поле `Address.City` содержится в `DisplayOrder`, метод вернет "Город (Адрес)".

- `GetPropertyValue`: Метод получает значение свойства для заданной модели и имени свойства. Если свойство содержит точку, означающую вложенность, метод обрабатывает этот случай, возвращая значение вложенного свойства.

```html
<Table CancellationToken="CancellationToken"
       DisplayOrder="@new[] { "Name", "Age", "Address.City" }"
        IsLoading="IsLoading"
        MaxItemsPerPage="20"
        OnAddNewItem="HandleAddNewItem"
        OnChangeItem="HandleChangeItem"
        OnChangeLoadingState="HandleLoadingChange"
        @ref="Table"
        Repository="Repository"
        Snackbar="Snackbar"
        TableTitle="Список Пользователей"
        TModel="User"/>
```
В данном примере компонент `Table<T>` будет отображать таблицу с колонками "Имя", "Возраст" и "Город (Адрес)" для каждого пользователя.

Таким образом, параметры `DisplayOrder` и `TableTitle` компонента `Table<T>` позволяют гибко настраивать отображение таблицы в зависимости от потребностей проекта.