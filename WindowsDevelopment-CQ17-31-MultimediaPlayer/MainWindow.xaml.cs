using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //Playlist
        Playlist currentPlaylist;

        //Media player
        MusicBox myMusicBox;

        public MainWindow()
        {
            InitializeComponent();
        }

        //--------------------------Events--------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            currentPlaylist = new Playlist("My playlist");
            PlayListListView.ItemsSource = currentPlaylist.trackList;
            PlayListNameTextBlock.Text = currentPlaylist.playlistName;
        }

        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "MP3 Files(*.mp3;*.MP3)|*.mp3;*.MP3"
            };

            if (fileDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                string[] trackPaths = fileDialog.FileNames;
                int playlistCount = currentPlaylist.trackList.Count;
                for (int i = 0; i < trackPaths.Length; i++)
                {
                    bool trackDuplicate = false;

                    for (int j = 0; j < playlistCount; j++)
                    {
                        if (trackPaths[i].Equals(currentPlaylist.trackList[j].FilePath))
                        {
                            trackDuplicate = true;
                            break;
                        }
                    }

                    if(!trackDuplicate) currentPlaylist.trackList.Add(new Track(trackPaths[i]));
                }
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void RemoveTrackButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = PlayListListView.SelectedItems.Cast<Track>().ToList();
            int amount = selected.Count;
            Mouse.OverrideCursor = Cursors.Wait;
            for (int i = 0; i < amount; i++)
                currentPlaylist.trackList.Remove(selected[i]);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void PlaylistTrack_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Track;
            if (item != null)
            {
                TrackNameTextBlock.Text = item.Name;
                MessageBox.Show("Item's Double Click handled!");
            }
        }

    }
}