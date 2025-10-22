# Die Umstellung von Newtonsoft.Json (Json.NET) auf System.Text.Json:


| Nr. | Thema / Problem | Ursache / Unterschied | Lösung / Workaround |
|-----|------------------|-----------------------|---------------------|
| 1 | Property-Namen anders (camelCase vs PascalCase) | System.Text.Json nutzt standardmäßig camelCase | `PropertyNamingPolicy = null` oder `JsonNamingPolicy.CamelCase` explizit setzen |
| 2 | Private Setter / Felder werden ignoriert | Nur public Getter/Setter werden serialisiert | `[JsonInclude]` verwenden oder öffentliche Setter hinzufügen |
| 3 | Konstruktorbindung (Immutable Types) | Nur passende Parameter werden gebunden | `[JsonConstructor]` verwenden, Namen exakt abstimmen |
| 4 | `[JsonProperty]` nicht erkannt | Attribut heißt jetzt `[JsonPropertyName]` | Alle `[JsonProperty]` → `[JsonPropertyName]` ersetzen |
| 5 | Unterschiedliche Converter-API | JsonConverter-API neu (Read/Write statt ReadJson/WriteJson) | Converter neu schreiben, auf `JsonConverter<T>`-Basis |
| 6 | TypeNameHandling / Referenzen fehlen | Keine automatische TypeInfo/ReferenceHandling | `ReferenceHandler.Preserve` oder `[JsonDerivedType]` nutzen |
| 7 | Null-Werte werden ausgelassen | Neues Default-Verhalten (`WhenWritingNull`) | `DefaultIgnoreCondition = JsonIgnoreCondition.Never` |
| 8 | `JObject` / `JToken` fehlen | Kein dynamisches JSON-Modell | `JsonDocument`, `JsonNode` oder `JsonObject` verwenden |
| 9 | Enums als Zahlen statt Strings | Standard: numerisch | `JsonStringEnumConverter` registrieren |
| 10 | `[DataMember]` wird ignoriert | System.Text.Json beachtet DataContract-Attribute nicht | `[JsonPropertyName]` statt `[DataMember(Name=...)]` |
| 11 | `[IgnoreDataMember]` funktioniert nicht | Wird ignoriert | `[JsonIgnore]` verwenden |
| 12 | Zahlen als String im JSON | `"12.34"` passt nicht zu `double` | `JsonNumberHandling.AllowReadingFromString` oder eigener Converter |
| 13 | `IsRequired` / `EmitDefaultValue` fehlen | DataMember-Features fehlen | Validierung manuell oder per `[Required]` durchführen |
