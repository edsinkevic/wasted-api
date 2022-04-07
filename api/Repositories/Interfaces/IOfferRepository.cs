using LanguageExt;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Interfaces;
public interface IOfferRepository
{
    public Task<IEnumerable<Offer>> Get();
    public Task<IEnumerable<Offer>> GetByVendorName(string name);
    public Task<Either<List<string>, Offer>> Create(OfferCreate req);
    public Task<Either<List<string>, Offer>> Update(OfferUpdate req);
    public Task<Either<List<string>, Offer>> Delete(string id);

}