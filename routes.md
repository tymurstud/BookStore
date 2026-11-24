# Tabela tras aplikacji (Routing)

| Adres (URL / Szablon) | Kontroler.Akcja | Metoda HTTP | Opis |
|---|---|---|---|
| `/books/year/{year}` | Book.ByYear | GET | Filtrowanie ksiazek wg roku wydania (min. 1450) |
| `/books/isbn/{isbn}` | Book.ByIsbn | GET | Szukanie ksiazki po 13-cyfrowym ISBN i przekierowanie do Details |
| `/archive/{year}/{month}` | Book.Archive | GET | Archiwum ksiazek wg roku i miesiaca (1-12) |
| `/docs/{*path}` | Home.Docs | GET | Obsluga sciezki dokumentacji (catch-all) |
| `/Book/Details/{id}` | Book.Details | GET | Routing atrybutowy [HttpGet("Book/Details/{id:int}")] |
| `/Admin/Dashboard/Index` | Admin/Dashboard.Index | GET | Panel administratora, statystyki zadan i Top 5 |
| `/Admin/Authors/Index` | Admin/Authors.Index | GET | Lista autorow w obszarze Admin |
| `/Admin/Authors/Create` | Admin/Authors.Create | GET, POST | Dodawanie nowego autora w obszarze Admin |
| `/Home/Index` | Home.Index | GET | Strona glowna aplikacji |
| `/Home/Privacy` | Home.Privacy | GET | Strona polityki prywatnosci |
| `/Home/Routes` | Home.Routes | GET | Zestawienie tras wygenerowane przez IActionDescriptorCollectionProvider |
| `/Home/HttpStatus` | Home.HttpStatus | GET | Wlasne strony bledow (404, 403, 401, 500) |
| `/Home/Error` | Home.Error | GET | Widok bledu 500 generowany przez filtr wyjatku |
| `/Book/Index` | Book.Index | GET | Pelna lista ksiazek z magazynu w pamieci |
| `/Book/Hello` | Book.Hello | GET | Zwracanie czystego tekstu |
| `/Book/Status` | Book.Status | GET | Zwracanie wskazanego kodu statusu HTTP |
| `/Book/Echo` | Book.Echo | GET | Odczyt parametrow z query string |
| `/Book/Square/{id}` | Book.Square | GET | Odczyt segmentu sciezki |
| `/Book/Import` | Book.Import | POST | Import danych JSON przez [FromBody] |
| `/Book/Inspect` | Book.Inspect | GET | Odczyt surowego obiektu Request i naglowkow |
| `/Book/Sum` | Book.Sum | GET | Sumowanie parametrow z query string z obsluga bledow |
| `/Book/UserAgent` | Book.UserAgent | GET | Odczyt naglowka User-Agent |
| `/Book/Health` | Book.Health | GET | Zwracanie statusu aplikacji w formacie JSON |
| `/Book/Boom` | Book.Boom | GET | Celowe rzucenie wyjatku do przetestowania GlobalExceptionFilter |
| `/Book/Create` | Book.Create | GET, POST | Formularz dodawania ksiazki z walidacja i wzorcem PRG |
