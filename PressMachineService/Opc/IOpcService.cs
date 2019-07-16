namespace PressMachineServices.Opc
{
    /// <summary>
    /// Логика работы с OPC сервером.
    /// </summary>
    public interface IOpcService
    {
        /// <summary>
        /// Задает OPC параметры для соединения.
        /// </summary>
        void ConfigureProcessor();

        /// <summary>
        /// Запускает опрос.
        /// </summary>
        void Run();

        /// <summary>
        /// Останавливает опрос.
        /// </summary>
        void Stop();
    }
}
