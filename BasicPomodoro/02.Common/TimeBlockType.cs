using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPomodoro.Common
{
    public enum TimeBlockType
    {
        Pomodoro,
        [Description("Short break")]
        ShortBreak,
        [Description("Long break")]
        LongBreak
    }
}
