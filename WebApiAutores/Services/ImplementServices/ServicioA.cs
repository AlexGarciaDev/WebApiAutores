using WebApiAutores.services;

namespace WebApiAutores.Services.ImplementServices
{
    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> logger;

        public ServicioA(ILogger<ServicioA> logger)
        {
            this.logger = logger;
        }

        public void RealizarTarea()
        {
            logger.LogInformation("Log lanzado desde el servicio A en el metodo realizar tarea");
        }
    }
}
