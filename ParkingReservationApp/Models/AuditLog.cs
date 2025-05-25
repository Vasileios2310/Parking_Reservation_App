namespace ParkingReservationApp.Models;

public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; }
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Entity { get; set; }
    public string Description { get; set; }
}
