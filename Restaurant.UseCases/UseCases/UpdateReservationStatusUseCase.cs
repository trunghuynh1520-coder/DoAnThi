using Restaurant.CoreBusiness;
using Restaurant.UseCases.PluginInterfaces;

namespace Restaurant.UseCases;

public sealed class UpdateReservationStatusUseCase(IReservationRepository reservationRepository)
{
    public async Task ExecuteAsync(int reservationId, string status)
    {
        var reservation = await reservationRepository.GetReservationByIdAsync(reservationId)
            ?? throw new InvalidOperationException("Không tìm thấy lịch đặt bàn.");

        switch (status)
        {
            case Reservation.ArrivedStatus:
                reservation.MarkArrived();
                break;
            case Reservation.CancelledStatus:
                reservation.Cancel();
                break;
            case Reservation.NoShowStatus:
                reservation.MarkNoShow(DateTime.Now);
                break;
            default:
                throw new ArgumentException("Trạng thái đặt bàn không hợp lệ.", nameof(status));
        }

        await reservationRepository.UpdateReservationStatusAsync(reservation);
    }
}
