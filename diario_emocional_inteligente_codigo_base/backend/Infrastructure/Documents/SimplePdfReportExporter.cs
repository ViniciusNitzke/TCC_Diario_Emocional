using System.Globalization;
using System.Text;
using DiarioEmocional.Api.Application.Documents;
using DiarioEmocional.Api.Application.DTOs;

namespace DiarioEmocional.Api.Infrastructure.Documents;

public sealed class SimplePdfReportExporter : IPdfReportExporter
{
    public byte[] Export(string userName, WeeklyReportResponse report) =>
        Build("Relatorio emocional semanal", new[]
        {
            $"Usuario: {userName}",
            $"Total de registros: {report.Total}",
            $"Emocao mais frequente: {report.EmocaoMaisFrequente}",
            $"Intensidade media: {report.IntensidadeMedia}",
            $"Insight: {report.InsightPrincipal}",
            "Este relatorio e informativo e nao substitui acompanhamento profissional."
        });

    private static byte[] Build(string title, IEnumerable<string> lines)
    {
        var content = new StringBuilder();
        content.AppendLine("BT");
        content.AppendLine("/F1 18 Tf");
        content.AppendLine("50 780 Td");
        content.AppendLine($"({Escape(title)}) Tj");
        content.AppendLine("/F1 11 Tf");

        foreach (var line in lines)
        {
            content.AppendLine("0 -24 Td");
            content.AppendLine($"({Escape(RemoveAccents(line))}) Tj");
        }

        content.AppendLine("ET");
        var stream = Encoding.ASCII.GetBytes(content.ToString());
        var objects = new List<string>
        {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>",
            $"<< /Length {stream.Length} >>\nstream\n{Encoding.ASCII.GetString(stream)}endstream"
        };

        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.ASCII, leaveOpen: true);

        writer.WriteLine("%PDF-1.4");
        var offsets = new List<long> { 0 };
        for (var i = 0; i < objects.Count; i++)
        {
            writer.Flush();
            offsets.Add(ms.Position);
            writer.WriteLine($"{i + 1} 0 obj");
            writer.WriteLine(objects[i]);
            writer.WriteLine("endobj");
        }

        writer.Flush();
        var xref = ms.Position;
        writer.WriteLine("xref");
        writer.WriteLine($"0 {objects.Count + 1}");
        writer.WriteLine("0000000000 65535 f ");
        foreach (var offset in offsets.Skip(1))
            writer.WriteLine($"{offset:0000000000} 00000 n ");

        writer.WriteLine("trailer");
        writer.WriteLine($"<< /Size {objects.Count + 1} /Root 1 0 R >>");
        writer.WriteLine("startxref");
        writer.WriteLine(xref);
        writer.WriteLine("%%EOF");
        writer.Flush();

        return ms.ToArray();
    }

    private static string Escape(string text) =>
        text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
