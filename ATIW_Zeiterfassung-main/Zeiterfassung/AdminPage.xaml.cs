using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Zeiterfassung
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        private readonly AdminPageProvider AdminPageProvider;
        private readonly DbHandler handler;
        private List<User> Users { get; set; }
        private List<Entry> Entrys { get; set; } = new();
        public AdminPage(DbHandler dbHandler)
        {
            InitializeComponent();
            handler = dbHandler;
            AdminPageProvider = new(dbHandler);
            Users = AdminPageProvider.GetUsers();
            UserComboBox.ItemsSource = Users;
            FilterComboBox.ItemsSource = AdminPageProvider.GetFilterOptions();
        }

        private void UserComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            HandleListViewSource();
        }

        private void HandleListViewSource()
        {
            if (UserComboBox.SelectedItem is null)
                return;
            var user = UserComboBox.SelectedItem as User;
            Entrys = AdminPageProvider.GetEntries(user.GetId());
            if (FilterComboBox.SelectedItem is not null)
            {
                var splittedString = (FilterComboBox.SelectedItem as string).Split('/');
                var month = int.Parse(splittedString[0]);
                var year = int.Parse(splittedString[1]);
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddSeconds(-1);
                EntryHistoryListView.ItemsSource = Entrys.Where(e => e.StartTimeStamp.Date <= endDate && e.StartTimeStamp >= startDate).OrderByDescending(e => e.StartTimeStamp).ToList();
            }
            else
            {
                EntryHistoryListView.ItemsSource = Entrys.OrderByDescending(e => e.StartTimeStamp).ToList();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterComboBox.SelectedItem = null;
        }

        private void FilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            HandleListViewSource();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var page = new AddUserPage(handler, this, Users);
            page.Show();
        }
    }
}
