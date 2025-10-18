using HerbstSchulung.Hosting.Abstractions.Deskriptor;
using Microsoft.AspNetCore.Mvc;

namespace HerbstSchulung.WebApi.Controllers;

/// <summary>
/// API-Controller für  Deskriptoren.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DeskriptorController(IDeskriptorService service) : ControllerBase
{
    /// <summary>
    /// Liefert alle Deskriptoren.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DeskriptorDto>), StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<DeskriptorDto>> GetAllAsync() =>
        await service.GetAllAsync(HttpContext.RequestAborted);

    /// <summary>
    /// Liefert einen Deskriptor per Id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DeskriptorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<DeskriptorDto?> GetByIdAsync(int id) =>
        await service.GetByIdAsync(id, HttpContext.RequestAborted);

    /// <summary>
    /// Legt einen neuen Deskriptor an.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeskriptorDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DeskriptorDto>> CreateAsync([FromBody] DeskriptorDto dto)
    {
        var created = await service.CreateAsync(dto, HttpContext.RequestAborted);
        return CreatedAtAction(nameof(CreateAsync), new { id = created.Id }, created);
    }

    /// <summary>
    /// Aktualisiert einen vorhandenen Deskriptor.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<bool> UpdateAsync(int id, [FromBody] DeskriptorDto dto) =>
        await service.UpdateAsync(dto, HttpContext.RequestAborted);

    /// <summary>
    /// Löscht einen Deskriptor.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<bool> DeleteAsync(int id) =>
        await service.DeleteAsync(id, HttpContext.RequestAborted);
}
