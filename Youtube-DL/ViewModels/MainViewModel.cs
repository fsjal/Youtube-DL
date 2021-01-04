using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Youtube_DL.Entities;
using Youtube_DL.Models;
using Youtube_DL.ViewModels.Base;
using static Youtube_DL.Entities.Link;

namespace Youtube_DL.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region Source
        public ObservableCollection<Link> Links { get; set; } = new ObservableCollection<Link>();
        public int SelectedIndex { get; set; }
        #endregion Source

        private Link currentLink;

        #region Commands
        public RelayCommand NewCommand { get; }
        public RelayCommand OpenCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand RemoveCommand { get; }
        public RelayCommand ExtractAudioCommand { get; }
        public RelayCommand ItemClickCommand { get;  }
        public RelayCommand ExtractAudioItemCommand { get; }
        public RelayCommand CopyLinkCommand { get; }
        private RelayCommand downloadCommand;
        public RelayCommand DownloadCommand
        {
            get { return downloadCommand; }
            set { downloadCommand = value; OnPropertyChanged(); }
        }

        #endregion Commands

        #region Download
        private string downloadContent = "Download";

        public string DownloadImage
        {
            get { return downloadContent; }
        }

        public string DownloadContent
        {
            get { return downloadContent; }
            set { downloadContent = value; OnPropertyChanged(); OnPropertyChanged(nameof(DownloadImage)); }
        }
        #endregion Download

        private bool isWorking;

        public bool IsWorking
        {
            get { return isWorking; }
            set {
                isWorking = value;
                if (IsWorking)
                {
                    DownloadContent = "Stop";
                    DownloadCommand = new RelayCommand(e => OnStopDownload(), e => true);
                } else
                {
                    DownloadContent = "Download";
                    DownloadCommand = new RelayCommand(e => OnStartDownloadAsync(), e => true);
                }
            
            }
        }

        private bool extractAudioAll;

        public bool ExtractAudioAll
        {
            get { return extractAudioAll; }
            set { extractAudioAll = value; OnPropertyChanged(); }
        }


        private CancellationTokenSource tokenSource;

        public MainViewModel()
        {
            AddCommand = new RelayCommand(e => AddLinkFromClipboard(), e => true);
            DownloadCommand = new RelayCommand(e => OnStartDownloadAsync(), e => Links.Count > 0);
            NewCommand = new RelayCommand(e => OnNew(), e => true);
            OpenCommand = new RelayCommand(e => OnOpen(), e => true);
            SaveCommand = new RelayCommand(e => OnSave(), e => Links.Count > 0);
            RemoveCommand = new RelayCommand(e => OnRemove(e), e => SelectedIndex != -1 && !IsWorking);
            ItemClickCommand = new RelayCommand(e => OnLinkClicked(e), e => true);
            ExtractAudioCommand = new RelayCommand(e => OnExtractAudio(e), e => true);
            ExtractAudioItemCommand = new RelayCommand(e => OnExtractAudioItem(), e => true);
            CopyLinkCommand = new RelayCommand(e => OnCopyLink(e), e => true);
            DownloadClient.OnInfoParsed += IfInfoParsed;
        }

        private void OnCopyLink(object e)
        {
            Clipboard.SetText((string)e);
        }

        private void OnExtractAudioItem()
        {
            ExtractAudioAll = Links.All(link => link.ExtractAudio);
        }

        private void OnExtractAudio(object e)
        {
            bool isPressed = (bool)e;
            foreach (Link link in Links) link.ExtractAudio = isPressed; 
        }

        private void OnLinkClicked(object e)
        {
            Link link = (Link)e;
            if (link != null && link.LinkStatus == Status.Finished && link.FilePath != null) OpenMedia(link);
        }

        private void OnRemove(object e)
        {
            System.Collections.IList selectedLinks = (System.Collections.IList)e;
            List<Link> items = new List<Link>(selectedLinks.Count);
            foreach (Link link in selectedLinks) {
                items.Add(link);
            }
            foreach(Link link in items)
            {
                Links.Remove(link);
            }
        }

        private void OnSave()
        {
            var dialog = new VistaSaveFileDialog();
            dialog.DefaultExt = "txt";
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; ;
            if ((bool)!dialog.ShowDialog()) return;
            
            string filePath = dialog.FileName;
            File.WriteAllLines(filePath, Links.Select(link => link.Url));
        }

        private void OnOpen()
        {
            var dialog = new VistaOpenFileDialog();
            dialog.DefaultExt = "txt";
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"; ;
            if ((bool)!dialog.ShowDialog()) return;
            
            string filePath = dialog.FileName;
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines) AddLink(line);
        }

        private void OnNew()
        {
            Links.Clear();
        }

        private async void OnStartDownloadAsync()
        {
            var dialog = new VistaFolderBrowserDialog
            {
                RootFolder = System.Environment.SpecialFolder.MyDocuments
            };
            if ((bool)!dialog.ShowDialog()) return;

            string directory = dialog.SelectedPath;
            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            IsWorking = true;
            try
            {
                foreach (Link link in Links)
                {
                    if (link.LinkStatus == Status.Finished) continue;
                    currentLink = link;
                    link.LinkStatus = Status.Downloading;
                    string cmd = link.ExtractAudio ? $"-x --audio-format m4a {link.Url}" : link.Url;
                    var process = new ProcessCommand(cmd, directory, token);
                    await Task.Run(() => DownloadClient.downloadAsync(process, token), token);
                    if (link.LinkStatus == Status.Failed)
                    {
                        link.Eta = "Failed";
                        continue;
                    }
                    link.LinkStatus = Status.Finished;
                    link.Progress = 100.0;
                    link.Eta = "Finished";
                    try
                    {
                        NormalizeName(directory);
                    } catch(Exception)
                    {

                    }
                }
            } catch (OperationCanceledException)
            {
                currentLink.LinkStatus = Status.Failed;
                currentLink.Eta = "Canceled";
            }
            IsWorking = false;
            tokenSource.Dispose();
        }
        
        private void OnStopDownload()
        {
            tokenSource.Cancel();
        }

        private void IfInfoParsed(Information info, string content)
        {
            switch(info)
            {
                case Information.Name:
                    currentLink.Name = content;
                    break;
                case Information.Size:
                    currentLink.Size = content;
                    break;
                case Information.Speed:
                    currentLink.Speed = content;
                    break;
                case Information.Eta:
                    currentLink.Eta = content;
                    break;
                case Information.Progress:
                    currentLink.Progress = double.Parse(content);
                    break;
                case Information.Error:
                    currentLink.LinkStatus = Status.Failed;
                    currentLink.Name = content;
                    break;
            }
        }

        private void AddLink(string url)
        {
            if (url != null && url.StartsWith("http"))
            {
                Links.Add(new Link(url));
                ExtractAudioAll = false;
            }
        }

        private void AddLinkFromClipboard()
        {
            AddLink(GetClipboardContent());
        }

        private void NormalizeName(string directory)
        {
            string name = currentLink.Name;
            string newName = name.Substring(0, name.LastIndexOf('-')) + name.Substring(name.LastIndexOf('.'));
            string oldPath = $"{directory}/{name}";
            string newPath = $"{directory}/{newName}";
            string salt = new Random().Next(1, 10).ToString();

            while (File.Exists(newPath))
            {
                newName = name.Substring(0, name.LastIndexOf('-')) + salt + name.Substring(name.LastIndexOf('.'));
                newPath = $"{directory}/{newName}";
                salt += new Random().Next(1, 10).ToString();
            }
            File.Move(oldPath, newPath);
            currentLink.FilePath = newPath;
        }

        private string GetClipboardContent()
        {
            string content = null;
            if (Clipboard.ContainsText(TextDataFormat.Text)) content = Clipboard.GetText(TextDataFormat.Text);
            return content;
        }

        private void OpenMedia(Link link)
        {
            if (!File.Exists(link.FilePath)) return;
            using (Process process = new Process())
            {
                process.StartInfo.FileName = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
                process.StartInfo.Arguments = $"\"{link.FilePath}\"";
                process.Start();
            }
        }
    }
}
