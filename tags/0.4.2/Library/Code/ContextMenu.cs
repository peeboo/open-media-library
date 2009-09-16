using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class ContextMenu : BaseModelItem
    {
        IList localItems;
        Choice subtitleTracks;
        Choice audioTracks;
        Choice chapterSelection;
        Boolean showAudioOptions = false;
        Boolean showSubtitleOptions = false;
        Boolean showChapterOptions = false;
        Boolean playDisk = false;

        public Boolean ShowAudioOptions
        {
            get { return showAudioOptions; }
            set
            {
                showAudioOptions = value;
                FirePropertyChanged("ShowAudioOptions");
            }
        }

        public Boolean ShowSubtitleOptions
        {
            get { return showSubtitleOptions; }
            set
            {
                showSubtitleOptions = value;
                FirePropertyChanged("ShowSubtitleOptions");
            }
        }

        public Boolean ShowChapterOptions
        {
            get { return showChapterOptions; }
            set
            {
                showChapterOptions = value;
                FirePropertyChanged("ShowChapterOptions");
            }
        }

        public Boolean PlayDisk
        {
            get { return playDisk; }
            set
            {
                playDisk = value;
                FirePropertyChanged("PlayDisk");
            }
        }

        public ContextMenu()
        {
            localItems = new List<ICommand>();
        }

        public ContextMenu(IModelItemOwner owner)
            : base()
        {
            localItems = new List<ICommand>();
        }

        public ContextMenu(IList LocalItems)
        {
            localItems = LocalItems;
        }

        public ContextMenu(IModelItemOwner owner, IList LocalItems)
            : base()
        {
            localItems = LocalItems;
        }

        public IList LocalItems
        {
            get { return localItems; }
        }

        public void AddAudioCommand(ICommand cmd)
        {
            localItems.Add(cmd);
            cmd.Invoked += new EventHandler(audioCommand_Invoked);
        }

        public void AddSubtitleCommand(ICommand cmd)
        {
            localItems.Add(cmd);
            cmd.Invoked += new EventHandler(subtitleCommand_Invoked);
        }

        public void AddChapterCommand(ICommand cmd)
        {
            localItems.Add(cmd);
            cmd.Invoked +=new EventHandler(chapterCommand_Invoked);
        }

        public void AddPlayCommand(ICommand cmd)
        {
            localItems.Add(cmd);
            cmd.Invoked +=new EventHandler(playCommand_Invoked);
        }

        public void audioCommand_Invoked(object sender, EventArgs e)
        {
            ShowAudioOptions = true;
        }

        public void subtitleCommand_Invoked(object sender, EventArgs e)
        {
            ShowSubtitleOptions = true;
        }

        public void chapterCommand_Invoked(object sender, EventArgs e)
        {
            ShowChapterOptions = true;
        }

        public void playCommand_Invoked(object sender, EventArgs e)
        {
            PlayDisk = true;
        }

        public Choice SubtitleTracksChoice
        {
            get { return subtitleTracks; }
            set
            {
                subtitleTracks = value;
                FirePropertyChanged("SubtitleTracks");
            }
        }

        public Choice AudioTracksChoice
        {
            get { return audioTracks; }
            set
            {
                audioTracks = value;
                FirePropertyChanged("AudioTracks");
            }
        }

        public Choice ChapterSelectionChoice
        {
            get { return chapterSelection; }
            set
            {
                chapterSelection = value;
                FirePropertyChanged("ChapterSelection");
            }
        }
    }
}
