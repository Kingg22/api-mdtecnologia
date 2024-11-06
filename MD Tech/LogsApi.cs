using NLog;

namespace MD_Tech
{
    internal class LogsApi
    {
        private readonly Logger LOGGER;

        public LogsApi()
        {
            LOGGER = LogManager.GetLogger("Anonimous Class");
        }

        public LogsApi(Type classType)
        {
            LOGGER = LogManager.GetLogger(classType.FullName);
        }

        public void Informacion(string mensaje)
        {
            this.LOGGER.Info(mensaje);
        }

        public void Advertencia(string mensaje)
        {
            this.LOGGER.Warn(mensaje);
        }

        public void Errores(string mensaje)
        {
            this.LOGGER.Error(mensaje);
        }

        public void Depuracion(string mensaje)
        {
            this.LOGGER.Debug(mensaje);
        }

        public void Excepciones(Exception exception, string mensaje)
        {
            this.LOGGER.Error(exception, mensaje);
        }
    }
}
