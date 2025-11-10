using System;

namespace API.TrenAmbiental.DTO.DomainObjects;

public static class DataHoraAtual
{
    public static DateTime ObterDataHoraServidorWindows()
    {
        return TimeZoneInfo.ConvertTime(DateTime.Now,
            TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }

    public static DateTime ObterDataHoraServidorLinux()
    {
        throw new NotImplementedException();
    }
}