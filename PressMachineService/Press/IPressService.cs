namespace PressMachineServices.Press
{
    using System.Collections.Generic;

    /// <summary>
    /// Логика для работы с пресом.
    /// </summary>
    public interface IPressService
    {
        /// <summary>
        /// Вернет данные о пресовании.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <returns>Список данных о пресовании.</returns>
        List<PressOperationData> GetPressOperationData(PressOperationDataFilter filter);

        /// <summary>
        /// Вернет прессы.
        /// </summary>
        /// <returns>Набор проессов.</returns>
        List<Press> GetPresses();
    }
}
