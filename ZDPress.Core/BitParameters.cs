using System;

namespace ZDPress.Opc
{
    /// <summary>
    /// Битовые параметры
    /// </summary>
    [Flags]
    public enum BitParameters
    {
        None = 0,
        ShowGraph = 1 << 0,       // 1
        AvtomatRezhim = ShowGraph << 1, //2
        RuchnoRezhim = AvtomatRezhim << 1, //3
        IzmerenieOsiStart = RuchnoRezhim << 1, //4        
        LeftWheel = IzmerenieOsiStart << 1, //5
        RightWheel = LeftWheel << 1,   //6
        VihodV0Mon = RightWheel << 1, //7
        AlarmDispPress1 = VihodV0Mon << 1, //8
        AlarmDispPress2 = AlarmDispPress1 << 1,   //9
        AlarmDispPress3 = AlarmDispPress2 << 1 //10
    }
}
