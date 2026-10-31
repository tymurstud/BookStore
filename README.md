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