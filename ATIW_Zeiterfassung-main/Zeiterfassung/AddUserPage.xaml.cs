using System;
using System.Collections.Generic;
using System.Windows;

namespace Zeiterfassung
{
    /// <summary>
    /// Interaction logic for AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Window
    {
        public AddUserPage(DbHandler handler, AdminPage adminPage, List<User> users)
        {
            InitializeComponent();
            Handler = handler;
            AdminPage = adminPage;
            Users = users;
        }

        public DbHandler Handler { get; }
        public AdminPage AdminPage { get; }
        public List<User> Users { get; }

        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            var user = new User(Guid.NewGuid(), UsernameTextbox.Text, (bool)IsAdminCheckBox.IsChecked, ChipIdTextbox.Text);
            if (Handler.TryCreateUser(user))
            {
                Users.Add(user);
                ClosePage();
                Close();
            }
            else
                ShowErrorLabel();
        }

        private void ShowErrorLabel()
        {
            DuplicateChipLabel.Content = "Nutzer konnte aufgrund bereits vorhandenen Stempel-Id nicht hinzugefügt werden.";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClosePage();
        }

        private void ClosePage()
        {
            AdminPage.Focus();
            AdminPage.IsEnabled = true;
            AdminPage.Show();
        }
    }
}
