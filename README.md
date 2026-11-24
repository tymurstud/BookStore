# BookStore – Lab 1 & Lab 2

Aplikacja ASP.NET Core MVC (projekt BookStore.Web) wykonana w ramach laboratoriów.

---

# Lab 1 – Pierwsza aplikacja MVC i protokół HTTP

## A1. Struktura projektu

| Element | Rola |
|---|---|
| Program.cs | Punkt wejścia aplikacji. Konfiguruje usługi i potok obsługi żądań (middleware) oraz routing. |
| Controllers/ | Klasy kontrolerów obsługujące żądania HTTP (akcje). |
| Models/ | Klasy danych i modeli przekazywanych do widoków. |
| Views/ | Widoki Razor (.cshtml). Views/Shared zawiera layout i widoki częściowe. |
| wwwroot/ | Pliki statyczne (CSS, JS, biblioteki). Jedyny katalog serwowany bezpośrednio. |
| appsettings.json | Konfiguracja aplikacji. |
| Properties/launchSettings.json | Porty i środowisko uruchomieniowe, używane tylko na maszynie dewelopera. |

## B. Podgląd HTTP w przeglądarce

| Pytanie | Odpowiedź |
|---|---|
| Metoda żądania | GET |
| Kod statusu odpowiedzi | 200 OK |
| Content-Type odpowiedzi | text/html; charset=utf-8 |
| Nagłówek User-Agent żądania | Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:159.0) Gecko/20100101 Firefox/159.0 |
| Liczba żądań i dlaczego więcej niż jedno | 8 żądań. Przeglądarka najpierw pobiera dokument HTML (/), a następnie osobnymi żądaniami zasoby, do których on się odwołuje: trzy arkusze CSS (bootstrap.min.css, site.css, BookStore.Web.styles.css), trzy skrypty JavaScript (jquery.min.js, bootstrap.bundle.min.js, site.js) oraz ikonę favicon.ico. |

Adres /abc zwraca kod statusu 404 Not Found, ponieważ w aplikacji nie ma takiej trasy.

## C. BookController

Akcje: Hello (tekst), Status (dowolny kod statusu), Details (widok Razor).

Zadanie C1: wywołania /Book/Status?code=404, ?code=418 i ?code=500 zwracają same kody statusu, które widać w zakładce Network (kolumna Status). Przeglądarka wyświetla pustą stronę lub własny komunikat o błędzie HTTP.

StatusCode(404) nie wyświetla strony błędu, ponieważ zwraca tylko kod statusu z pustym ciałem odpowiedzi. Stronę błędu generuje osobny middleware (UseStatusCodePages), który nie jest włączony w tym szablonie. Wrócimy do tego w Lab 3.

## D. Odczyt danych z żądania

| Akcja | Źródło danych |
|---|---|
| Echo | query string (/Book/Echo?name=Ala&count=3) |
| Square | zmienna ścieżki {id} (/Book/Square/7) |
| Import | ciało żądania w formacie JSON (POST /Book/Import) |
| Inspect | surowy obiekt Request |

Zrzut ekranu zakładki Network dla /Book/Echo?name=Ala: docs/network-echo.png.

## Postman

- POST /Book/Import z nagłówkiem Content-Type: application/json zwraca 200 OK i JSON z odesłanymi polami (docs/postman-200.png).
- POST /Book/Import z nagłówkiem Content-Type: text/plain zwraca 415 Unsupported Media Type, ponieważ powiązanie [FromBody] wymaga zgodnego typu zawartości (docs/postman-415.png).
- GET /Book/Import zwrócił w moim projekcie 404 Not Found (instrukcja przewidywała 405 Method Not Allowed). Akcja Import jest oznaczona atrybutem [HttpPost], więc żądanie GET nie jest przez nią obsługiwane.
- Kolekcja Postman: docs/BookStore.postman_collection.json.

Uwaga: port aplikacji zależy od profilu uruchomieniowego (podczas testów w Postmanie było to 5000, później 5258).

## Zadania samodzielne

