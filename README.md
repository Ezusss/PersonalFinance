# PersonalFinance (WPF) + xUnit tests

Мини-приложение для учёта личных финансов. UI на WPF (MVVM). Отчёты строятся **за выбранный месяц**. Бизнес-логика покрыта unit-тестами.

---

## Требования
- Windows 10/11  
- Visual Studio 2022 17.11+ или .NET SDK  
- Целевая платформа проекта: `net10.0-windows` (при необходимости можно сменить на `net8.0-windows` в `.csproj`)  
Проверка SDK: запустите `dotnet --info`.

---

## Быстрый старт
1) Клонирование: `git clone https://github.com/<ВАШ_НИК>/<ИМЯ_РЕПО>.git`  
2) Переход в папку: `cd PersonalFinance`  
3) Восстановление пакетов: `dotnet restore`  
4) Сборка: `dotnet build`  
5) Запуск приложения: `dotnet run --project PersonalFinance.csproj`  
6) Запуск тестов: `dotnet test`

---

## Как пользоваться
1) Выберите кошелёк в левой колонке.  
2) Вверху укажите **месяц** (DatePicker).  
3) В блоке «Новая транзакция» заполните дату, тип (`Income`/`Expense`), сумму и описание, затем нажмите **Добавить**.  
   Кнопка активна, если сумма > 0 и для расхода хватает средств.  
4) «Группы по типу (месяц)»: две группы (`Income`/`Expense`), группы отсортированы по общей сумме по убыванию, транзакции внутри — по дате по возрастанию.  
5) «ТОП-3 трат (месяц)»: три крупнейших расхода за выбранный месяц.  
6) Кнопка **Seed** — заполняет демо-данными.

---

## Бизнес-правила
- `CurrentBalance = InitialBalance + ΣIncome − ΣExpense`.  
- Нельзя добавить расход, если сумма превышает текущий баланс.  
- Нельзя добавить транзакцию с суммой `<= 0`.

---

## Структура репозитория
- `Application/` — сервисы (например, `FinanceService`, `IFinanceService`).  
- `Domain/` — сущности: `Wallet`, `Transaction`, `TransactionType`, `TransactionGroup`.  
- `Wpf/` — ViewModels и вспомогательное MVVM.  
- `App.xaml`, `MainWindow.xaml`, … — WPF UI.  
- `PersonalFinance.csproj` — проект приложения.  
- `PersonalFinance.Test/` — xUnit-тесты (`FinanceServiceTests.cs`, `PersonalFinance.Test.csproj`).  
Тестовый проект ссылается на приложение через `ProjectReference` с относительным путём.
