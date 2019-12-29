using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        Model model;

        //Threads
        BackgroundWorker AddTracks;

        DispatcherTimer _timer;
      
        MusicBox MusicBox;

        //more stuffz
        bool isUserDragging = false;

        //List for saving data of random play mode
        List<int> listIndexForRandomPlayMode;
        List<int> trackingPlayedTrack;

        public MainWindow()
        {
            InitializeComponent();

            // Set up timer 
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += timer_Tick;

        }

        //-------------------------Threads-------------------------

        private void AddTracks_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] trackPaths = (string[])e.Argument;
           
            int playlistCount = currentPlaylist.TrackCount;
            for (int i = 0; i < trackPaths.Length; i++)
            {
                bool trackDuplicate = false;
                
                for (int j = 0; j < playlistCount; j++)
                {
                    if (trackPaths[i].Equals(currentPlaylist.TrackList[j].FilePath))
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
            currentPlaylist.addTrack(track);
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
            model = Model.GetInstance();
            MusicBox = MusicBox.getInstance();
            MusicBox.SetTrackEndedEvent(LoopMode_trackEndEventHandler);
            MusicBox.SetMediaOpenedUIUpdate(MediaOpened_EventHandler);
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
                currentPlaylist.removeTrack(selected[i]);
            PlayListListView.ItemsSource = currentPlaylist.TrackList;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void PlaylistTrack_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (currentPlaylist == null) return;
            if (currentPlaylist.currentTrackIdx != -1)
            {
                currentPlaylist.savePosition(currentPlaylist.currentTrackIdx, MusicBox.getCurrentPosition());
            }

            var item = ((FrameworkElement)e.OriginalSource).DataContext as Track;
            if (item != null)
            {
                MarqueeTrackName(item.Name);
                if (item.FilePath == null)
                {
                    MusicBox.getInstance().stopTrack();
                    TimeTextBlock.Text = "00:00 | 00:00";
                    return;
                }
                currentPlaylist.currentTrackIdx = PlayListListView.SelectedIndex;
                MusicBox.getInstance().playTrack(item.FilePath, _timer, item.position);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            if (currentPlaylist.currentTrackIdx != -1)
            {
                currentPlaylist.savePosition(currentPlaylist.currentTrackIdx, MusicBox.getCurrentPosition());
            }

            //if more than 1 track is selected
            if (PlayListListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please choose one and only one song");
                return;
            }

            //if no track is selected 
            if (PlayListListView.SelectedItems.Count == 0)
            {
                if (currentPlaylist.playMode == PLAY_MODE.RANDOM)
                {
                    //PlayListListView.SelectedIndex = listIndexForRandomPlayMode[currentIndexOfRandomPlayMode];
                    //currentIndexOfRandomPlayMode++;
                    PlayListListView.SelectedIndex = new Random().Next() % currentPlaylist.TrackCount;
                }
                else
                    //play the first track
                    PlayListListView.SelectedIndex = 0;
           
            }
            
            string buttonTag = (sender as Button).Tag?.ToString();

            //if playlist has been fully played, waiting to be played from the start
            if (buttonTag == "Replay")
            {
                
                (sender as Button).Tag = "";
                MusicBox.stopTrack();

                //if it's in random mode
                if (currentPlaylist.playMode == PLAY_MODE.RANDOM)
                {
                    //pick random a track to play
                    int pickedTrack;
                    do
                    {
                        pickedTrack = new Random().Next(100, 1000) % currentPlaylist.TrackCount;
                    } while (pickedTrack != PlayListListView.SelectedIndex); //avoid picking the same track that has already played

                    PlayListListView.SelectedIndex = pickedTrack;
                }
                else //if it's in sequential
                {
                    PlayListListView.SelectedIndex = 0;
                }
            }
            var item = PlayListListView.SelectedItem as Track;
            MarqueeTrackName(item.Name);
            if (item.FilePath == null)
            {
                MusicBox.getInstance().stopTrack();
                TimeTextBlock.Text = "00:00 | 00:00";
                return;
            }
            if(currentPlaylist.currentTrackIdx == -1 && item.position != null)
            {
                item.position = null;
            }
            currentPlaylist.currentTrackIdx = PlayListListView.SelectedIndex;
            MusicBox.getInstance().playTrack(item.FilePath, _timer, item.position);

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            if (currentPlaylist.currentTrackIdx != -1)
            {
                currentPlaylist.savePosition(currentPlaylist.currentTrackIdx, null);
            }
            TimeTextBlock.Text = "Track stopped";
            currentPlaylist.currentTrackIdx = -1;
            MusicBox.getInstance().stopTrack();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null) return;
            if (currentPlaylist.currentTrackIdx != -1)
            {
                currentPlaylist.savePosition(currentPlaylist.currentTrackIdx, MusicBox.getCurrentPosition());
            }
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
            PlayListListView.ItemsSource = currentPlaylist.TrackList;
            PlayListNameTextBlock.Text = currentPlaylist.playlistName;
            if (LoopButton.Tag.ToString() == "On")
            {
                currentPlaylist.loopMode = LOOP_MODE.INFINITE;
            }

            if (ShuffleButton.Tag.ToString() == "On")
            {
                currentPlaylist.playMode = PLAY_MODE.RANDOM;
            }

            trackingPlayedTrack = new List<int>();
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
            if (!MusicBox.getInstance().isPlaying) return;
            var currentPos = MusicBox.getInstance().getCurrentPosition();
            var duration = MusicBox.getInstance().getDuration();
            if(currentPos != null && duration != null && !isUserDragging)
            { 
                MusicSlider.Minimum = 0;
                MusicSlider.Maximum = duration.Value.TotalMilliseconds;
                MusicSlider.Value = currentPos.Value.TotalMilliseconds;
            }
            TimeTextBlock.Text = $"{currentPos?.ToString(@"mm\:ss")} | {duration?.ToString(@"mm\:ss")}";    
        }

        private void SavePlayListButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                MessageBox.Show("There are no playlist");
                return;
            }
            model.SavePlayList(currentPlaylist);
        }

        private void LoadPlayListButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPlayListDialog dialog = new LoadPlayListDialog();
            dialog.NewPlaylistEvent += NewPlaylist_NewPlaylistEvent;
            dialog.ShowDialog();
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Tag.ToString() == "Off")
            {
                button.Tag = "On";
                if (currentPlaylist != null)
                {
                    currentPlaylist.loopMode = LOOP_MODE.INFINITE;
                }
            }
            else
            {
                button.Tag = "Off";
                if (currentPlaylist != null)
                {
                    currentPlaylist.loopMode = LOOP_MODE.ONE_TIME;
                }
            }
        }

        private void LoopMode_trackEndEventHandler(object sender, EventArgs e)
        {
            TimeTextBlock.Text = $"{MusicBox.getInstance().getCurrentPosition()?.ToString(@"mm\:ss")} | " +
                $"{MusicBox.getDuration()?.ToString(@"mm\:ss")}";
            currentPlaylist.savePosition(currentPlaylist.currentTrackIdx, null);
            currentPlaylist.currentTrackIdx = -1;
            if (currentPlaylist.playMode == PLAY_MODE.RANDOM) // PLAYMODE RANDOM
            {

                if (trackingPlayedTrack.Contains(PlayListListView.SelectedIndex)  == false)
                {
                    trackingPlayedTrack.Add(PlayListListView.SelectedIndex);
                }

                if (trackingPlayedTrack.Count < currentPlaylist.TrackCount)
                {     
                    int pickedTrack;
                    do
                    {
                        pickedTrack = new Random().Next(100, 1000) % currentPlaylist.TrackCount;
                    } while (trackingPlayedTrack.Contains(pickedTrack));

                    //when success
                    PlayListListView.SelectedIndex = pickedTrack;
                    PlayButton_Click(PlayButton, null);
                }
                else
                {
                    trackingPlayedTrack.Clear();
                    if (currentPlaylist.loopMode == LOOP_MODE.INFINITE)
                    {
                        int pickedTrack = new Random().Next(100, 1000) % currentPlaylist.TrackCount;
                        PlayListListView.SelectedIndex = pickedTrack;
                        PlayButton_Click(PlayButton, null);
                    }
                    else
                    {
                        PlayButton.Tag = "Replay";
                        return;
                    }
                    
                }
            }

            if (currentPlaylist.playMode == PLAY_MODE.SEQUENTIAL) // PLAYMODE RANDOM
            {
                if (PlayListListView.SelectedIndex < PlayListListView.Items.Count - 1)
                {

                    PlayListListView.SelectedIndex++;
                    PlayButton_Click(PlayButton, null);
                }
                else
                {
                    //back to the start
                    if (currentPlaylist.loopMode == LOOP_MODE.INFINITE)
                    {
                        PlayListListView.SelectedIndex = 0;
                        PlayButton_Click(PlayButton, null);
                    }
                    else
                    {
                        PlayButton.Tag = "Replay";
                        return;
                    }
                }
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                return;
            }

            MusicBox.stopTrack();
            if (PlayListListView.SelectedIndex < PlayListListView.Items.Count - 1)
            {
                MusicBox.stopTrack();
                PlayListListView.SelectedIndex++;
            }
            else
            {
                PlayListListView.SelectedIndex = 0;
            }

            PlayButton_Click(PlayButton, null);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlaylist == null)
            {
                return;
            }

            if (PlayListListView.SelectedIndex > 0)
            {
                MusicBox.stopTrack();
                PlayListListView.SelectedIndex--;
            }
            else
            {
                //if this is already the first track
                StopButton_Click(StopButton, null);
            }
            PlayButton_Click(PlayButton, null);
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button.Tag.ToString() == "Off")
            {
                button.Tag = "On";
                if (currentPlaylist != null)
                {
                    trackingPlayedTrack.Clear();
                    currentPlaylist.playMode = PLAY_MODE.RANDOM;
                }
            }
            else
            {
                button.Tag = "Off";
                if (currentPlaylist != null)
                {
                    currentPlaylist.playMode = PLAY_MODE.SEQUENTIAL;
                }
            }


        }

        private void MediaOpened_EventHandler(object sender, EventArgs e)
        {
            TimeTextBlock.Text = $"{MusicBox.getInstance().getCurrentPosition()?.ToString(@"mm\:ss")} | " +
                $"{MusicBox.getDuration()?.ToString(@"mm\:ss")}";
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            MusicBox.JumTrack(5);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    {
                        BackwardButton_Click(BackwardButton, null);
                        break;
                    }
                case Key.Right:
                    {
                        ForwardButton_Click(ForwardButton, null);
                        break;
                    }
                case Key.Up:
                    {
                        BackButton_Click(BackButton, null);
                        break;
                    }
                case Key.Down:
                    {
                        NextButton_Click(NextButton, null);
                        break;
                    }
                case Key.Space:
                    {
                       if (MusicBox.isPlaying == true)
                        {
                            PauseButton_Click(PauseButton, null);
                        }
                       else
                        {
                            PlayButton_Click(PlayButton, null);
                        }
                        break;
                    }
            }
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            MusicBox.JumTrack(-5);
        }

        private void MusicSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isUserDragging = true;
        }

        private void MusicSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isUserDragging = false;
            if (currentPlaylist == null) return;
            if (currentPlaylist.currentTrackIdx != -1)
            {
                Track myTrack = currentPlaylist.getTrack(currentPlaylist.currentTrackIdx);
                if (myTrack.FilePath == null)
                {
                    MusicBox.getInstance().stopTrack();
                    TimeTextBlock.Text = "00:00 | 00:00";
                    return;
                }
                currentPlaylist.savePosition(currentPlaylist.currentTrackIdx, TimeSpan.FromMilliseconds(MusicSlider.Value));
                MusicBox.playTrack(myTrack.FilePath, _timer, TimeSpan.FromMilliseconds(MusicSlider.Value));
            }
        }
    }
}