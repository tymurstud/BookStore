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