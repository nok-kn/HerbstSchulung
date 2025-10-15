using HerbstSchulung.Hosting.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HerbstSchulung.WebApi.Controllers;

/// <summary>
/// Beispiel-Controller, demonstriert DI, Logging, ProblemDetails, Filter und Swagger-Annotations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GreetingController(IGreeter greeter, IClock clock, ILogger<GreetingController> logger) : ControllerBase
{
    /// <summary>
    /// Gibt eine Begrüßung zurück. Nutzt den IGreeter-Service und protokolliert die Anfrage.
    /// </summary>
    /// <param name="name">Name der Person</param>
    /// <returns>Begrüßungstext</returns>
    /// <response code="200">Erfolgreiche Antwort</response>
    /// <response code="400">Ungültige Eingabe</response>
    [HttpGet("say-hello-async")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<string> SayHelloAsync(string name)
    {
        var greeting = await greeter.GreetAsync(name, HttpContext.RequestAborted);
        return greeting;

        // Empfehlungen:
        // - leichtgewichtige Controller-Methoden (keine Business-Logik)
        // - cross-cutting concerns (Logging, Timing, Exception Handling etc.) in Filter auslagern
        // - asynchrone Methoden bevorzugen
        // - HttpContext.RequestAborted als Quelle für CancellationToken benutzen
        // - xml Kommentare für Swagger und IntelliSense nutzen
    }

    /// <summary>
    /// Gibt eine Begrüßung zurück (synchron und validierung). 
    /// </summary>
    /// <param name="name">Name der Person</param>
    /// <returns>Begrüßungstext</returns>
    /// <response code="200">Erfolgreiche Antwort</response>
    /// <response code="400">Ungültige Eingabe</response>
    [HttpGet("say-hello")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<string> SayHello([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // Durch ProblemDetails werden strukturierte Fehler zurückgegeben
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(name)] = new[] { "Name darf nicht leer sein." }
            }));
        }

        var now = clock.UtcNow;
        logger.LogInformation("Begrüßung angefragt für {Name} um {Time}", name, now);

        var greeting = greeter.Greet(name);
        return Ok(greeting);
    }


    /// <summary>
    /// Beispiel-Endpunkt, der absichtlich einen Fehler erzeugt, um ProblemDetails zu zeigen.
    /// </summary>
    [HttpGet("server-error")]
    public IActionResult Fail()
    {
        throw new InvalidOperationException("Absichtlich ausgelöster Fehler für Demo");
    }

    [HttpGet("not-found-error")]
    public IActionResult GetNotFound()
    {
        throw new KeyNotFoundException("Der angeforderte Ressource wurde nicht gefunden.");
    }

}
