namespace WebApiAutores.DTOs.Authentication
{
    public class ResponseAuthenticacion
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