- Sum: /Book/Sum?a=2&b=3 zwraca 5. Dla /Book/Sum?a=x&b=3 zwracany jest komunikat "Niepoprawne parametry" ze statusem 400 Bad Request. Model binding nie potrafi zamienić tekstu "x" na typ int, więc ModelState.IsValid ma wartość false i akcja zwraca BadRequest. Bez sprawdzania ModelState parametr a miałby wartość domyślną 0.
- UserAgent: akcja zwraca nagłówek User-Agent klienta. W przeglądarce wynik to Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:159.0) Gecko/20100101 Firefox/159.0. W Postmanie klient przedstawia się innym ciągiem, ponieważ każdy klient wysyła własny User-Agent.
- Health: /Book/Health zwraca JSON z polami status ("ok") i time (bieżący czas). Metoda Json() ustawia nagłówek Content-Type: application/json.

## Pytania kontrolne

1. GET jest idempotentny (wielokrotne wywołanie nie zmienia stanu serwera), a dane przekazuje w adresie URL (query string). POST nie jest idempotentny, a dane przekazuje w ciele żądania.
2. 200 OK – sukces; 302 Found – tymczasowe przekierowanie; 400 Bad Request – błędne żądanie; 401 Unauthorized – brak uwierzytelnienia; 403 Forbidden – brak uprawnień; 404 Not Found – nie znaleziono zasobu; 405 Method Not Allowed – niedozwolona metoda HTTP; 415 Unsupported Media Type – nieobsługiwany typ zawartości; 500 Internal Server Error – błąd serwera.
3. Model binding określa źródło na podstawie atrybutów ([FromQuery], [FromRoute], [FromBody]). Bez atrybutów szuka wartości po nazwie parametru w danych formularza, w trasie i w query string.
4. Pliki z wwwroot są serwowane przez middleware UseStaticFiles. Katalog Controllers zawiera kod źródłowy, który jest kompilowany i wykonywany na serwerze, a nie wysyłany klientowi.

---

# Lab 2 – Widoki Razor, formularze i walidacja

## A. Model i magazyn w pamięci

Dodano klasę `Book` z polami: Id, Title, Isbn, PublicationDate, NumPages, Price, Author.
Dane są przechowywane w statycznej liście `BookStoreMemory.Books` z trzema domyślnymi książkami (Solaris, Cyberiada, Lalka). Lista jest tymczasowa – po restarcie aplikacji nowo dodane książki znikają (baza danych zostanie dodana w Lab 4).

## B. Layout, _ViewStart, sekcje

Zmodyfikowano `_Layout.cshtml`:
- dodano link "Książki" w nawigacji (asp-controller="Book" asp-action="Index"),
- dodano opcjonalną sekcję `@await RenderSectionAsync("Styles", required: false)` przed `</head>`,
- dodano opcjonalną sekcję `@await RenderSectionAsync("Scripts", required: false)` przed `</body>`,
- dodano sekcję `@await RenderSectionAsync("Footer", required: false)` w stopce,
- dodano komunikat sukcesu z TempData: `@if (TempData["Success"] is string ok) { <div class="alert alert-success">@ok</div> }`.

Zadanie B1: widok `Index.cshtml` definiuje `@section Styles { <style> h2 { color: #0a6640; } </style> }` – reguła CSS trafia do `<head>` layoutu. Gdy widok definiuje sekcję, której layout nie renderuje (`@await RenderSectionAsync`), ASP.NET Core zgłosi wyjątek `InvalidOperationException` podczas renderowania.

## C. Lista książek i widok częściowy wiersza

`BookController.Index()` zwraca `View(BookStoreMemory.Books)`. Widok `Views/Book/Index.cshtml` iteruje po liście i renderuje każdy wiersz przez widok częściowy `_BookRow.cshtml`. Użycie widoku częściowego pozwala oddzielić logikę wiersza i wielokrotnie go reużywać.

## D. Model widoku i walidacja atrybutami

Klasa `BookFormViewModel` zawiera pola formularza z atrybutami walidacji:
- `[Required]`, `[StringLength(200, MinimumLength = 2)]` dla tytułu,
- `[Isbn]` (własny atrybut) dla numeru ISBN,
- `[NotInFuture]` (własny atrybut) dla daty wydania,
- `[Range(1, 20000)]` dla liczby stron,
- `[Range(0.01, 10000)]` dla ceny,
- `IValidatableObject` – walidacja na poziomie modelu: książka sprzed 1900 r. nie może kosztować więcej niż 100 zł.

