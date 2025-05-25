namespace ParkingReservationApp.DTOs;

public class ReservationPaymentDto
{
    public int ReservationId { get; set; }

    public string CreditCardNumber { get; set; }

    public string CardHolderName { get; set; }

    public string Expiration { get; set; } // format: MM/YY

    public string CVC { get; set; }

    public decimal Amount { get; set; } // optional if fixed, else calculate
}