namespace PressMachine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using PressMachineServices.Press;
    using System.Linq;
    using Excel = Microsoft.Office.Interop.Excel;
    using System.Configuration;

    public class ChartWindowViewModel : BaseViewModel
    {

        /// <summary>
        /// Сервис для работы с прессом.
        /// </summary>
        private IPressService _pressService;

        /// <summary>
        /// Фильтр время От.
        /// </summary>
        private DateTime _from;

        /// <summary>
        /// Выбранный пресс.
        /// </summary>
        private Press _selectedPress;

        /// <summary>
        /// Фильтр время До.
        /// </summary>
        private DateTime _to;

        /// <summary>
        /// Фильтр дата От.
        /// </summary>
        public DateTime From
        {
            get
            {
                return this._from;
            }
            set
            {
                if (this._from != value)
                {
                    this._from = value;

                    this.OnPropertyChanged("From");

                    this._to = this._from.AddDays(1);
                    RefreshPressData();
                }
            }
        }

        /// <summary>
        /// Фильтр время До.
        /// </summary>
        public DateTime To
        {
            get
            {
                return this._to;
            }
            set
            {
                if (this._to != value)
                {
                    this._to = value;
                    this.OnPropertyChanged("To");
                    RefreshPressData();
                }
            }
        }

        /// <summary>
        /// Скорость.
        /// </summary>
        private ChartData _speedData1;

        public ChartData SpeedData1
        {
            get
            {
                return this._speedData1;
            }
            set
            {
                if (this._speedData1 != value)
                {
                    this._speedData1 = value;

                    this.OnPropertyChanged("SpeedData1");
                }
            }
        }

        /// <summary>
        /// Скорость уставка.
        /// </summary>
        private ChartData _speedSPData;
        public ChartData SpeedSPData
        {
            get
            {
                return this._speedSPData;
            }
            set
            {
                if (this._speedSPData != value)
                {
                    this._speedSPData = value;

                    this.OnPropertyChanged("SpeedSPData");
                }
            }
        }

        /// <summary>
        /// Положение.
        /// </summary>
        private ChartData _positionData1;
        public ChartData PositionData1
        {
            get
            {
                return this._positionData1;
            }
            set
            {
                if (this._positionData1 != value)
                {
                    this._positionData1 = value;

                    this.OnPropertyChanged("PositionData1");
                }
            }
        }

        /// <summary>
        /// Положение уставка.
        /// </summary>
        private ChartData _positionSPData;
        public ChartData PositionSPData
        {
            get
            {
                return this._positionSPData;
            }
            set
            {
                if (this._positionSPData != value)
                {
                    this._positionSPData = value;

                    this.OnPropertyChanged("PositionSPData1");
                }
            }
        }

        /// <summary>
        /// Усилие.
        /// </summary>
        private ChartData _powerData1;
        public ChartData PowerData1
        {
            get
            {
                return this._powerData1;
            }
            set
            {
                if (this._powerData1 != value)
                {
                    this._powerData1 = value;

                    this.OnPropertyChanged("PowerData1");
                }
            }
        }

        /// <summary>
        /// Усилие уставка.
        /// </summary>
        private ChartData _powerSPData;
        public ChartData PowerSPData
        {
            get
            {
                return this._powerSPData;
            }
            set
            {
                if (this._powerSPData != value)
                {
                    this._powerSPData = value;

                    this.OnPropertyChanged("PowerSPData");
                }
            }
        }

        /// <summary>
        /// Температура.
        /// </summary>
        private ChartData _temperatureData;
        public ChartData TemperatureData
        {
            get
            {
                return this._temperatureData;
            }
            set
            {
                if (this._temperatureData != value)
                {
                    this._temperatureData = value;

                    this.OnPropertyChanged("TemperatureData");
                }
            }
        }

        public ObservableCollection<PressDataSelected> PressDataSelected { get; set; }

        public PressDataSelected _selectedPressData { get; set; }

        /// <summary>
        /// Выбранные операции прессования за день
        /// </summary>
        public PressDataSelected SelectedPressData
        {
            get
            {
                return this._selectedPressData;
            }
            set
            {
                if (this._selectedPressData != value)
                {
                    this._selectedPressData = value;

                    this.OnPropertyChanged("SelectedPressData");
                    this.RefreshChartData();
                }
            }
        }

        /// <summary>
        /// Прессы.
        /// </summary>
        public ObservableCollection<Press> Presses { get; private set; }

        /// <summary>
        /// Выбранный пресс.
        /// </summary>
        public Press SelectedPress
        {
            get
            {
                return this._selectedPress;
            }
            set
            {
                if (this._selectedPress != value)
                {
                    this._selectedPress = value;

                    this.OnPropertyChanged("SelectedPress");
                    RefreshPressData();
                }
            }
        }

        public ICommand ShowChartCommand { get; set; }

        public ICommand CreateReportCommand { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChartWindowViewModel"/>
        /// </summary>
        public ChartWindowViewModel(IPressService pressService)
        {
            this.ShowChartCommand = new RelayCommand(new Action<object>(this.OnShowChartCommand));
            this.CreateReportCommand = new RelayCommand(new Action<object>(this.OnCreateReportCommand));
            this._pressService = pressService;// ?? throw new ArgumentNullException("pressService");

            _from = DateTime.Now.Date;
            _to = _from.AddDays(1);
         
            _speedData1 = new ChartData();

            _powerData1 = new ChartData();

            _positionData1 = new ChartData();

            _positionSPData = new ChartData();
            _speedSPData = new ChartData();
            _powerSPData = new ChartData();
            this.LoadPresses();
        }

        public void OnShowChartCommand(object obj)
        {
            if (this._selectedPress == null)
            {
                MessageBox.Show("Необходимо выберать пресс");
                return;
            }

            this.RefreshPressData();
        }

        public void OnCreateReportCommand(object obj)
        {
            if (this._selectedPress == null)
            {
                MessageBox.Show("Необходимо выберать пресс");
                return;
            }
            PressOperationDataFilter filter = this.GetPressOperationDataFilter();

            List<PressOperationData> data = this._pressService.GetPressOperationData(filter);

            Excel.Application excelApp = new Excel.Application();

            // Сделать приложение Excel видимым
            //excelApp.Visible = true;
            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = excelApp.ActiveSheet;
            // Установить заголовки столбцов в ячейках

            workSheet.Cells[1, "A"] = "Дата/Время";
            workSheet.Cells[1, "B"] = "Уставка Высоты";
            workSheet.Cells[1, "C"] = "Высота";
            workSheet.Cells[1, "D"] = "Уставка Усилия";
            workSheet.Cells[1, "E"] = "Усилие";
            workSheet.Cells[1, "F"] = "Уставка скорости";
            workSheet.Cells[1, "G"] = "Скорость";
            workSheet.Cells[1, "H"] = "Температура";

            for (int i = 0; i < data.Count; i++)
            {
                workSheet.Cells[i + 2, "A"] = data[i].DateInsert.ToString();
                workSheet.Cells[i + 2, "B"] = data[i].PositionSP.ToString();
                workSheet.Cells[i + 2, "C"] = data[i].Position.ToString();
                workSheet.Cells[i + 2, "D"] = data[i].PowerSP.ToString();
                workSheet.Cells[i + 2, "E"] = data[i].Power.ToString();
                workSheet.Cells[i + 2, "F"] = data[i].SpeedSP.ToString();
                workSheet.Cells[i + 2, "G"] = data[i].Speed.ToString();
                workSheet.Cells[i + 2, "H"] = data[i].Temperature.ToString();
            }
            //" + DateTime.Now.ToString() + "
            string folder = ConfigurationManager.AppSettings["ReportFolder"];
            string savePath = @folder + @"report" + @DateTime.Now.ToString("MM-dd-yy") + @".xlsx";
            workSheet.SaveAs(savePath);

            //workSheet.Clo
        }

        private void LoadPresses()
        {
            List<Press> presses = this._pressService.GetPresses();

            this.Presses = new ObservableCollection<Press>(presses);
        }

        /// <summary>
        /// Обновляет данные в графике.
        /// </summary>
        private void RefreshPressData()
        {
            PressOperationDataFilter filter = this.GetPressOperationDataFilter();

            List<PressOperationData> data = this._pressService.GetPressOperationData(filter);


            this.PressDataSelected = new ObservableCollection<PressDataSelected>();
            foreach (PressOperationData item in data)
            {
                if (!PressDataSelected.Any() || !PressDataSelected.Any(pd => pd.UniqueId == item.UniqueID))
                {
                    PressDataSelected PressDataSelectedItem = new PressDataSelected();
                    PressDataSelectedItem.Id = item.Id;
                    PressDataSelectedItem.UniqueId = item.UniqueID;
                    PressDataSelectedItem.DateInsert = item.DateInsert.ToString();
                    PressDataSelected.Add(PressDataSelectedItem);
                }
            }
            this.OnPropertyChanged("PressDataSelected");

        }

        /// <summary>
        /// Обновляет данные в графике.
        /// </summary>
        private void RefreshChartData()
        {
            PressOperationDataFilter filter = this.GetPressOperationDataFilter();

            List<PressOperationData> data = this._pressService.GetPressOperationData(filter);

            if (SelectedPressData != null)
            {
                data = data.Where(d => d.UniqueID == SelectedPressData.UniqueId).ToList();

                RefreshCharts(data);
            }
           
        }

        /// <summary>
        /// Вернет фильтр для получения данных.
        /// </summary>
        /// <returns>Фильтр.</returns>
        private PressOperationDataFilter GetPressOperationDataFilter()
        {
            PressOperationDataFilter filter = new PressOperationDataFilter();

            filter.Press = this.SelectedPress;

            filter.From = this.From;

            filter.To = this.To;

            return filter;
        }

        /// <summary>
        /// Обновит графики.
        /// </summary>
        public void RefreshCharts(List<PressOperationData> data)
        {
            ChartData positionData = new ChartData();
            ChartData positionSPData = new ChartData();

            ChartData speedData = new ChartData();
            ChartData speedSPData = new ChartData();

            ChartData temperatureData = new ChartData();

            ChartData powerData = new ChartData();
            ChartData powerSPData = new ChartData();

            List<ChartData> speedDataList = new List<ChartData>();

            SpeedData1.Clear();

            PositionData1.Clear();

            PowerData1.Clear();

            SpeedSPData.Clear();
            PowerSPData.Clear();
            PositionSPData.Clear();

            foreach (PressOperationData item in data)
            {
                SpeedData1.AddData(item.DateInsert, item.Speed);
                PositionData1.AddData(item.DateInsert, item.Position);
                PowerData1.AddData(item.DateInsert, item.Power);

                SpeedSPData.AddData(item.DateInsert, item.SpeedSP);
                PowerSPData.AddData(item.DateInsert, item.PowerSP);
                PositionSPData.AddData(item.DateInsert, item.PositionSP);
                temperatureData.AddData(item.DateInsert, item.Temperature);

            }

            this.TemperatureData = temperatureData;

        }
    }
}
