using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    /// <summary>
    /// Interaction logic for LoadPlayListDialog.xaml
    /// </summary>
    public partial class LoadPlayListDialog : Window
    {
        Model model;
        private BindingList<Playlist> collection;

        public delegate void PlaylistInfosDelegate(Playlist playlist);
        public event PlaylistInfosDelegate NewPlaylistEvent = null;

        public LoadPlayListDialog()
        {
            InitializeComponent();
        }

        private void LoadPlayListDialog_Loaded(object sender, RoutedEventArgs e)
        {
            model = Model.GetInstance();
            collection = model.GetCollection();
            CollectionListView.ItemsSource = collection;
        }

        private void GetPlayList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (NewPlaylistEvent != null)
            {
                if (collection == null || collection.Count == 0)
                {
                    return;
                }

                //get selected item
                var item = ((FrameworkElement)e.OriginalSource).DataContext as Playlist;
                if (item != null)
                {
                    NewPlaylistEvent.Invoke(item);
                    DialogResult = true;
                }
            }
        }

        
    }
}
