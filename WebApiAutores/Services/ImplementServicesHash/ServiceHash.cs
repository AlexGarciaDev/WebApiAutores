using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAutores.DTOs.Authentication;

namespace WebApiAutores.Services.ImplementServicesHash
{
    public class ServiceHash : IServiceHash
    {
        private ResultHash ResultHash;

        public ResultHash getHash()
        {
            return ResultHash;
        }
        public void BuilderHash(string PlaneText)
        {
            ResultHash = new ResultHash();
            var randomValue = new byte[16];

            using(var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(randomValue);
            }
            var hash = Hash(PlaneText, randomValue);

            ResultHash.Hash = hash;
            ResultHash.RandomValue= randomValue;
        }

        private string Hash(string PlaneText, byte[] sal) 
        {
            var derivedKey = KeyDerivation.Pbkdf2(password: PlaneText,
                    salt: sal, prf:KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 32
                );

            var hash = Convert.ToBase64String( derivedKey );

            return hash;
        }

        
    }
}
