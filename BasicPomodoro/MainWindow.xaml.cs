using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BasicPomodoro.BusinessLayer;
using BasicPomodoro.Common;
using Microsoft.Toolkit.Uwp.Notifications;

namespace BasicPomodoro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PomodoroSetup? _pomodoroSetup;
        private bool _isStarted = false;
        private bool _isFirstPomodoro = true;
        private bool _isPaused = false;
        private bool _willCompletePomodoro = false;
        private TimeSpan _timeRemaining = TimeSpan.Zero;
        private int _pomodorosCompleted = 0;
        private bool _startPomodorosAutomatically = false;
        private bool _startBreaksAutomatically = false;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            btnSettings.Click += BtnSettings_Click;
            btnStartPause.Click += BtnStartPause_Click;
            btnStop.Click += BtnStop_Click;
            lblPomodorosCompleted.Content = _pomodorosCompleted;
            lblTimeLeft.Content = _timeRemaining.ToString();

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0,0,1);
            _timer.Tick += Timer_Tick;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_isStarted)
            {
                _timer.Stop();
                _willCompletePomodoro = false;
                _timeRemaining = TimeSpan.Zero;
                lblTimeLeft.Content = _timeRemaining.ToString();
                _isStarted = false;
                _isPaused = false;
                btnStartPause.Content = Constants.Start;
                btnSettings.IsEnabled = true;
                lblTimeBlockType.Content = Constants.InitialMessage;
            }
        }

        private void BtnStartPause_Click(object sender, RoutedEventArgs e)
        {
            if (!_isStarted)
            {
                _pomodoroSetup = new PomodoroSetup();
                _isStarted = true;
                _isFirstPomodoro = true;
                _timer.Start();
                UpdateTimeBlock();
                btnStartPause.Content = Constants.Pause;
                btnSettings.IsEnabled = false;
                _startPomodorosAutomatically = Properties.Settings.Default.StartPomodorosAutomatically;
                _startBreaksAutomatically = Properties.Settings.Default.StartBreaksAutomatically;
                return;
            }
            TogglePause();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timeRemaining -= TimeSpan.FromSeconds(1);
            lblTimeLeft.Content = _timeRemaining.ToString();

            if (_timeRemaining <= TimeSpan.Zero)
            {           
                UpdateTimeBlock();
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsForm = new SettingsWindow();
            settingsForm.ShowDialog();
        }

        private void UpdateTimeBlock()
        {
            if (_willCompletePomodoro)
            {
                _pomodorosCompleted++;
                lblPomodorosCompleted.Content = _pomodorosCompleted;
                _willCompletePomodoro = false;
            }
            TimeBlock? block = _pomodoroSetup.GetNextTimeBlock();
            if (block is not null)
            {
                if (!_isFirstPomodoro)
                {
                    Notify($"{lblTimeBlockType.Content} completed");
                }
                _timeRemaining = block.BlockDuration;
                lblTimeBlockType.Content = block.BlockType.GetEnumValueDescription();
                lblTimeLeft.Content = _timeRemaining.ToString();
                _willCompletePomodoro = block.BlockType == TimeBlockType.Pomodoro;

                if ((!_isFirstPomodoro && _willCompletePomodoro && !_startPomodorosAutomatically) || ((block.BlockType == TimeBlockType.ShortBreak || block.BlockType == TimeBlockType.LongBreak) && !_startBreaksAutomatically) )
                {
                    TogglePause();
                }
                _isFirstPomodoro = false;
                return;
            }
            _timer.Stop();
            _isStarted = false;
            btnSettings.IsEnabled = true;
            lblTimeBlockType.Content = Constants.DailyObjectiveCompleted;
            Notify(Constants.DailyObjectiveCompleted);
        }

        private void TogglePause()
        {
            if (!_isPaused)
            {
                _timer.Stop();
                _isPaused = true;
                btnStartPause.Content = Constants.Start;
                return;
            }
            _timer.Start();
            _isPaused = false;
            btnStartPause.Content = Constants.Pause;
        }

        private void Notify(string text)
        {
            new ToastContentBuilder().AddText(text).Show();
        }
    }
}