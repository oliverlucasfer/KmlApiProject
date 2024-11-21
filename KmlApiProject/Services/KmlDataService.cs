using SharpKml.Base;
using SharpKml.Dom;

namespace KmlApiProject.Services;

public class KmlDataService
{
    private readonly string _kmlFilePath;
    private List<Placemark> _placemarks;

    public KmlDataService(string kmlFilePath)
    {
        _kmlFilePath = kmlFilePath;
        _placemarks = [];
        LoadKmlData();
    }

    private void LoadKmlData()
    {
        using var stream = new FileStream(_kmlFilePath, FileMode.Open);
        var parser = new Parser();
        parser.Parse(stream);

        if (!(parser.Root is not Kml kml || kml.Feature is not Document document))
        {
            _placemarks = document.Features.OfType<Placemark>().ToList();
        }
    }

    public IEnumerable<Placemark> FilterData(
        string cliente,
        string situacao,
        string bairro,
        string referencia,
        string ruaCruzamento
    )
    {
        var query = _placemarks.AsQueryable();

        if (!string.IsNullOrEmpty(cliente))
            query = query.Where(p => p.Name.Contains(cliente, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(situacao))
            query = query.Where(p =>
                p.Description.Text.Contains(situacao, StringComparison.OrdinalIgnoreCase)
            );

        if (!string.IsNullOrEmpty(bairro))
            query = query.Where(p =>
                p.Address.Contains(bairro, StringComparison.OrdinalIgnoreCase)
            );

        if (!string.IsNullOrEmpty(referencia) && referencia.Length >= 3)
            query = query.Where(p =>
                p.Name.Contains(referencia, StringComparison.OrdinalIgnoreCase)
            );

        if (!string.IsNullOrEmpty(ruaCruzamento) && ruaCruzamento.Length >= 3)
            query = query.Where(p =>
                p.Address.Contains(ruaCruzamento, StringComparison.OrdinalIgnoreCase)
            );

        return [.. query];
    }

    public IEnumerable<string> GetUniqueValuesForFilter(string field)
    {
        return field.ToLower() switch
        {
            "cliente" => _placemarks.Select(p => p.Name).Distinct(),
            "situacao" => _placemarks.Select(p => p.Description?.Text).Distinct(),
            "bairro" => _placemarks.Select(p => p.Address).Distinct(),
            _ => Enumerable.Empty<string>(),
        };
    }
}
