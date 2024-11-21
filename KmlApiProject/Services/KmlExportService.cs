using SharpKml.Base;
using SharpKml.Dom;

namespace KmlApiProject.Services;

public class KmlExportService
{
    public string ExportToKml(IEnumerable<Placemark> placemarks)
    {
        var kml = new Kml();
        var document = new Document();

        foreach (var placemark in placemarks)
        {
            document.AddFeature(placemark);
        }

        kml.Feature = document;

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ExportedData.kml");

        var serializer = new Serializer();
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            serializer.Serialize(kml, stream);
        }

        return filePath;
    }
}
