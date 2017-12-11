using System;
using System.Collections.Generic;
using System.Text;

namespace TvConsole
{
    public enum TvConsoleMouseEventType
    {
        ButtonUpOrClick = 0,
        MouseMoved = 1,
        DoubleClick = 2,
        VerticalWheelMoved = 4,
        HorizontalWheelMoved = 8
    }
}
