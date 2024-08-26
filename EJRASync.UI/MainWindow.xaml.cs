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
using Amazon.S3;
using EJRASync.Lib;

namespace EJRASync.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            var viewModel = (MainWindowViewModel)this.DataContext;
            var s3Client = new AmazonS3Client("", "", new AmazonS3Config
            {
                ServiceURL = Constants.MinioUrl,
                ForcePathStyle = true,
            });
            viewModel.S3Client = s3Client;

            viewModel.SyncManager = new SyncManager(viewModel.S3Client);
        }
    }
}