using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Zeiterfassung
{
    /// <summary>
    /// Interaction logic for EntryFormPage.xaml
    /// </summary>
    public partial class EntryFormPage : Window
    {
        private User User { get; set; }
        private readonly DbHandler DbHandler;
        private List<string> hours = new();
        private List<string> minutes = new();
        private TimeTrackingPage Page;
        public List<Entry> Entries { get; set; }
        public EntryFormPage(User user, DbHandler dbHandler, TimeTrackingPage page, List<Entry> entries)
        {
            InitializeComponent();
            User = user;
            DbHandler = dbHandler;
            Page = page;
            FillComboBoxes();
            DatePicker.SelectedDate = DateTime.Now.Date;
            Entries = entries;
        }

        private void FillComboBoxes()
        {
            CreateLists();
            StartTimePicker_Hours.ItemsSource = hours;
            StartTimePicker_Minutes.ItemsSource = minutes;
            EndTimePicker_Hours.ItemsSource = hours;
            EndTimePicker_Minutes.ItemsSource = minutes;
        }

        private void CreateLists()
        {
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i.ToString("00"));
            }
            for (int i = 0; i < 60; i++)
            {
                minutes.Add(i.ToString("00"));
            }
        }

        private void CreateEntryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DatePicker.SelectedDate.HasValue
                || StartTimePicker_Hours.SelectedItem is null
                || StartTimePicker_Minutes.SelectedItem is null
                || EndTimePicker_Hours.SelectedItem is null
                || EndTimePicker_Minutes.SelectedItem is null)
            {
                ShowErrorLabel("Eine der gemachten Eingaben ist inkorrekt. Bitte anpassen");
                return;
            }
            var date = DatePicker.SelectedDate.Value;
            var startTime = new DateTime
                (date.Year, date.Month, date.Day, Convert.ToInt32(StartTimePicker_Hours.SelectedValue), Convert.ToInt32(StartTimePicker_Minutes.SelectedValue), 0, 0);
            var endTime = new DateTime
                (date.Year, date.Month, date.Day, Convert.ToInt32(EndTimePicker_Hours.SelectedValue), Convert.ToInt32(EndTimePicker_Minutes.SelectedValue), 0, 0);
            if (endTime <= startTime)
            {
                ShowErrorLabel("Das Enddatum darf nicht kleiner als das Startdatum sein");
                return;
            }
            var entry = new Entry(Guid.NewGuid(), User.GetId(), startTime, endTime);
            DbHandler.SafeEntryInDatabase(entry);
            Entries.Add(entry);
            Close();
            UpdateTimeTrackingPage();
        }

        private void UpdateTimeTrackingPage()
        {
            Page.IsEnabled = true;
            Page.Focus();
            Page.UpdatePage();
        }

        private void ShowErrorLabel(string text)
        {
            ErrorLabel.Content = text;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateTimeTrackingPage();
        }
    }
}
