namespace PressMachine
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using PressMachineServices.Database;
    using PressMachineServices.Opc;
    using PressMachineServices.Press;
    using ZDPress.Opc;
    using System.Windows.Controls.DataVisualization;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Сервис для работы с прессами.
        /// </summary>
        private IPressService _pressService;

        /// <summary>
        /// Сервис для работы с прессами.
        /// </summary>
        private IOpcService _opcService;

        private PressMachineDbContex _dbContext;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this._dbContext = new PressMachineDbContex();

            this._opcService = new OpcService(this._dbContext);

            this._pressService = new PressService(this._dbContext);

            this.SetChartWindow();

            this._opcService.ConfigureProcessor();

            this._opcService.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this._opcService.Stop();
        }

        /// <summary>
        /// Настраивает и показывает форму с графиками.
        /// </summary>
        private void SetChartWindow()
        {
            // Create the startup window
            ChartWindow chartWindow = new ChartWindow();

            // Do stuff here, e.g. to the window
            chartWindow.Title = "Прессование";

            ChartWindowViewModel chartWindowViewModel = new ChartWindowViewModel(this._pressService);

            chartWindow.DataContext = chartWindowViewModel;
            
            // Show the window
            chartWindow.Show();
        }
    }
}
