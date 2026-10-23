namespace Application.Common;

public record Slot(DateTime Date, TimeOnly StartTime, TimeOnly EndTime, int AvailabilityId);
