using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GlobusTech;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private User currentUser;


    public MainWindow(User currentUser)
    {
        InitializeComponent();
        this.currentUser = currentUser;
        if (currentUser.Role.Name == null) 
            NavigationPanel.Visibility = Visibility.Collapsed;
        else
        {
            NavigationPanel.Visibility = currentUser.Role.Name == "Клиент" ? Visibility.Collapsed : Visibility.Visible;
        }
        MainFrame.Navigate(new ServicePage(currentUser));
    }

    private void ServicesNavButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ServicePage(currentUser));
    }

    private void ApplicationsNavButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new ApplicationPage(currentUser));

    }

    private void UsersNavButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new UsersPage());

    }
}