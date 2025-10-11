# Hausaufgabe - Best Practices für hochwertigen C#-Code mit .editorconfig 

## 1. Bestehende Datei als Vorlage nutzen

Nimm die existierende `.editorconfig` im Projekt-Root und committe sie ins Git.
Betrachte die als Basis und passe sie schrittweise an. 

## 2. Team-Commitment schaffen

Besprecht gemeinsam, welche Regeln Sinn machen und welche nicht:
- Welche Standards helfen wirklich?
- Wo gibt es unnötige Komplexität?
- Passen die Regeln zu eurer Projekt-Realität?

- Verstehe, dass Standards weniger Code-Reviews und bessere Wartbarkeit bedeuten
- Verstehe, warum jede Regel existiert
- Benutze die Regeln während Entwicklung (Integration in VS)

## 3. Projekte bereinigen - schrittweise 

**Phase 1: Warnungen beheben**
- bestehende Warnungen aufräumen

**Phase 2: Von suggestion/warning zu warning/error upgraden**
- Erst wenn Projekt sauber ist: erhöhe Regeln von `warning` auf `error` bzw. von `suggestion` auf `warning`
- Damit werden Verstöße nicht mehr übersehen
- eventuell: CI/CD Pipeline kann dann strikte Prüfungen erzwingen

**Phase 3: Sonar als zusatzlicher Checker**
- Starte mit kritischen Issues und dann anpassen
- für schnelles Feedback während der Entwicklung: SonarLint in der IDE nutzen


## 4. Team-Commitment aufbauen

- Feiert Meilensteine im Biergarden: "Projekt ist jetzt zu 100% konform!"
- Nutzt die `.editorconfig` nicht als Dogma oder Cargo Cult sondern als Team-Agreement und passe die auf Bedarf an