## E. Własne atrybuty walidacji

- `NotInFutureAttribute` – sprawdza czy data nie jest z przyszłości.
- `IsbnAttribute` – sprawdza format ISBN-10 lub ISBN-13 (cyfry, myślniki i spacje dozwolone). Opcja `RequireChecksum = true` włącza weryfikację sumy kontrolnej ISBN-13. Domyślnie wyłączona, ponieważ dane z bazy gravity.db (Lab 5) zawierają numery ISBN niespełniające sumy kontrolnej.

## F. Formularz z tag helperami i Post-Redirect-Get

Formularz `Create.cshtml` używa tag helperów (`asp-for`, `asp-validation-for`, `asp-validation-summary`) i zawiera wszystkie pola z `BookFormViewModel`. Po poprawnym wysłaniu formularza następuje przekierowanie do `Index` (wzorzec PRG), co zapobiega ponownemu wysłaniu formularza przy odświeżeniu strony (F5). Zielony komunikat z `TempData["Success"]` przetrwa jedno przekierowanie i wyświetla się na liście książek.

Walidacja działa po stronie klienta (jQuery Validation Unobtrusive, atrybuty `data-val-*`) oraz po stronie serwera (`ModelState.IsValid`).

## Pytania kontrolne

1. `@Model.Title` jest bezpieczne, bo Razor automatycznie koduje HTML (np. `<script>` staje się `&lt;script&gt;`). `@Html.Raw(Model.Title)` wyłącza kodowanie i jest podatne na XSS.
2. `ViewBag`/`ViewData` przekazują dane między akcją a widokiem w ramach jednego żądania. `TempData` przechowuje dane przez jedno przekierowanie (żądanie + następne żądanie po redirect).
3. Powody tworzenia modelu widoku: (1) ochrona przed overpostingiem – formularz nie może nadpisać pól, których nie ma w VM; (2) inne reguły walidacji niż w encji bazy danych; (3) możliwość agregacji danych z wielu encji w jednym obiekcie.
4. `ModelState.IsValid` sprawdza czy wszystkie atrybuty walidacji zostały spełnione i czy model binding nie napotkał błędów (np. niepoprawny format liczby). Jest wykonywany przez framework automatycznie podczas wiązania modelu w akcji POST.


---

# Lab 3 – Routing, middleware i filtry

## A. Trasy konwencjonalne

W `Program.cs` zarejestrowano trasy w kolejności od najbardziej szczegółowych do ogólnej:
1. `books-by-year` (`books/year/{year:int:min(1450)}`) – ograniczenie typu `int` oraz minimalnej wartości 1450 (rok druku pierwszej książki Gutenberga).
2. `books-by-isbn` (`books/isbn/{isbn:regex(^\\d{{13}}$)}`) – wyrażenie regularne wymuszające dokładnie 13 cyfr.
3. `archive` (`archive/{year:int}/{month:int:range(1,12)}`) – trasa archiwum z ograniczeniem zakresu miesiąca 1–12.
4. `docs` (`docs/{*path}`) – trasa typu catch-all przekazująca całą podścieżkę do parametru `path`.
5. `admin-area` (`{area:exists}/{controller=Dashboard}/{action=Index}/{id?}`) – obsługa obszaru administracyjnego.
6. `default` (`{controller=Home}/{action=Index}/{id?}`) – trasa domyślna.

## B. Routing oparty na atrybutach i zestawienie tras

Dla akcji `BookController.Details` zastosowano routing atrybutowy:
`[HttpGet("Book/Details/{id:int}")]`.
Akcja przestaje być dostępna przez konwencjonalne trasy domyślne, lecz tag helpery Razor (`asp-action="Details" asp-route-id="3"`) generują adresy poprawnie na podstawie szablonu atrybutu.

W `HomeController` dodano akcję `Routes` korzystającą z `IActionDescriptorCollectionProvider`, prezentującą wszystkie zarejestrowane trasy. Pełne zestawienie znajduje się w pliku `routes.md`.

