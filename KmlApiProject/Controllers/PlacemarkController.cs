using KmlApiProject.Models;
using KmlApiProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace KmlApiProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlacemarkController : ControllerBase
{
    private readonly KmlDataService _kmlDataService;
    private readonly KmlExportService _kmlExportService;

    public PlacemarkController(KmlDataService kmlDataService, KmlExportService kmlExportService)
    {
        _kmlDataService = kmlDataService;
        _kmlExportService = kmlExportService;
    }

    [HttpPost("export")]
    public IActionResult ExportKml([FromBody] FilterModel filters)
    {
        if (!IsValidFilter(filters))
            return BadRequest("Filtros fornecidos são inválidos.");

        var filteredData = _kmlDataService.FilterData(
            filters.Cliente,
            filters.Situacao,
            filters.Bairro,
            filters.Referencia,
            filters.RuaCruzamento
        );
        var exportedFilePath = _kmlExportService.ExportToKml(filteredData);

        return PhysicalFile(
            exportedFilePath,
            "application/vnd.google-earth.kml+xml",
            "ExportedData.kml"
        );
    }

    [HttpGet]
    public IActionResult GetFilteredData([FromQuery] FilterModel filters)
    {
        if (!IsValidFilter(filters))
            return BadRequest("Filtros fornecidos são inválidos.");

        var filteredData = _kmlDataService.FilterData(
            filters.Cliente,
            filters.Situacao,
            filters.Bairro,
            filters.Referencia,
            filters.RuaCruzamento
        );
        return Ok(filteredData);
    }

    [HttpGet("filters")]
    public IActionResult GetAvailableFilters()
    {
        var clienteValues = _kmlDataService.GetUniqueValuesForFilter("cliente");
        var situacaoValues = _kmlDataService.GetUniqueValuesForFilter("situacao");
        var bairroValues = _kmlDataService.GetUniqueValuesForFilter("bairro");

        var filters = new
        {
            Clientes = clienteValues,
            Situacoes = situacaoValues,
            Bairros = bairroValues,
        };

        return Ok(filters);
    }

    private bool IsValidFilter(FilterModel filters)
    {
        if (filters == null)
            return false;

        if (filters.Referencia?.Length < 3 || filters.RuaCruzamento?.Length < 3)
            return false;

        var validClientes = _kmlDataService.GetUniqueValuesForFilter("cliente");
        var validSituacoes = _kmlDataService.GetUniqueValuesForFilter("situacao");
        var validBairros = _kmlDataService.GetUniqueValuesForFilter("bairro");

        if (!string.IsNullOrEmpty(filters.Cliente) && !validClientes.Contains(filters.Cliente))
            return false;

        if (!string.IsNullOrEmpty(filters.Situacao) && !validSituacoes.Contains(filters.Situacao))
            return false;

        if (!string.IsNullOrEmpty(filters.Bairro) && !validBairros.Contains(filters.Bairro))
            return false;

        return true;
    }
}
