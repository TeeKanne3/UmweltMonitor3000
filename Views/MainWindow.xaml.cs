using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
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
using UmweltMonitor3000.Application.Models;
using UmweltMonitor3000.Application.ViewModels;
using UmweltMonitor3000.Application.Services;
using UmweltMonitor3000.Application.ViewModels;

namespace UmweltMonitor3000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowLogic _logic = new();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, _) => DarkTitleBar.TrySetDarkTitleBar(this);
            DataContext = new MainViewModel();
        }
    }
}