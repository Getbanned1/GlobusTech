using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Net.Mime.MediaTypeNames;

namespace GlobusTech
{
    /// <summary>
    /// Логика взаимодействия для ApplicationPage.xaml
    /// </summary>
    public partial class ApplicationPage : Page
    {
        private ObservableCollection<ApplicationModel> _applicationPages;
        private GlobusTechnologyContext _context;
        public ApplicationPage()
        {
            InitializeComponent();
            _applicationPages = new();
            _context = new();

            LoadApplications();
        }
        public void LoadApplications()
        {
            try
            {
                var statuses = _context.ApplicationStatuses.Select(s => s.Name).ToList();
                statuses.Insert(0, "Все");
                StatusSortBox.ItemsSource = statuses;

                _applicationPages.Clear();
                var query = _context.Applications
                    .Include(a => a.Status)
                    .Include(a => a.User)
                    .Include(a => a.Service)
                    .AsQueryable();



                if (StatusSortBox.SelectedItem != null && StatusSortBox.SelectedItem.ToString() != "Все")
                {
                    query = query.Where(q => q.Status.Name == StatusSortBox.SelectedItem.ToString());

                }

                if (!string.IsNullOrEmpty(SearchBox.Text))
                {
                    var searchText = SearchBox.Text.ToLower();
                    query = query.Where(q => q.Service.Name.ToLower().Contains(searchText));
                }
                foreach (var item in query)
                    _applicationPages.Add(item);
                if (ApplicationGrid.ItemsSource == null)
                    ApplicationGrid.ItemsSource = _applicationPages;
                LoadComboboxData();
            }
            catch  (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.GetType().Name}");
            }
            


        }

        private void LoadComboboxData()
        {
            if (StatusColumn.ItemsSource == null)
            {
                StatusColumn.ItemsSource = _context.ApplicationStatuses.ToList();
                StatusColumn.DisplayMemberPath = "Name";
                StatusColumn.SelectedItemBinding = new Binding("Status");
            }
            if (FullNameColumn.ItemsSource == null)
            {
                FullNameColumn.ItemsSource = _context.Users.ToList();
                FullNameColumn.DisplayMemberPath = "FullName";
                FullNameColumn.SelectedItemBinding = new Binding("User");
            }
            if (ServiceColumn.ItemsSource == null)
            {
                ServiceColumn.ItemsSource = _context.Services.ToList();
                ServiceColumn.DisplayMemberPath = "Name";
                ServiceColumn.SelectedItemBinding = new Binding("Service");
            }


        }

        private void AddApplication_Click(object sender, RoutedEventArgs e)
        {
            var application = new ApplicationModel
            {
                Id = _applicationPages.Count() + 1,
                Service = _context.Services.FirstOrDefault(),
                User = _context.Users.FirstOrDefault(),
                Date = DateTime.Now,
                SpecialistAmount = 1,
                Comment = null,
                Status = _context.ApplicationStatuses.First(s => s.Name == "Новая")


            };
            if (application.SpecialistAmount > application.Service.AvalibleSlots)
            {
                MessageBox.Show($"Недостаточно свободных слотов для услуги \"{application.Service.Name}\".");
                return;
            }
            _context.Applications.Add(application);
            _applicationPages.Add(application);
        }

        private async void DeleteApplication_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (ApplicationGrid.SelectedItem is not ApplicationModel selectedappl)
                return;

            var result = MessageBox.Show(
                $"Удалить заявку {selectedappl.Service.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            _applicationPages.Remove(selectedappl);
            _context.Applications.Remove(selectedappl);
            await _context.SaveChangesAsync();
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplicationGrid.CommitEdit(DataGridEditingUnit.Row, true);
                UpdateCost();
                foreach (var service in _context.Services)
                {
                    if (service.AvalibleSlots < 0)
                        throw new InvalidOperationException($"Услуга {service.Name} имеет отрицательное количество лицензий!");
                }
                await _context.SaveChangesAsync();
                MessageBox.Show("Данные сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message},Тип{ex.GetType().Name}");
            }
        }

        private void UpdateCost()
        {
            foreach (var app in _applicationPages)
            {
                app.TotalCost = (app.Service?.Price ?? 0) * app.SpecialistAmount;
                _context.Applications.Update(app);
            }

        }

        private void StatusSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadApplications();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadApplications();
        }
        private void ApplicationGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.Item is not ApplicationModel app)
                return;

            // Смотрим, изменилась ли колонка "Статус"
            if (e.Column.Header.ToString() == "Статус")
            {
                var newStatus = app.Status;
                var confirmedStatus = _context.ApplicationStatuses.FirstOrDefault(s => s.Name == "Подтверждена");

                if (newStatus == confirmedStatus)
                {
                    // проверка свободных слотов
                    if (app.Service == null)
                    {
                        MessageBox.Show("Выберите услугу!");
                        e.Cancel = true;
                        return;
                    }

                    if (app.SpecialistAmount > app.Service.AvalibleSlots)
                    {
                        MessageBox.Show($"Недостаточно свободных слотов для услуги \"{app.Service.Name}\".");
                        app.Status = _context.ApplicationStatuses.First(s => s.Name == "Отменена");
                        _context.SaveChanges();
                        LoadApplications();
                        e.Cancel = true;
                        return;
                    }
                    app.Service.AvalibleSlots -= app.SpecialistAmount;
                    // уменьшаем слоты только при подтверждении
                    _context.Services.Update(app.Service);
                    _context.Applications.Update(app);
                }
            }
        }

    }
}
