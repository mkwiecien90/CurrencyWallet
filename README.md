Działanie API (DDD + CQRS):

Endpoint do pobierania dostepnych walut z cache który aktualizuje co1h zapytanie do api NBP
![image](https://github.com/user-attachments/assets/81f054c9-254d-4f57-b5eb-2aebb86f1dc9)

Endpoint do tworzenia portfela, któremu nadajemy nazwę, jego startowa zawrtość to 0:
![image](https://github.com/user-attachments/assets/15e516e6-3940-432d-b737-8c2b15bb50aa)

Endpoint do pobierania portfolio portfeli wraz z ich zawartością:
![image](https://github.com/user-attachments/assets/8265c415-a19f-426b-b9ca-77ab44911e7b)

Endpoint do pobierania informacji dla pojedyńczego portfela:
![image](https://github.com/user-attachments/assets/826a79e8-cfc5-4431-ad41-fadfa5deccec)

Endpoint do usuwania danego portfela z portfolio:
![image](https://github.com/user-attachments/assets/3436a13b-cfbe-47aa-8b67-54429673e061)

Endpoint do wpłacania środków na portfel, w dostepnych walutach:
![image](https://github.com/user-attachments/assets/35ed0212-e374-4096-90f9-d08b5980f206)

Endpoint do wypłacania środków z portfela:
![image](https://github.com/user-attachments/assets/84561d94-2bf9-477a-9c98-e2cd0429901f)

Endpoint do konwersji z jednej waluty na inną w obrębie portfela z spośród dostępnych walut:
![image](https://github.com/user-attachments/assets/b1dce333-8ca3-44c2-b8f1-a26c0f076c3a)


Dodatkowe ulepszenia, które mozna zaplanowac

**1. Wdrożenie aplikacji w platformie Azure:**
Aplikacja może być wdrożona na platformie Azure z wykorzystaniem kilku usług, które zapewnią skalowalność, dostępność oraz zarządzanie w chmurze.

**Proponowane usługi:**

**Azure App Service:**
Proste w użyciu, zarządzane środowisko dla aplikacji webowych i API.
Automatyczne skalowanie i wysoką dostępność.
Integracja z GitHub lub Azure DevOps do CI/CD.

**Azure Kubernetes Service (AKS):**
W przypadku, gdy aplikacja jest kontenerowana, AKS może zapewnić elastyczne zarządzanie i skalowanie usług.
Dzięki Kubernetes można łatwo wdrażać aplikację w wielu środowiskach i zapewnić ciągłą dostępność oraz automatyczne skalowanie.

**Azure Functions:**
Dla specyficznych operacji asynchronicznych lub zadań rozdzielonych na małe jednostki, które można wykonać bez konieczności utrzymywania dedykowanych serwerów, warto rozważyć Azure Functions. Funkcje mogą obsługiwać takie zadania jak obliczenia kursów walutowych, przetwarzanie płatności, czy integracje z zewnętrznymi systemami.

**Azure SQL Database / Cosmos DB:**
Dla aplikacji opartej na SQL można użyć Azure SQL Database – skalowalna, zarządzana baza danych.
Azure Cosmos DB – jeśli aplikacja korzysta z dokumentów lub danych o wysokiej dostępności i opóźnieniach w czasie rzeczywistym, np. w przypadku przetwarzania dużych ilości danych o transakcjach.

**Azure Service Bus / Event Grid:**
Do obsługi komunikacji między różnymi częściami systemu i przesyłania zdarzeń między mikroserwisami.
Azure Service Bus może być użyty do przesyłania wiadomości między aplikacjami, zarządzania kolejkami wiadomości i zapewnienia asynchronicznego przetwarzania transakcji.

**2. Zapewnienie wydajności przy dużej liczbie transakcji:**
W przypadku aplikacji, która będzie obsługiwała dużą liczbę transakcji, istnieje kilka podejść do zapewnienia wydajności.

**Proponowane strategie:**

**Skalowanie poziome (horizontal scaling):**
Można wdrożyć aplikację na wielu instancjach w Azure, wykorzystując App Service Plan z możliwością automatycznego skalowania (AutoScale), by dostosować się do zmieniającego się obciążenia.
Azure Kubernetes Service z automatycznym skalowaniem pozwoli na dynamiczne skalowanie aplikacji w zależności od zapotrzebowania.

**Bazy danych z replikacją i shardowaniem:**
Azure SQL Database może oferować funkcje replikacji geograficznej oraz automatycznego skalowania, co umożliwia wysoką dostępność i równomierne rozłożenie obciążenia.
Cosmos DB z opcją sharding'u umożliwi podział danych na różne partie (np. według waluty, regionu itp.), co pozwoli na efektywne zarządzanie dużymi wolumenami danych.

**Caching:**
Azure Cache for Redis może być użyteczne w celu przechowywania wyników często używanych zapytań, takich jak kursy walutowe, transakcje, lub dane dotyczące stanu konta użytkownika. Dzięki temu aplikacja nie musi wykonywać kosztownych zapytań do bazy danych przy każdym żądaniu.

**Asynchroniczność i kolejki:**
Aby odciążyć bazę danych, transakcje mogą być przetwarzane asynchronicznie za pomocą Azure Service Bus lub Azure Queue Storage. Zamiast natychmiastowego wykonywania wszystkich operacji, mogą one trafić do kolejki, gdzie są przetwarzane w tle.

**3. Integracja z systemami zewnętrznymi i zapewnienie, że transakcja jest zatwierdzona dopiero po zaakceptowaniu przez system zewnętrzny:**
Aby zapewnić, że transakcje w naszym systemie będą "zatwierdzone" dopiero po zaakceptowaniu przez system zewnętrzny, można rozważyć następujące podejście:

**Saga Pattern / Orkiestracja procesów:**
Rozważenie implementacji sagi (saga pattern) w systemie. Saga jest sposobem zarządzania transakcjami rozproszonymi w systemach, który zapewnia, że jeśli operacja w jednym systemie zakończy się niepowodzeniem, inne operacje są odwracane.
Można to zaimplementować przy użyciu Azure Service Bus i kolejki, gdzie system zewnętrzny będzie stanowił jeden z kroków w procesie zatwierdzania transakcji. Dopóki system zewnętrzny nie potwierdzi transakcji, operacja jest w stanie oczekiwania.

**Transactional Outbox Pattern:**
Aby zapewnić, że stan transakcji w systemie nie jest zmieniany, zanim nie zostanie potwierdzony przez system zewnętrzny, można użyć wzorca Transactional Outbox.
Po zapisaniu transakcji w bazie danych, wiadomość o tej transakcji trafia do specjalnej tabeli (outbox), która jest monitorowana przez usługę (np. Azure Functions), odpowiedzialną za wysyłanie komunikatów do systemu zewnętrznego. Dopóki transakcja nie zostanie zatwierdzona przez system zewnętrzny, status transakcji w aplikacji nie jest zmieniany.

**CQRS (Command Query Responsibility Segregation) i Event Sourcing:**
CQRS pozwala oddzielić zapisywanie danych (komendy) od ich odczytu (zapytania), co umożliwia łatwiejsze zarządzanie transakcjami.
Event Sourcing może być wykorzystane do zapisania stanu systemu jako sekwencji zdarzeń, dzięki czemu łatwiej zarządzać historią transakcji i odtwarzać stany. Zdarzenia mogą obejmować m.in. "transakcja wysłana do systemu zewnętrznego" oraz "transakcja zatwierdzona przez system zewnętrzny".

**Potwierdzenie za pomocą webhooków:**
Jeśli system zewnętrzny wspiera webhooki, można skonfigurować aplikację, aby nasłuchiwała na te webhooki, które potwierdzają zaakceptowanie transakcji. Dopóki webhook nie potwierdzi statusu transakcji, jej status w systemie pozostaje w stanie "oczekujący".

**Podsumowanie:**
Wdrożenie w Azure: Można wykorzystać App Service, AKS, Azure Functions, Azure SQL Database lub Cosmos DB dla skalowalności i elastyczności.
Wydajność: Zapewnienie wydajności poprzez skalowanie poziome, Caching (Redis), kolejki asynchroniczne oraz replikację bazy danych.
Integracja z systemami zewnętrznymi: Implementacja wzorców takich jak Saga Pattern, Transactional Outbox Pattern, CQRS i Event Sourcing w celu zapewnienia, że transakcje są zatwierdzane dopiero po akceptacji przez systemy zewnętrzne.
