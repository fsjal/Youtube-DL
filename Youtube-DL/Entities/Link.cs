using Youtube_DL.ViewModels.Base;

namespace Youtube_DL.Entities
{
    class Link : NotifyPropertyChangedBase
    {
        public enum Information {
            Name,
            Speed,
            Size,
            Eta,
            Progress,
            Error,
        }

        public enum Status
        {
            Waiting,
            Downloading,
            Failed,
            Finished,
        }

        public Link(string url)
        {
            Url = url;
            Name = url;
        }

        #region Url 
        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        #endregion Url

        #region Name
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }
        #endregion Name

        #region ExtractAudio
        private bool extractAudio;
        public bool ExtractAudio 
        {
            get { return extractAudio; }
            set { extractAudio = value; OnPropertyChanged(); }
        }
        #endregion ExtractAudio

        #region Size
        private string size;
        public string Size
        { 
            get { return size; }
            set { size = value; OnPropertyChanged(); }
        }
        #endregion Size

        #region Speed
        private string speed;
        public string Speed
        { 
            get { return speed; }
            set { speed = value; OnPropertyChanged(); }
        }
        #endregion Speed

        #region Eta
        private string eta = "Waiting";
        public string Eta
        { 
            get { return eta; }
            set { eta = value; OnPropertyChanged(); }
        }
        #endregion Eta

        #region Progress
        private double progress;
        public double Progress
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(); }
        }
        #endregion Progress

        #region FilePath
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        #endregion FilePath

        #region Status
        private Status linkStatus = Status.Waiting;

        public Status LinkStatus
        {
            get { return linkStatus; }
            set { linkStatus = value; OnPropertyChanged(); }
        }
        #endregion Status
    }
}
