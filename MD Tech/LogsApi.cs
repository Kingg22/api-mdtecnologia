using NLog;

namespace MD_Tech
{
    public class LogsApi<T>
    {
        private readonly Logger LOGGER = LogManager.GetLogger(typeof(T).FullName);

        public void Informacion(string mensaje)
        {
            mensaje = mensaje.Replace(Environment.NewLine, string.Empty);
            LOGGER.Info(mensaje);
        }

        public void Advertencia(string mensaje)
        {
            mensaje = mensaje.Replace(Environment.NewLine, string.Empty);
            LOGGER.Warn(mensaje);
        }

        public void Errores(string mensaje)
        {
            mensaje = mensaje.Replace(Environment.NewLine, string.Empty);
            LOGGER.Error(mensaje);
        }

        public void Depuracion(string mensaje)
        {
            mensaje = mensaje.Replace(Environment.NewLine, string.Empty);
            LOGGER.Debug(mensaje);
        }

        public void Excepciones(Exception exception, string mensaje)
        {
            mensaje = mensaje.Replace(Environment.NewLine, string.Empty);
            LOGGER.Error(exception, mensaje);
        }
    }
}
