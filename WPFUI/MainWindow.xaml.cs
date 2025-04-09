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
using Engine.EventArgs;
using Engine.ViewModels;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppSession _appSession = new AppSession();

        public MainWindow()
        {
            InitializeComponent();

            _appSession.OnMessageRaised += OnMessageRaised;

            DataContext = _appSession;
        }

        private void OnChange_SearchItem(object sender, TextChangedEventArgs e)
        {
            
        }

        private void OnClick_RefreshDatabase(object sender, RoutedEventArgs e)
        {
            _appSession.RefreshDatabase();
        }

        private void OnSelect_ChangeItem(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void OnMessageRaised(object sender, MessageEventArgs e)
        {
            txtError.Text = e.Message;
        }
    }
}