using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using SnapJudgement.Models;
using SQLite;

namespace SnapJudgement
{
    public class ContestantsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Contestant> contestants;
        private SQLiteConnection database;
        public static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Contestants.sjdat");

        public ObservableCollection<Contestant> Contestants
        {
            get => contestants;
            set
            {
                contestants = value;
                OnPropertyChanged(); // MAUI delegate for updating the UI
            }
        }

        public ContestantsViewModel()
        {
            contestants = new ObservableCollection<Contestant>();
        }

        public void AddContestant(Contestant contestant)
        {
            if (contestants.Any(c => c.Name == contestant.Name))
            {
                return;
            }

            contestants.Add(contestant);
            SortAndUpdateRankings();
        }

        public void ClearContestants()
        {
            contestants.Clear();
            SortAndUpdateRankings();
        }

        private void SortAndUpdateRankings()
        {
            var sortedList = contestants.OrderByDescending(c => c.OverallScore).ToList();

            int currentRank = 1;
            int previousScore = 41; // larger than the maximum possible score
            int sameRankCount = 0;

            foreach (var contestant in sortedList)
            {
                if (contestant.OverallScore < previousScore)
                {
                    currentRank += sameRankCount; // increment rank by the number of contestants potentially sharing the same rank above this contestant
                    sameRankCount = 1; // reset this to 1, because we have a new score
                    previousScore = contestant.OverallScore;
                }
                else
                {
                    sameRankCount++; // no increase in rank, because they come joint nth
                }

                contestant.Ranking = currentRank;
            }

            Contestants = new ObservableCollection<Contestant>(sortedList);
        }

        // Event handler to update the UI when properties change
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DBStatus LoadContestantsFromDatabase()
        {
            List<Contestant> newContestants = new();
            try
            {
                database = new SQLiteConnection(DatabasePath);
                database.CreateTable<Contestant>(); //create table makes an existing table available as an object or makes a new one

                newContestants = database.Table<Contestant>().ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DBERROR: Exception thrown while loading from database:\n{ex.ToString()}");
                return DBStatus.LoadFail;
            }

            if (newContestants.Count == 0)
            {
                return DBStatus.LoadEmpty;
            }

            contestants.Clear();
            foreach (var contestant in newContestants)
            {
                contestants.Add(contestant);
            }
            SortAndUpdateRankings();

            return DBStatus.LoadSuccess;
        }

        public DBStatus SaveContestantsToDatabase()
        {
            try
            {
                database = new SQLiteConnection(DatabasePath);
                database.CreateTable<Contestant>();
                foreach (var contestant in contestants)
                {
                    database.InsertOrReplace(contestant);
                }
                return DBStatus.SaveSuccess; 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DBERROR: Exception thrown while saving database:\n{ex.ToString()}");
                return DBStatus.SaveFail;
            }
        }
        public DBStatus ClearDatabase()
        {
            try
            {
                database = new SQLiteConnection(DatabasePath);
                database.CreateTable<Contestant>();
                foreach (var contestant in contestants)
                {
                    database.Delete(contestant);
                }
                return DBStatus.ClearSuccess; 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DBERROR: Exception thrown while clearing database:\n{ex.ToString()}");
                return DBStatus.ClearFail;
            }
        }
    }
}