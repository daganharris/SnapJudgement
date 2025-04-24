using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
using SnapJudgement.Models;

namespace SnapJudgement
{
    [Table("tblContestants")]
    public class Contestant : INotifyPropertyChanged
    {
        private string name;
        private int portraitScore;
        private int macroScore;
        private int panoramicScore;
        private int wildcardScore;
        private int ranking;

        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }
        [Indexed]
        public int UserID { get; set; }

        [NotNull, MaxLength(20)]
        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        [NotNull]
        public int PortraitScore
        {
            get => portraitScore;
            set { portraitScore = value; OnPropertyChanged(); OnPropertyChanged(nameof(OverallScore)); } //Both event triggers needed, one updates the score and the other retallies the rankings in the viewmodel
        }

        [NotNull]
        public int MacroScore
        {
            get => macroScore;
            set { macroScore = value; OnPropertyChanged(); OnPropertyChanged(nameof(OverallScore)); }
        }

        [NotNull]
        public int PanoramicScore
        {
            get => panoramicScore;
            set { panoramicScore = value; OnPropertyChanged(); OnPropertyChanged(nameof(OverallScore)); }
        }

        [NotNull]
        public int WildcardScore
        {
            get => wildcardScore;
            set { wildcardScore = value; OnPropertyChanged(); OnPropertyChanged(nameof(OverallScore)); }
        }

        [Ignore] // Computed property, thus not stored in the database
        public int OverallScore
        {
            get
            {
                return PortraitScore + MacroScore + PanoramicScore + WildcardScore;
            }
        }

        [Ignore] //Property for display only, thus not stored in the database
        public int Ranking
        {
            get => ranking;
            set { ranking = value; OnPropertyChanged(); }
        }

        // Empty constructor required by sqlite for some reason -?
        public Contestant() { }

        public Contestant(string name, int portraitScore, int macroScore, int panoramicScore, int wildcardScore)
        {
            Name = name;
            PortraitScore = portraitScore;
            MacroScore = macroScore;
            PanoramicScore = panoramicScore;
            WildcardScore = wildcardScore; 
            UserID = User.currentUser.ID; // relationship in tblContestants to this user in tblUsers
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}