using LanguageExt;
using WastedApi;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Interfaces;
public interface IOfferEntryRepository
{
    public Task<IEnumerable<OfferEntry>> Get();
    public Task<Either<List<string>, OfferEntry>> Create(OfferEntryCreate req);
    public Task<Either<List<string>, OfferEntry>> Update(OfferEntryUpdate req);
    public Task<int> Clean();

}