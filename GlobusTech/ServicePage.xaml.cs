using Microsoft.EntityFrameworkCore;
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

namespace GlobusTech
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage(User currentUser)
        {
            InitializeComponent();
            DateSortBox.ItemsSource = new List<string>
            {
                "По умолчанию",
                "Новые",
                "Старые"
            };
            LoadServices();
            SearchBar.Visibility = currentUser.Role.Name == null ? Visibility.Collapsed : Visibility.Visible;
        }
        public void LoadServices()
        {
            using (var context = new GlobusTechnologyContext())
            {
                var services = context.Services
                    .Include(s => s.Applications)
                    .Include(s => s.ServiceSphere)
                    .Include(s => s.TeamType).AsQueryable();

                if (!string.IsNullOrEmpty(SearchBox.Text))
                {
                    var searchtext = SearchBox.Text.ToLower();
                    services = services.Where(s => s.Name.ToLower().Contains(searchtext));
                }
                if (DateSortBox.SelectedIndex != null || DateSortBox.SelectedIndex != 0)
                {
                    switch (DateSortBox.SelectedIndex)
                    {
                        case 1:
                            services = services.OrderBy(s => s.StartDate);
                            break;
                        case 2:
                            services = services.OrderByDescending(s => s.StartDate);
                            break;
                    }
                }
                ServiceCard.ItemsSource = services.ToList();
            }


        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadServices();
        }

        private void DateSortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadServices();
        }
    }
}
