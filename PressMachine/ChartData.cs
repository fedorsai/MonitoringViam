namespace PressMachine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class ChartData : ObservableCollection<KeyValuePair<DateTime, decimal>>
    {
        public Guid uniqueID;
        public void AddData(DateTime key, decimal value)
        {
            this.Add(new KeyValuePair<DateTime, decimal>(key, value));
        }
    }
}
