using LanguageExt;
using WastedApi;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Interfaces;
public interface IReservationRepository
{
    public Task<Either<List<string>, Reservation>> GetByCustomerId(string id);
    public Task<Either<List<string>, Reservation>> Create(ReservationCreate req);

}