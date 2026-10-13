# BookStore – Lab 1

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
| Nagłówek User-Agent żądania | (wklej swój, np. Mozilla/5.0 ... Chrome/...) |
| Liczba żądań i dlaczego więcej niż jedno | (wpisz liczbę z dołu okna Network). Przeglądarka najpierw pobiera dokument HTML, a następnie osobnymi żądaniami zasoby, do których on się odwołuje: CSS, JavaScript, ikonę favicon. |

Adres /abc zwraca kod statusu 404 Not Found.
## C. BookController

Sprawdzone adresy: /Book/Hello, /Book/Details/3, /Book/Status?code=404, ?code=418, ?code=500.

Zadanie C1: w zakładce Network (wpisz, jak przeglądarka pokazała każdy z kodów: 404, 418, 500).

StatusCode(404) nie wyświetla strony błędu, ponieważ zwraca tylko kod statusu z pustym ciałem odpowiedzi. Stronę błędu generuje osobny middleware (UseStatusCodePages), który nie jest włączony w tym szablonie. Wrócimy do tego w Lab 3.