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
        Playlist currentPlaylist;
        BindingList<Track> trackList;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            currentPlaylist = new Playlist("My playlist");
            trackList = new BindingList<Track>(currentPlaylist.trackList);
            PlayListListView.ItemsSource = trackList;
            PlayListNameTextBlock.Text = currentPlaylist.playlistName;
        }
    }
}