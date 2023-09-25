using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPomodoro.BusinessLayer
{
    public class PomodoroSetup
    {
        private readonly Queue<TimeBlock> _timeBlockQueue;
        private readonly int _pomodoroDuration;
        private readonly int _shortBreakDuration;
        private readonly int _longBreakDuration;
        private readonly int _pomodorosForLongBreak;
        private readonly int _dailyPomodoroObjective;

        public PomodoroSetup() 
        {
            _timeBlockQueue = new Queue<TimeBlock>();
            _pomodoroDuration = Properties.Settings.Default.PomodoroDuration;
            _shortBreakDuration = Properties.Settings.Default.ShortBreakDuration;
            _longBreakDuration = Properties.Settings.Default.LongBreakDuration;
            _pomodorosForLongBreak = Properties.Settings.Default.PomodorosForLongBreak;
            _dailyPomodoroObjective = Properties.Settings.Default.DailyPomodoroObjective;
            EnqueueTimeBlocks();
        }

        private void EnqueueTimeBlocks()
        {
            int totalPomodoros;
            int pomodoros = 0;
            for (totalPomodoros = 0; totalPomodoros < _dailyPomodoroObjective; totalPomodoros++)
            {
                _timeBlockQueue.Enqueue(new TimeBlock 
                {
                    BlockType = Common.TimeBlockType.Pomodoro,
                    BlockDuration = GetTimeBlockInterval(_pomodoroDuration)
                });
                pomodoros++;
                if (totalPomodoros != _dailyPomodoroObjective - 1)
                {
                    if (pomodoros == _pomodorosForLongBreak)
                    {
                        _timeBlockQueue.Enqueue(new TimeBlock
                        {
                            BlockType = Common.TimeBlockType.LongBreak,
                            BlockDuration = GetTimeBlockInterval(_longBreakDuration)
                        });
                        pomodoros = 0;
                    }
                    else
                    {
                        _timeBlockQueue.Enqueue(new TimeBlock
                        {
                            BlockType = Common.TimeBlockType.ShortBreak,
                            BlockDuration = GetTimeBlockInterval(_shortBreakDuration)
                        });
                    }
                }
            }
        }

        private TimeSpan GetTimeBlockInterval(long minutes)
        {
            return new TimeSpan(minutes * 60 * (long)Math.Round(Math.Pow(10, 7)));
        }

        public TimeBlock? GetNextTimeBlock()
        {
            if (_timeBlockQueue.Count > 0)
            {
                return _timeBlockQueue.Dequeue();
            }
            return null;
        }
    }
}
