using LanguageExt;
using WastedApi.Models;
using WastedApi.Requests;

namespace Wasted.Interfaces;

public interface IMemberRepository
{
    public Task<Either<List<string>, Member>> Create(MemberSignup req, bool isAdmin);
    public Task<Either<List<string>, Member>> Get(UserLogin req);
    public Task<Either<List<string>, Member>> GetById(Guid id);
}