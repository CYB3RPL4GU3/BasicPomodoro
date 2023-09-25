using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BasicPomodoro
{
    /// <summary>
    /// Lógica de interacción para SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private bool _hasChanged = false;

        public SettingsWindow()
        {
            InitializeComponent();
            _hasChanged = false;
            ReadSettings(); // Read settings before setting event listeners, so setting control values for the first time doesn't count as a change, hence _hasChanged remains false
            btnSaveExit.Click += BtnSaveExit_Click;
            txtPomodoroLength.TextChanged += TxtPomodoroLength_TextChanged;
            txtShortBreakLength.TextChanged += TxtShortBreakLength_TextChanged;
            txtLongBreakLength.TextChanged += TxtLongBreakLength_TextChanged;
            txtPomodorosForLongBreak.TextChanged += TxtPomodorosForLongBreak_TextChanged;
            txtDailyPomodoroObjective.TextChanged += TxtDailyPomodoroObjective_TextChanged;
            chkAutoPomodoros.Click += ChkAutoPomodoros_Click;
            chkAutoBreaks.Click += ChkAutoBreaks_Click;
            btnCancel.Click += BtnCancel_Click;
            Closing += SettingsWindow_Closing;
        }

        private void SettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_hasChanged)
            {
                var ans = MessageBox.Show("Changes to settings were made. Do you want to save them?", "BasicPomodoro", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (ans)
                {
                    case MessageBoxResult.Yes:
                        Properties.Settings.Default.Save();
                        _hasChanged = false;
                        break;
                    case MessageBoxResult.No:
                        Properties.Settings.Default.Reload();
                        _hasChanged = false;
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reload();
            _hasChanged = false;
            Close();
        }

        private void ChkAutoBreaks_Click(object sender, RoutedEventArgs e)
        {
            UpdateSetting("StartBreaksAutomatically", chkAutoBreaks);
        }

        private void ChkAutoPomodoros_Click(object sender, RoutedEventArgs e)
        {
            UpdateSetting("StartPomodorosAutomatically", chkAutoPomodoros);
        }

        private void TxtDailyPomodoroObjective_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSetting("DailyPomodorObjective", txtDailyPomodoroObjective);
        }

        private void TxtPomodorosForLongBreak_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSetting("PomodorosForLongBreak", txtPomodorosForLongBreak);
        }

        private void TxtLongBreakLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSetting("LongBreakDuration", txtLongBreakLength);
        }

        private void TxtShortBreakLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSetting("ShortBreakDuration", txtShortBreakLength);
        }

        private void TxtPomodoroLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSetting("PomodoroDuration", txtPomodoroLength);
        }

        private void BtnSaveExit_Click(object sender, RoutedEventArgs e)
        {
            if (_hasChanged)
            {
                Properties.Settings.Default.Save();
                _hasChanged = false;
            }

            Properties.Settings.Default.Reload();
            Close();
        }

        /// <summary>
        /// Method that reads settings from the Settings.settings file and sets every control with the appropriate value.
        /// </summary>
        private void ReadSettings()
        {
            txtPomodoroLength.Text = Properties.Settings.Default.PomodoroDuration.ToString();
            txtShortBreakLength.Text = Properties.Settings.Default.ShortBreakDuration.ToString();
            txtLongBreakLength.Text = Properties.Settings.Default.LongBreakDuration.ToString();
            txtPomodorosForLongBreak.Text = Properties.Settings.Default.PomodorosForLongBreak.ToString();
            txtDailyPomodoroObjective.Text = Properties.Settings.Default.DailyPomodoroObjective.ToString();
            chkAutoPomodoros.IsChecked = Properties.Settings.Default.StartPomodorosAutomatically;
            chkAutoBreaks.IsChecked = Properties.Settings.Default.StartBreaksAutomatically;
        }

        /// <summary>
        /// Method that updates a setting from the content of a TextBox
        /// </summary>
        /// <param name="settingName">Setting to update</param>
        /// <param name="txt">TextBox to read</param>
        private void UpdateSetting(string settingName, TextBox txt)
        {
            SettingsProperty prop = Properties.Settings.Default.Properties[settingName];
            Type valueType = prop.PropertyType;
            object oldValue = Properties.Settings.Default[settingName];

            object? newValue = null;
            if (!txt.Text.Equals(string.Empty))
            {
                try
                {
                    newValue = Convert.ChangeType(txt.Text, valueType);
                }
                catch (Exception)
                {
                    //Conversion failed, undo text change
                    txt.Text = oldValue.ToString();
                    return;
                }
            }

            if (newValue is not null && newValue != oldValue)
            {
                Properties.Settings.Default[settingName] = newValue;
                _hasChanged = true;
            }  
        }

        /// <summary>
        /// Method that updates a setting from the state of a CheckBox
        /// </summary>
        /// <param name="settingName">Setting to update</param>
        /// <param name="chk">CheckBox to read</param>
        private void UpdateSetting(string settingName, CheckBox chk)
        {
            bool oldValue = (bool)Properties.Settings.Default[settingName];
            if (chk.IsChecked.HasValue && chk.IsChecked.Value != oldValue)
            {
                Properties.Settings.Default[settingName] = chk.IsChecked.Value;
                _hasChanged = true;
            }
        }
    }
}
