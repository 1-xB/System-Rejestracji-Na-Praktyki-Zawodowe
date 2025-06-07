# System rejestracji na praktyki zawodowe

System do zarządzania rejestracją studentów na praktyki zawodowe w firmach. Aplikacja umożliwia tworzenie, edycję i zarządzanie relacjami między studentami a firmami oferującymi praktyki.

## Struktura projektu

- **Core**  
  Zawiera podstawowe modele (Student, Company, Registration) oraz interfejsy serwisów i repozytoriów.

- **Data**  
  Warstwa dostępu do danych. Odpowiada za komunikację z bazą danych poprzez repozytoria.

- **Service**  
  Logika biznesowa aplikacji. Implementuje interfejsy z warstwy Core, realizuje operacje na danych i generowanie dokumentów.

- **ConsoleApp**  
  Aplikacja konsolowa - interfejs użytkownika do obsługi systemu.

## Funkcjonalności

- Zarządzanie studentami (dodawanie, edycja, usuwanie)
- Zarządzanie firmami (dodawanie, edycja, usuwanie)
- Rejestracja studentów na praktyki w wybranych firmach
- Automatyczne generowanie umów o praktyki zawodowe w formacie PDF
- Wysyłanie umów na adresy email studentów

## Wymagania systemowe

- .NET 9.0
- Dostęp do bazy danych SQL Server
- Połączenie internetowe (do wysyłania emaili)

## Uruchomienie aplikacji

1. Sklonuj repozytorium
2. Skonfiguruj połączenie z bazą danych w pliku `appsettings.json`
3. Zbuduj projekt: `dotnet build`
4. Uruchom aplikację: `dotnet run --project SystemPraktykZawodowych.ConsoleApp`

## Instrukcja użycia

### Menu główne
System oferuje menu z następującymi opcjami:
1. Pokaż wszystkie rejestracje
2. Znajdź rejestrację po ID
3. Dodaj rejestrację
4. Aktualizuj rejestrację
5. Usuń rejestrację
6. Wyślij umowę
7. Zarządzaj studentami
8. Zarządzaj firmami
0. Wyjście

### Zarządzanie studentami
W sekcji zarządzania studentami możesz:
- Wyświetlić wszystkich studentów
- Wyszukać studenta po ID
- Dodać nowego studenta
- Zaktualizować dane istniejącego studenta
- Usunąć studenta

**UWAGA:** Podczas dodawania studenta **należy podać prawidłowy adres email**, ponieważ jest on używany do wysłania umowy o praktykach. Bez poprawnego adresu email nie będzie możliwe otrzymanie dokumentu umowy.

### Dodawanie rejestracji
Aby zarejestrować studenta na praktyki:
1. Wybierz opcję "Dodaj rejestrację"
2. Podaj ID istniejącego studenta
3. Podaj ID firmy, w której student chce odbyć praktyki
4. System automatycznie sprawdzi czy firma ma dostępne miejsca
5. Po dodaniu rejestracji, system wyśle umowę na adres email studenta

## Technologie

- C# / .NET 9.0
- Microsoft SQL Server
- Dapper
- PdfSharpCore (generowanie dokumentów PDF)
- Dependency Injection