## C. Obszar Admin

Utworzono obszar `Admin` z własnym layoutem `_AdminLayout.cshtml`, nawigacją oraz kontrolerami:
- `DashboardController` (`/Admin/Dashboard`) – wyświetla statystyki serwera (łączną liczbę zapytań oraz Top 5 najpopularniejszych ścieżek).
- `AuthorsController` (`/Admin/Authors`) – lista oraz dodawanie autorów w pamięci podręcznej (`AuthorStore`).

## D. Middleware licznika żądań i czasu obsługi

Zaimplementowano `RequestTimingMiddleware` oraz singleton `RequestStats`:
- Dodaje do nagłówków odpowiedzi `X-Elapsed-Ms` (czas generowania w milisekundach) oraz `X-Request-Count` (numer obsłużonego żądania).
- Zlicza liczbę wywołań poszczególnych ścieżek za pomocą bezpiecznego wątkowo `ConcurrentDictionary<string, long>`, co pozwala na prezentację Top 5 na Dashboardzie.
- Obsługuje mechanizm short-circuit dla ścieżek `/blokada` zwracając status 403 Forbidden bez przekazywania żądania dalej w potoku.

## E. Filtry akcji i filtry wyjątków

- `LogActionFilter` – filtr akcji logujący wejście i wyjście z akcji wraz z przekazanymi argumentami (`START`/`KONIEC`). Zarejestrowany jako `Scoped` i nałożony atrybutem `[ServiceFilter(typeof(LogActionFilter))]`.
- `MeasureTimeAttribute` – filtr akcji dodający nagłówek `X-Action-Ms`.
- `GlobalExceptionFilter` – globalny filtr wyjątków (`builder.Services.AddControllersWithViews(o => o.Filters.Add<GlobalExceptionFilter>())`). Dla ścieżek `/api` zwraca `ProblemDetails` (JSON, status 500), a dla pozostałych widok `Error.cshtml` ze statusem 500 i identyfikatorem śladu. Do testów dodano akcję `/Book/Boom`.

## F. Strony błędów 404 i 500

W `Program.cs` skonfigurowano `app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}")`.
Akcja `HomeController.HttpStatus(int code)` zwraca odpowiedni kod HTTP oraz dedykowany widok z czytelnym komunikatem:
- 404: "Nie znaleziono strony lub zasobu."
- 403: "Brak uprawnień do tego zasobu."
- 401: "Wymagane logowanie."
- pozostałe kody: "Serwer zwrócił kod błędu."
Dzięki `WithReExecute` przeglądarka zachowuje oryginalny URL żądania i właściwy kod statusu HTTP.


---

# Lab 4 – Entity Framework Core (Code First) i wstrzykiwanie zależności

## A. Pakiety i narzędzie dotnet ef

Do projektu `BookStore.Web` zainstalowano pakiety:
- `Microsoft.EntityFrameworkCore.Sqlite` – dostawca bazy danych SQLite dla EF Core.
- `Microsoft.EntityFrameworkCore.Design` – narzędzia czasu projektowania do tworzenia migracji.
Utworzono manifest narzędzi (`dotnet new tool-manifest`) i zainstalowano lokalne narzędzie `dotnet-ef`.

## B. Encja, DbContext, łańcuch połączenia

- Model `Book` rozszerzono o pole `Publisher` oraz atrybuty walidacji (`[Required]`, `[StringLength(200)]`).
- Utworzono klasę kontekstu `AppDbContext` w przestrzeni `BookStore.Web.Data`:
  - `DbSet<Book> Books => Set<Book>();`
  - W `OnModelCreating`: precyzja ceny `HasPrecision(10, 2)`, unikalny indeks na numerze ISBN `HasIndex(x => x.Isbn).IsUnique()`, dane początkowe seedowane przez `HasData` (Solaris, Lalka).
- W `appsettings.json` zdefiniowano połączenie: `"ConnectionStrings": { "Default": "Data Source=bookstore.db" }` oraz włączono logowanie poleceń SQL: `"Microsoft.EntityFrameworkCore.Database.Command": "Information"`.
- Zarejestrowano kontekst w `Program.cs` za pomocą `builder.Services.AddDbContext<AppDbContext>`.

