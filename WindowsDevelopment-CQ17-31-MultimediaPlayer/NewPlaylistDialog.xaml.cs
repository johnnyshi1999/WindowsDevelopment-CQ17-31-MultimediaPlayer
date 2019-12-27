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

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    /// <summary>
    /// Interaction logic for NewPlaylist.xaml
    /// </summary>
    public partial class NewPlaylistDialog : Window
    {
        public delegate void PlaylistInfosDelegate(Playlist playlist);
        public event PlaylistInfosDelegate NewPlaylistEvent = null;
        public NewPlaylistDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if(PlaylistNameField.Text.Replace(" ", "").Length == 0)
            {
                MessageBox.Show("Cannot make a nameless playlist");
                return;
            }
            if(NewPlaylistEvent != null)
            {
                var playlist = new Playlist(PlaylistNameField.Text);
                NewPlaylistEvent.Invoke(playlist);
            }
            this.Close();
        }
    }
}
