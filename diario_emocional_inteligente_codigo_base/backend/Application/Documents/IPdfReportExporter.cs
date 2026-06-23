using DiarioEmocional.Api.Application.DTOs;

namespace DiarioEmocional.Api.Application.Documents;

public interface IPdfReportExporter
{
    byte[] Export(string userName, WeeklyReportResponse report);
}
