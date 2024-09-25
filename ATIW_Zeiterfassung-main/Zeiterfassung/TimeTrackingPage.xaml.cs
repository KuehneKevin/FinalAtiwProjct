using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Zeiterfassung
{
    /// <summary>
    /// Interaction logic for TimeTrackingPage.xaml
    /// </summary>
    public partial class TimeTrackingPage : Window
    {
        public User? User { get; set; }
        private bool IsCheckedIn = false;
        private DateTime? CheckInTimeStamp;
        private readonly DbHandler dbHandler = new();
        private readonly TimeTrackingPageProvider timeTrackingPageProvider;
        private List<Entry> Entrys = new();
        private DispatcherTimer? Timer;
        private TimeSpan InternDurationTimeSpan = new TimeSpan();
        private string DurationTextBoxString = "    -------------";
        public bool IsEventTriggerd = false;

        public TimeTrackingPage()
        {
            InitializeComponent();
            timeTrackingPageProvider = new TimeTrackingPageProvider(dbHandler);
            ChipCheckInOutBox.Focus();
            DataContext = DurationTextBoxString;
        }

        private void ChipCheckInOutBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
                return;
            CheckInOrOut();
        }

        private void CheckInOrOut()
        {
            if (User is null)
            {
                User = timeTrackingPageProvider.GetUser(ChipCheckInOutBox.Text);
                if (User is null)
                    return;
                Entrys = timeTrackingPageProvider.GetEntries(User.GetId()).OrderBy(e => e.StartTimeStamp).ToList();
            }
            EntryFormButton.IsEnabled = true;
            if (User.IsAdmin)
            {
                var adminPage = new AdminPage(dbHandler);
                adminPage.Show();
                this.Close();
                return;
            }
            CheckInTimeStamp = timeTrackingPageProvider.CreateEntryOrCheckIn(User, IsCheckedIn, CheckInTimeStamp, Entrys);
            UpdateCurrentStampTextBoxes();

            IsCheckedIn = !IsCheckedIn;
            UpdatePage();
        }

        private void UpdateCurrentStampTextBoxes()
        {
            if (CheckInTimeStamp is not null)
            {
                var timeStampString = CheckInTimeStamp.ToString();
                var dateTime = DateTime.Parse(timeStampString);
                StartTimeValueLabel.Content = dateTime.ToString("HH:mm");
                Timer = new DispatcherTimer();
                Timer.IsEnabled = true;
                Timer.Interval = new TimeSpan(0, 1, 0);
                Timer.Tick += Timer_Tick;
                DurationTextBoxString = InternDurationTimeSpan.ToString(@"hh\:mm");
                DurationValueLabel.Content = DurationTextBoxString;
            }
            else
            {
                Timer.Stop();
                Timer = null;
                IsEventTriggerd = false;
                InternDurationTimeSpan = new TimeSpan();
                StartTimeValueLabel.Content = "    ---------------";
                DurationTextBoxString = "    -------------";
                DurationValueLabel.Content = DurationTextBoxString;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (IsEventTriggerd)
            {
                InternDurationTimeSpan = InternDurationTimeSpan.Add(new TimeSpan(0, 1, 0));
                DurationTextBoxString = InternDurationTimeSpan.ToString(@"hh\:mm");
                DurationValueLabel.Content = DurationTextBoxString;
            }
            else
            {
                IsEventTriggerd = true;
            }
        }

        public void UpdatePage()
        {
            HelloUserLabel.Content = $"Hallo {User.Username}";
            ChipCheckInOutBox.Text = string.Empty;
            CheckInOutButton.Content = timeTrackingPageProvider.GetButtonContent(IsCheckedIn);
            ChipCheckInOutBox.Focus();
            EntryHistoryListView.ItemsSource = Entrys.OrderByDescending(e => e.StartTimeStamp).ToList();
            EntryHistoryListView.SelectedItem = null;
        }

        private void CheckInOutButton_Click(object sender, RoutedEventArgs e)
        {
            CheckInOrOut();
        }

        private void EntryFormButton_Click(object sender, RoutedEventArgs e)
        {
            var entryForm = new EntryFormPage(User, dbHandler, this, Entrys);
            ChipCheckInOutBox.Focus();
            IsEnabled = false;
            entryForm.Show();
        }

        private void EntryHistoryListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var entry = EntryHistoryListView.SelectedItem as Entry;
            if (entry is null)
                return;
            var page = new EntryDetailPage(entry, dbHandler, this, Entrys);
            ChipCheckInOutBox.Focus();
            IsEnabled = false;
            page.Show();
        }
    }
}
