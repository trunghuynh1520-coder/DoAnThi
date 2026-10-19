using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Restaurant.UseCases;

internal static class ExcelWorkbookWriter
{
    private static readonly XNamespace Main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private static readonly XNamespace Relationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
    private static readonly XNamespace PackageRelationships = "http://schemas.openxmlformats.org/package/2006/relationships";
    private static readonly XNamespace ContentTypes = "http://schemas.openxmlformats.org/package/2006/content-types";

    public static byte[] Create(IEnumerable<IReadOnlyList<object?>> rows)
    {
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
        {
            AddXml(archive, "[Content_Types].xml", new XDocument(
                new XElement(ContentTypes + "Types",
                    new XElement(ContentTypes + "Default", new XAttribute("Extension", "rels"), new XAttribute("ContentType", "application/vnd.openxmlformats-package.relationships+xml")),
                    new XElement(ContentTypes + "Default", new XAttribute("Extension", "xml"), new XAttribute("ContentType", "application/xml")),
                    new XElement(ContentTypes + "Override", new XAttribute("PartName", "/xl/workbook.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml")),
                    new XElement(ContentTypes + "Override", new XAttribute("PartName", "/xl/worksheets/sheet1.xml"), new XAttribute("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml")))));
            AddXml(archive, "_rels/.rels", new XDocument(
                new XElement(PackageRelationships + "Relationships",
                    new XElement(PackageRelationships + "Relationship", new XAttribute("Id", "rId1"), new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"), new XAttribute("Target", "xl/workbook.xml")))));
            AddXml(archive, "xl/workbook.xml", new XDocument(
                new XElement(Main + "workbook", new XAttribute(XNamespace.Xmlns + "r", Relationships),
                    new XElement(Main + "sheets", new XElement(Main + "sheet", new XAttribute("name", "HoaDon"), new XAttribute("sheetId", 1), new XAttribute(Relationships + "id", "rId1"))))));
            AddXml(archive, "xl/_rels/workbook.xml.rels", new XDocument(
                new XElement(PackageRelationships + "Relationships",
                    new XElement(PackageRelationships + "Relationship", new XAttribute("Id", "rId1"), new XAttribute("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"), new XAttribute("Target", "worksheets/sheet1.xml")))));

            var sheetRows = rows.Select((row, rowIndex) => new XElement(Main + "row",
                new XAttribute("r", rowIndex + 1),
                row.Select((value, columnIndex) => CreateCell(rowIndex + 1, columnIndex + 1, value))));
            AddXml(archive, "xl/worksheets/sheet1.xml", new XDocument(
                new XElement(Main + "worksheet", new XElement(Main + "sheetData", sheetRows))));
        }
        return output.ToArray();
    }

    private static XElement CreateCell(int row, int column, object? value)
    {
        var reference = ColumnName(column) + row.ToString(CultureInfo.InvariantCulture);
        if (value is byte or short or int or long or float or double or decimal)
            return new XElement(Main + "c", new XAttribute("r", reference), new XAttribute("t", "n"),
                new XElement(Main + "v", Convert.ToString(value, CultureInfo.InvariantCulture)));

        return new XElement(Main + "c", new XAttribute("r", reference), new XAttribute("t", "inlineStr"),
            new XElement(Main + "is", new XElement(Main + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), value?.ToString() ?? string.Empty)));
    }

    private static string ColumnName(int index)
    {
        var name = string.Empty;
        while (index > 0)
        {
            index--;
            name = (char)('A' + index % 26) + name;
            index /= 26;
        }
        return name;
    }

    private static void AddXml(ZipArchive archive, string path, XDocument document)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
        using var stream = entry.Open();
        document.Save(stream);
    }
}
