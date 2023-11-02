namespace WebApiAutores.Services.ImplementServices
{
    public class TiposDeInyeccion
    {
    }

    public class ServicioTransient
    {
        public Guid guid = Guid.NewGuid();

    }

    public class ServicioScoped
    {
        public Guid guid = Guid.NewGuid();
    }

    public class ServicioSingleton
    {
        public Guid guid = Guid.NewGuid();
    }
}
