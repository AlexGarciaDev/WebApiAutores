using System.Runtime.InteropServices.ObjectiveC;

namespace WebApiAutores.Services
{
    public class WriteFiles : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nameFile = "archivo1.txt";
        private Timer timer;

        public WriteFiles(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork,null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            WriteFile("Proceso iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            WriteFile("Proceso finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            WriteFile("Proceso en ejecucion: "+DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        public void WriteFile(string message)
        {
            var path = $@"{env.ContentRootPath}\wwwroot\{nameFile}";

            using (StreamWriter write = new StreamWriter(path, append: true))
            {
                write.WriteLine(message);
            }
        }
    }
}
