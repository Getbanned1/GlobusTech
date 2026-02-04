using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows.Data;
using System;

namespace GlobusTech
{
    public partial class UsersPage : Page
    {
        private ObservableCollection<User> _users;
        private ObservableCollection<Role> _roles;
        private GlobusTechnologyContext _context;

        public ObservableCollection<User> Users => _users;

        public UsersPage()
        {
            InitializeComponent();

            _context = new GlobusTechnologyContext();
            _users = new ObservableCollection<User>();
            _roles = new ObservableCollection<Role>();

            DataContext = this;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем роли и компании
                _roles.Clear();
                foreach (var role in _context.Roles)
                    _roles.Add(role);

                // ComboBox в DataGrid
                RoleColumn.ItemsSource = _roles;
                RoleColumn.DisplayMemberPath = "Name";
                RoleColumn.SelectedItemBinding = new Binding("Role");

                CompanyColumn.ItemsSource = _context.Companies.ToList();
                CompanyColumn.DisplayMemberPath = "Name";
                CompanyColumn.SelectedItemBinding = new Binding("Company");

                // Загружаем пользователей
                _users.Clear();

                var query = _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Company)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    var text = SearchBox.Text.ToLower();
                    query = query.Where(u => u.FullName.ToLower().Contains(text));
                }

                foreach (var user in query)
                    _users.Add(user);

                // ItemsSource устанавливаем ОДИН РАЗ
                if (UsersGrid.ItemsSource == null)
                    UsersGrid.ItemsSource = _users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersGrid.CommitEdit(DataGridEditingUnit.Row, true);
                await _context.SaveChangesAsync();
                MessageBox.Show("Изменения сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var role = _roles.FirstOrDefault();

            var newUser = new User
            {
                Id = _users.Count() + 1,
                FullName = "Новый пользователь",
                Email = "email",
                Login = "login",
                Password = "passw",
                Role = role,
                Company = null
            };

            _context.Users.Add(newUser);
            _users.Add(newUser);
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is not User selectedUser)
                return;

            var result = MessageBox.Show(
                $"Удалить пользователя {selectedUser.FullName}?",
                "Подтверждение",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            _users.Remove(selectedUser);
            _context.Users.Remove(selectedUser);
            await _context.SaveChangesAsync();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }
    }
}
