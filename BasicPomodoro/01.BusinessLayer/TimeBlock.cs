using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicPomodoro.Common;

namespace BasicPomodoro.BusinessLayer
{
    public class TimeBlock
    {
        public TimeSpan BlockDuration { get; set; }

        public TimeBlockType BlockType { get; set; }
    }
}