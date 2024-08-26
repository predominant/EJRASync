using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Amazon.S3;
using EJRASync.Lib;

namespace EJRASync.UI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public AmazonS3Client S3Client;
        public SyncManager SyncManager;

        private ObservableCollection<ContentItem> _contentItems;

        public ObservableCollection<ContentItem> ContentItems
        {
            get => this._contentItems;
            set
            {
                this._contentItems = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand SyncCommand { get; }

        public MainWindowViewModel()
        {
            this.S3Client = new AmazonS3Client("", "", new AmazonS3Config
            {
                ServiceURL = Constants.MinioUrl,
                ForcePathStyle = true,
            });
        }

        public void Init()
        {
            this.SyncManager = new SyncManager(this.S3Client);
            this.GetTopLevelItems().Wait();
        }

        private async Task GetTopLevelItems()
        {
            foreach (var item in await this.SyncManager.ListS3FoldersAsync("ejra-cars"))
            {
                this.ContentItems.Add(new ContentItem
                {
                    Name = item,
                    Type = "Car",
                    Status = "TODO",
                });
            }

            foreach (var item in await this.SyncManager.ListS3FoldersAsync("ejra-tracks"))
            {
                this.ContentItems.Add(new ContentItem
                {
                    Name = item,
                    Type = "Track",
                    Status = "TODO",
                });
            }
        }

        public async Task SyncContent()
        {
            await this.SyncManager.SyncAllAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
