using System;
using System.Collections.Generic;
using System.Windows;

namespace Zeiterfassung
{
    /// <summary>
    /// Interaction logic for EntryDetailPage.xaml
    /// </summary>
    public partial class EntryDetailPage : Window
    {
        private Entry Entry { get; set; }
        private DbHandler DbHandler { get; set; }
        private bool IsEditingMode { get; set; } = false;
        private readonly TimeTrackingPage TimeTrackingPage;
        private readonly List<Entry> Entrys;
        public EntryDetailPage(Entry entry, DbHandler dbHandler, TimeTrackingPage page, List<Entry> entrys)
        {
            InitializeComponent();
            Entry = entry;
            DbHandler = dbHandler;
            TimeTrackingPage = page;
            Entrys = entrys;
            FillBoxes();
        }

        private void FillBoxes()
        {
            StartBox.Text = Entry.StartTimeStamp.ToString("HH:mm:ss");
            EndBox.Text = Entry.EndTimeStamp.ToString("HH:mm:ss");
            EntryDatePicker.SelectedDate = Entry.StartTimeStamp;
            TimeSpanBox.Text = Entry.TimeSpan;
        }

        private void EditSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEditingMode)
                ReverseMode();
            else
                OverrideExistingEntry();
        }

        private void CancelCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditingMode)
                ReverseMode();
            else
                Close();
        }

        private void ReverseMode()
        {
            EntryDatePicker.IsEnabled = !EntryDatePicker.IsEnabled;
            StartBox.IsEnabled = !StartBox.IsEnabled;
            EndBox.IsEnabled = !EndBox.IsEnabled;
            IsEditingMode = !IsEditingMode;
            DeleteButton.IsEnabled = !DeleteButton.IsEnabled;
            ErrorLabel.Content = string.Empty;
            FillBoxes();
            ChangeButtonContent();
        }

        private void ChangeButtonContent()
        {
            if (IsEditingMode)
            {
                EditSaveButton.Content = "Speichern";
                CancelCloseButton.Content = "Abbrechen";
            }
            else
            {
                EditSaveButton.Content = "Bearbeiten";
                CancelCloseButton.Content = "Schließen";
            }
        }

        private void OverrideExistingEntry()
        {
            var entryBackUp = new Entry(Entry);
            try
            {
                if (EntryDatePicker.SelectedDate.Value.ToString("dd/MM/yyyy") != Entry.StartTimeStamp.ToString("dd/MM/yyyy"))
                {
                    var date = EntryDatePicker.SelectedDate.Value;
                    var newStartDate = new DateTime(date.Year, date.Month, date.Day, Entry.StartTimeStamp.Hour, Entry.StartTimeStamp.Minute, Entry.StartTimeStamp.Second);
                    var newEndDate = new DateTime(date.Year, date.Month, date.Day, Entry.EndTimeStamp.Hour, Entry.EndTimeStamp.Minute, Entry.EndTimeStamp.Second);
                    Entry.StartTimeStamp = newStartDate;
                    Entry.EndTimeStamp = newEndDate;
                }
                if (StartBox.Text != Entry.StartTimeStamp.ToString("HH:mm:ss"))
                {
                    var startDateTime = DateTime.Parse(StartBox.Text);
                    var newStartDateTime = new DateTime(Entry.StartTimeStamp.Year, Entry.StartTimeStamp.Month, Entry.StartTimeStamp.Day, startDateTime.Hour, startDateTime.Minute, startDateTime.Second);
                    Entry.StartTimeStamp = newStartDateTime;
                }
                if (EndBox.Text != Entry.EndTimeStamp.ToString("HH:mm:ss"))
                {
                    var endDateTime = DateTime.Parse(EndBox.Text);
                    var newEndDateTime = new DateTime(Entry.EndTimeStamp.Year, Entry.EndTimeStamp.Month, Entry.EndTimeStamp.Day, endDateTime.Hour, endDateTime.Minute, endDateTime.Second);
                    Entry.EndTimeStamp = newEndDateTime;
                }

                Entry.TimeSpan = (Entry.EndTimeStamp - Entry.StartTimeStamp).ToString(@"hh\:mm\:ss");
                TimeSpanBox.Text = Entry.TimeSpan;
            }
            catch (Exception)
            {
                ErrorLabel.Content = "Eine der eingegebenen Daten ist nicht korrekt, wodurch der Eintrag nicht angepasst werden kann.";
                Entry = entryBackUp;
                return;
            }
            DbHandler.UpdateExistingEntry(Entry);
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DbHandler.RemoveEntry(Entry);
            Entrys.Remove(Entry);
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TimeTrackingPage.UpdatePage();
            TimeTrackingPage.IsEnabled = true;
        }
    }
}