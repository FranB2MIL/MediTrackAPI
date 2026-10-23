namespace Application.Common;

public static class ArgentinaTime
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime Now() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    // IANA (Linux / Windows con ICU) → id de Windows → UTC-3 fijo (Argentina no tiene horario de verano)
    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "America/Argentina/Buenos_Aires", "Argentina Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (Exception)
            {
                // Probar el siguiente id
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone("ART", TimeSpan.FromHours(-3), "Argentina (UTC-3)", "Argentina (UTC-3)");
    }
}