## C. Migracje i baza SQLite

- Wygenerowano migrację początkową `InitialCreate` (`dotnet ef migrations add InitialCreate`) oraz zastosowano ją na bazie (`dotnet ef database update`).
- Dodano drugą migrację `AddPublisher` dodającą kolumnę `Publisher` do tabeli `Books`.
- Zaimplementowano automatyczne wykonywanie migracji przy starcie aplikacji w `Program.cs` (`Database.MigrateAsync()` z użyciem `CreateScope()`).

## D. Serwis i wstrzykiwanie zależności

Wydzielono logikę dostępu do danych z kontrolera do dedykowanego serwisu:
- Zdefiniowano interfejs `IBookService` oraz rekord `PagedResult<T>` obsługujący stronicowanie.
- Zaimplementowano `BookService`:
  - Odczyt danych z użyciem `AsNoTracking()` dla operacji pobierania listy, co wyłącza śledzenie zmian przez ChangeTracker i optymalizuje zużycie pamięci.
  - Asynchroniczne metody `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `IsbnExistsAsync`.
- Zarejestrowano serwis w kontenerze DI jako `Scoped` (`builder.Services.AddScoped<IBookService, BookService>()`).
- Zadanie D1: Zmiana na `AddSingleton` w trybie Development powoduje błąd walidacji zakresów (`InvalidOperationException`), ponieważ serwis typu singleton nie może bezpośrednio zależeć od usługi o zasięgu scoped (`AppDbContext`).
- Zadanie D2: Kontroler zależy od abstrakcji `IBookService`, a nie od konkretnego kontekstu bazy danych, co umożliwia łatwe mockowanie serwisu w testach jednostkowych (Lab 7) bez konieczności tworzenia bazy danych.

## E. Pełny CRUD, stronicowanie i wyszukiwanie

W `BookController` zaimplementowano pełen zestaw akcji opartych na `IBookService`:
- `Index`: obsługa stronicowania (`page`, `pageSize`), wyszukiwania wielopolowego (tytuł lub ISBN) oraz sortowania po tytule, dacie wydania lub cenie (`asc`/`desc`).
- `Details`: pobieranie szczegółów książki po identyfikatorze lub zwracanie 404 NotFound.
- `Create`: walidacja unikalności ISBN, mapowanie `BookFormViewModel` -> `Book`, wzorzec PRG z komunikatem w `TempData`.
- `Edit`: walidacja poprawności identyfikatora, sprawdzanie unikalności ISBN z wyłączeniem edytowanego rekordu (`IsbnExistsAsync(isbn, id)`), aktualizacja encji w bazie.
- `Delete` oraz `DeleteConfirmed` (`[ActionName("Delete")]`): potwierdzenie usunięcia i bezpieczne usunięcie książki z bazy.
- Przygotowano dedykowane widoki: `Index.cshtml` (nawigacja stronicowania, nagłówki sortowania, pole wyszukiwania), `Edit.cshtml`, `Delete.cshtml`, `Details.cshtml`, `Filtered.cshtml`.

## Zadania samodzielne

1. Sortowanie: zaimplementowano sortowanie po polach `title`, `price` i `date` w kierunkach `asc` i `desc` w `GetPageAsync` wyłącznie za pomocą drzewa wyrażeń LINQ (`OrderBy` / `OrderByDescending`), bez surowych zapytań SQL.
2. Wyszukiwanie wielopolowe: filtrowanie po tytule i ISBN (`b.Title.Contains(term) || b.Isbn.Contains(term)`) tłumaczone jest przez dostawcę SQLite bezpośrednio na klauzulę `WHERE ("b"."Title" LIKE '%' || @term || '%') OR ("b"."Isbn" LIKE '%' || @term || '%')` po stronie bazy danych.
3. Automatyczna migracja: `await db.Database.MigrateAsync()` uruchamiane w bloku `CreateScope()` przy starcie aplikacji gwarantuje aktualność schematu bazy danych.
