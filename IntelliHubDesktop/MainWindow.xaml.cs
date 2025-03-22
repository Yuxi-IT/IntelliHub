using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Helpers;
using IntelliHubDesktop.Models;
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

namespace IntelliHubDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConfigModel.Initialize();
            Runtimes.ApiUrl = ConfigModel.Read("api");
            Runtimes.Key = ConfigModel.Read("key");

        }
        private void opt_Themebtn_Click(object sender, RoutedEventArgs e)
        {
            void SetTheme(ApplicationTheme? theme = null)
            {
                if (theme != null)
                    ThemeResources.Current.RequestedTheme = theme;
                else
                    ThemeResources.Current.RequestedTheme = ColorsHelper.Current.SystemTheme.GetValueOrDefault(ApplicationTheme.Light);
            }

            if (opt_Themebtn.IsChecked == true)
            {
                SetTheme(ApplicationTheme.Dark);
                opt_Themebtn.Content = "☀";
            }
            else
            {
                SetTheme(ApplicationTheme.Light);
                opt_Themebtn.Content = "🌙";
            }
        }
    }
}