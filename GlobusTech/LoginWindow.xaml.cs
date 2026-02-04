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
using System.Windows.Shapes;

namespace GlobusTech
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private User currentUser;
        public LoginWindow()
        {
            InitializeComponent();
   
        }
        public void Auth()
        {
            using (var context = new GlobusTechnologyContext())
            {
                var user = context.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == LoginBox.Text && u.Password == PasswordBox.Password);
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                currentUser = user;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Auth();
            if(currentUser != null)
            {
                MainWindow window = new MainWindow(currentUser);
                window.Show();
                this.Close();
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            User user = new User()
            {
                Role = new Role()
                {
                    Id=0,
                    Name = null,
                }
            };
            currentUser = user;
            MainWindow window = new MainWindow(currentUser);
            window.Show();
            this.Close();
        }
    }
}
