using WebApiAutores.DTOs.Authentication;

namespace WebApiAutores.Services
{
    public interface IServiceHash
    {
        void BuilderHash(string PlaneText);

        ResultHash getHash();
    }
}
