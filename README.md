# BookStore – Lab 1

Aplikacja ASP.NET Core MVC (projekt BookStore.Web) wykonana w ramach Lab 1: pierwsza aplikacja MVC i protokół HTTP.

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