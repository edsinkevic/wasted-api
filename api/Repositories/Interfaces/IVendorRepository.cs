
using LanguageExt;
using WastedApi;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Interfaces;
public interface IVendorRepository
{
    public Task<IEnumerable<Vendor>> Get();
    public Task<Either<List<string>, Vendor>> GetByName(string name);
    public Task<Vendor> Create(VendorCreate req);
}