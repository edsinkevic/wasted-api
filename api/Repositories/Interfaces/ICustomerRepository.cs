using LanguageExt;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Interfaces;

public interface ICustomerRepository
{
    public Task<Either<List<string>, Customer>> Create(CustomerSignup req);
    public Task<Either<List<string>, Customer>> Get(UserLogin req);
    public Task<Either<List<string>, Customer>> GetById(Guid id);
}