# Interne Common Pakete  - Diskussion

Die Idee, ein oder mehrere **gemeinsame NuGet-Pakete**  für alle Projekte im Unternehmen/Abteilung zu erstellen klingt auf den ersten Blick sinnvoll:  
> *"Don’t repeat yourself", Konsistenz, zentrale Wartung.*
> *"Alle Entwickler benutzen endlich ein Standart*"
 
Doch in der Praxis birgt dieser Ansatz erhebliche Risiken und Problemen – besonders wenn Paketie **zu breit gefasst** oder **zu eng gekoppelt** ist.

---

## Hauptargumente gegen monolithischen Common Paketen

### 1. **Zu enge Kopplung (Tight Coupling)**
- Alle Anwendungen hängen von einzigen Paketen ab
- Ein einziger Bug kann **mehrere Systeme gleichzeitig lahmlegen**

### 2. **Versionierung wird zum Albtraum**
- Teams nutzen unterschiedliche .NET-Versionen, Frameworks oder Abhängigkeiten
- Pakete werden oft nicht semantisch versioniert - 10.2.3 => 10.2.4 mit Breaking Change
- Oder veraltete/unnötige Abhängigkeiten → Sicherheits- und Wartungsrisiken

### 3. **Dependency Bloat**
- Projekte ziehen Code mit, den sie **gar nicht benötigen** (z. B. Auth-Logik in einem Batch-Tool).
- Verstößt gegen das **Interface Segregation Principle** (SOLID):  
  > *„Clients should not be forced to depend on interfaces they do not use.“*

### 4. **Langsame Innovation & Release-Zyklen**
- Teams müssen auf Releases des Common-Pakets warten.
- Das Common Zeug wird zum **Bottleneck**.
- Angst vor Breaking Changes führt zu **veraltetem Code**.
- Der "dead code" in common libs ist schwierig zu endecken und entfernen
- Wenn ein Team eine bessere Lösung finden möchte, ist der Ausstieg aufwendig – wegen tiefer Integration.
 
### 5. **Unklare Verantwortlichkeit**
- Wer wartet das Paket? Wer testet es? Wer entscheidet über neue Features?
- Häufig entsteht **„Shared Nothing“-Ownership**: Niemand fühlt sich wirklich verantwortlich.

---

## Bessere Alternativen

### **Keine oder Standard oder eigene Pakete beworzugen
- Brauchst du wirklich ein zusätzliches Paket?
- Microsoft.Extensions.* (Logging, Configuration, Hosting, DependencyInjection, Options, Caching, HealthChecks, etc)
- keine eigene "multi-team" Frameworks bauen
- YAGNI: zuerst eine eigene Lösung nur in deinem Projekt benutzen, nicht denken, das könnte ja mal jemand anderes brauchen

### **Mehrere kleine, fokussierte Pakete**
Statt `MyCompany.Common` → besser:
- `MyCompany.Auth`
- `MyCompany.Logging`
- `MyCompany.Configuration`
- `MyCompany.Validation`
- `MyCompany.Resources.Icons`
 
Jedes Paket hat:
- Einen klaren Zweck
- Unabhängige Versionierung
- Keine unnötigen Abhängigkeiten
- Klare Ownership & SLAs
- Semantische Versionierung 
- Automatisierte Tests & CI/CD
- Keine Buisness Logik 

### **Beipiel SLA**
- Reaktionszeit bei Bugs - Kritische Bugs (z. B. Sicherheitslücken) werden innerhalb von 2 Werktagen behoben
- Release-Zyklen - Neue Minor-Versionen erscheinen maximal alle 4 Wochen. Patch-Releases bei Bedarf
- Breaking Changes - Breaking Changes werden mindestens 2 Monate im Voraus angekündigt und sind nur in Major-Versionen erlaubt
- Support-Dauer - Die letzten zwei Major-Versionen werden aktiv unterstützt
- Dokumentation - Jede Version wird mit Changelog, Migrationsanleitung und API-Dokumentation veröffentlicht


### **Architektur Regeln und Automation statt Code-Enforcement**  
- Common Basisklassen für mehrehre Solutions vermeiden 
- "Favor composition over inheritance"
- Copy und Paste Code bewusst akzeptieren
- Automatisierte Code Qualitätschecks (.editorconfig), ein AI Agent im CI/CD Pipeline, Code Reviews (Human)
- Gemeinsame Architektur Prinzipien – nicht gemeinsamer Code. z.B.:
  - API-First-Design - Alle externen Schnittstellen (HTTP, Messaging) werden als Vertrag definiert. HTTP-APIs: OpenAPI/Swagger 
  - Infrastruktur als Code (IaC) - Alle Umgebungen (Dev, Test, Prod) werden deklarativ und versioniert bereitgestellt
  - UI unf UX Guideline - geimensame Resources (icons, WPF Styles, Tailwind CSS Styles etc) - ja.
    Profesionalle Biblotheken (z.B. DevExpress WPF Themes) statt individualen Komponenten.
    Design System Owner Rolle definieren – Designer/in oder Dev mit Verantwortung für Konsistenz. UI und UX Review auch machen



## Hausaufgabe - Überlege, was macht ihr mit Zentralen Komponenten?
