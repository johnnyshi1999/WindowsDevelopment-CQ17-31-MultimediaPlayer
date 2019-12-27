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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WindowsDevelopment_CQ17_31_MultimediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //Playlist
        Playlist currentPlaylist;

        //Threads
        BackgroundWorker AddTracks;

        DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            // Set up timer 
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;

        }

        //-------------------------Threads-------------------------

        private void AddTracks_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] trackPaths = (string[])e.Argument;
           
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
                if (!trackDuplicate) AddTracks.ReportProgress(0, new Track(trackPaths[i]));

            }
        }

        private void AddTracks_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Track track = (Track)e.UserState;
            currentPlaylist.trackList.Add(track);
        }

        private void AddTracks_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        //--------------------------Events--------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Stuffz threadz
            AddTracks = new BackgroundWorker() {
               WorkerReportsProgress = true,
            };
            AddTracks.DoWork += AddTracks_DoWork;
            AddTracks.ProgressChanged += AddTracks_ProgressChanged;
            AddTracks.RunWorkerCompleted += AddTracks_RunWorkerCompleted;

            //Set stuffz
            TrackNameTextBlock.Measure(new Size(double.PositiveInfinity,
                                            double.PositiveInfinity));
            var Width = TrackNameTextBlock.DesiredSize.Width;
            Canvas.SetRight(TrackNameTextBlock, (TrackNameWrapper.ActualWidth - Width) / 2);
        }

        private void AddTrackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "MP3 Files(*.mp3;*.MP3)|*.mp3;*.MP3"
            };

            if (fileDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                AddTracks.RunWorkerAsync(fileDialog.FileNames);
            }
        }

        private void RemoveTrackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            var selected = PlayListListView.SelectedItems.Cast<Track>().ToList();
            int amount = selected.Count;
            Mouse.OverrideCursor = Cursors.Wait;
            for (int i = 0; i < amount; i++)
                currentPlaylist.trackList.Remove(selected[i]);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void PlaylistTrack_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentPlaylist == null) return;
            var item = ((FrameworkElement)e.OriginalSource).DataContext as Track;
            if (item != null)
            {
                MarqueeTrackName(item.Name);
                currentPlaylist.currentTrackIdx = PlayListListView.SelectedIndex;
                MusicBox.getInstance().playTrack(item.FilePath, _timer);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            if (PlayListListView.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please choose one and only one song");
                return;
            }
            var item = PlayListListView.SelectedItem as Track;
            MarqueeTrackName(item.Name);
            currentPlaylist.currentTrackIdx = PlayListListView.SelectedIndex;
            MusicBox.getInstance().playTrack(item.FilePath, _timer);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            MusicBox.getInstance().stopTrack();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            MusicBox.getInstance().pauseTrack();
        }

        private void TrackUp_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
        }

        private void TrackDown_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
        }

        //New playlist dialog
        private void NewPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var newPlaylist = new NewPlaylistDialog();
            newPlaylist.NewPlaylistEvent += NewPlaylist_NewPlaylistEvent;
            newPlaylist.Show();
        }

        private void NewPlaylist_NewPlaylistEvent(Playlist playlist)
        {
            currentPlaylist = playlist;
            PlayListListView.ItemsSource = currentPlaylist.trackList;
            PlayListNameTextBlock.Text = currentPlaylist.playlistName;
        }

        //-----------------------Animations-----------------------
        private void MarqueeTrackName(string trackName)
        {
            TrackNameTextBlock.Text = trackName;
            TrackNameTextBlock.Measure(new Size(double.PositiveInfinity,
                                            double.PositiveInfinity));
            var Width = TrackNameTextBlock.DesiredSize.Width;
            double height = TrackNameWrapper.ActualHeight - TrackNameTextBlock.ActualHeight;
            TrackNameTextBlock.Margin = new Thickness(0, height / 2, 0, 0);
            
            Storyboard marquee = new Storyboard();
            //Marquee
            DoubleAnimation marqueeAnimation = new DoubleAnimation
            {
                From = -Width,
                To = TrackNameWrapper.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(10)),
            };
            Storyboard.SetTarget(marqueeAnimation, TrackNameTextBlock);
            Storyboard.SetTargetProperty(marqueeAnimation, new PropertyPath("(Canvas.Right)"));
            marquee.Children.Add(marqueeAnimation);

            //Apply
            marquee.Begin(this);

        }

        private void timer_Tick(object sender, EventArgs e)
        {
                var currentPos = MusicBox.getInstance().getCurrentPosition();
                var duration = MusicBox.getInstance().getDuration();
                TimeTextBlock.Text = $"{currentPos} | {duration}";    
        }
    }
}