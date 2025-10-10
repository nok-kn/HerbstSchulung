# Kurzüberblick C#-Features der letzten Jahre - C# 6 bis C# 12   

| Feature | Ab C#-Version | Beispiel |
|---|---|---|
| Null-Conditional Operator | 6 | var len = person?.Name?.Length; |
| String Interpolation | 6 | var msg = $"Hallo {name}"; |
| nameof | 6 | throw new ArgumentNullException(nameof(name)); |
| Expression-bodied Members | 6 | int Len => Name.Length; |
| Tuples & Deconstruction | 7 | var (x, y) = GetPoint(); |
| out var | 7 | if (int.TryParse(s, out var n)) { /* ... */ } |
| Pattern Matching (is) | 7 | if (obj is string s && s.Length > 0) { } |
| Local Functions | 7 | int Add(int a, int b) { return a + b; } |
| Switch Expressions | 8 | var text = kind switch { 1 => "Eins", _ => "?" }; |
| Using Declarations | 8 | using var sw = new StreamWriter("a.txt"); |
| Async IEnumrables | 8 | await foreach (var x in stream) { /* ... */ } |
| Nullable Reference Types | 8 | string? n = null; |
| Records (Referenztypen) | 9 | public record Person(string Name); |
| Init-only Setter | 9 | public string Name { get; init; } |
| Target-typed new | 9 | List<int> xs = new(); |
| with-Ausdrücke (Records) | 9 | var p2 = p with { Name = "B" }; |
| Top-level Statements | 9 | Console.WriteLine("Hi"); |
| File-scoped Namespaces | 10 | namespace Demo; class C { } |
| Global using | 10 | global using System.Linq; |
| Record structs | 10 | public readonly record struct Point(int X, int Y); |
| required Members | 11 | class P { public required string Name { get; init; } } |
| Raw String Literals | 11 | var json = """{ ""a"": 1 }"""; |
| List Patterns | 11 | if (arr is [1, _, 3]) { /* ... */ } |
| UTF-8 String-Literale | 11 | ReadOnlySpan<byte> data = "Hi"u8; |
| Generic Math (INumber<T>) | 11 | T Sum<T>(T a, T b) where T : INumber<T> => a + b; |
| Primary Constructors | 12 | class Point(int x, int y) { public int X = x; } |
| Collection Expressions | 12 | int[] xs = [1, 2, 3]; var ys = new List<int> [.. xs, 4]; |
| Alias für beliebige Typen | 12 | using IntList = List<int>; |
| Default-Parameter in Lambdas | 12 | var f = (int x = 42) => x; |
| params Collections | 13 | `void M(params ReadOnlySpan<int> xs) { }` |
| Neuer Lock-Typ | 13 | `Lock myLock = new(); lock (myLock) { }` |
| \e Escape Sequence | 13 | `char esc = '\e'; // statt \u001b` |
| Implicit Index in Initializer | 13 | `var arr = new int[10] { [^1] = 99 };` |
| ref struct Interfaces | 13 | `ref struct MySpan : IDisposable { }` |
| allows ref struct Constraint | 13 | `void M<T>() where T : allows ref struct { }` |
| Partial Properties | 13 | `public partial class C { public partial string Name { get; set; } }` |
| Overload Resolution Priority | 13 | `[OverloadResolutionPriority(1)] void M(int x) { }` |
| field Keyword (Preview) | 13 | `public int P { get => field; set => field = value; }` |
| Extension Types | 14 | `implicit extension MyExt for string { public int Len => this.Length; }` |
| Extended nameof | 14 | `var name = nameof(person.Name.Length);` |
| Implicit Span Conversions | 14 | `ReadOnlySpan<char> span = "text";` |
| Lambda Improvements | 14 | `var f = (ref int x) => x++;` |
| Dictionary Expressions | 14 | `var dict = ["key": value, "key2": value2];` |
| Partial Constructors | 14 | `public partial class C { partial C(); }` |


Weitere Details: Microsoft-Dokumentation zu C#-Sprachfeatures (learn.microsoft.com).