using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SnapJudgement.Models;
using SQLite;

namespace SnapJudgement
{
    public partial class MainPage : ContentPage
    {
        private ContestantsViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();

            viewModel = new();
            BindingContext = viewModel;
        }

        private void btnEnter_Clicked(object sender, EventArgs e)
        {
            if (!int.TryParse(txtportraitEntry.Text, out int portraitScore) || portraitScore > 10 || portraitScore < 1)
            {
                DisplayAlert("Error", "Please enter a valid score for the contestant's portrait entry.", "OK");
                return;
            }
            if (!int.TryParse(txtmacroEntry.Text, out int macroScore) || macroScore > 10 || macroScore < 1)
            {
                DisplayAlert("Error", "Please enter a valid score for the contestant's macro entry.", "OK");
                return;
            }
            if (!int.TryParse(txtpanoramicEntry.Text, out int panoramicScore) || panoramicScore > 10 || panoramicScore < 1)
            {
                DisplayAlert("Error", "Please enter a valid score for the contestant's panoramic entry.", "OK");
                return;
            }
            if (!int.TryParse(txtwildcardEntry.Text, out int wildcardScore) || wildcardScore > 10 || wildcardScore < 1)
            {
                DisplayAlert("Error", "Please enter a valid score for the contestant's wildcard entry.", "OK");
                return;
            }

            viewModel.AddContestant(new Contestant(txtnameEntry.Text, portraitScore, macroScore, panoramicScore, wildcardScore));

            txtnameEntry.Text = string.Empty;
            txtportraitEntry.Text = string.Empty;
            txtmacroEntry.Text = string.Empty;
            txtpanoramicEntry.Text = string.Empty;
            txtwildcardEntry.Text = string.Empty;
            btnSave.IsEnabled = true;
        }
        private void ContestantFormChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtnameEntry.Text) ||
                String.IsNullOrWhiteSpace(txtportraitEntry.Text) ||
                String.IsNullOrWhiteSpace(txtmacroEntry.Text) ||
                String.IsNullOrWhiteSpace(txtpanoramicEntry.Text) ||
                String.IsNullOrWhiteSpace(txtwildcardEntry.Text))
            {
                btnEnter.IsEnabled = false;
            }
            else
            {
                btnEnter.IsEnabled = true;
            }
        }

        private void btnClearAll_Clicked(object sender, EventArgs e)
        {
            Task<bool> response = DisplayAlert("Clear All", "Are you sure you want to clear all contestants?", "Yes", "No");
            if (response.Result == true)
            {
                viewModel.ClearContestants();
            }
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Contestants.Count == 0)
            {
                Task<bool> response = DisplayAlert("Clear All", "Are you sure you want to clear the database?", "Yes", "No");
                if (response.Result == true)
                {
                    ResultAlertHelper(viewModel.ClearDatabase());
                }
            }
            ResultAlertHelper(viewModel.SaveContestantsToDatabase());
        }

        private void btnLoad_Clicked(object sender, EventArgs e)
        {
            ResultAlertHelper(viewModel.LoadContestantsFromDatabase());
        }

        private void ResultAlertHelper(DBStatus status)
        {
            switch (status)
            {
                case DBStatus.LoadSuccess:
                    DisplayAlert("Success", "Contestant records loaded successfully.", "OK");
                    break;
                case DBStatus.LoadEmpty:
                    DisplayAlert("Information", "Database contained no records for this user.", "OK");
                    break;
                case DBStatus.LoadFail:
                    DisplayAlert("Error", "An error occurred while loading contestant records.\nContact support if the issue persists.", "OK");
                    break;
                case DBStatus.SaveSuccess:
                    DisplayAlert("Success", "Contestant records saved successfully.", "OK");
                    break;
                case DBStatus.SaveFail:
                    DisplayAlert("Error", "An error occurred while saving contestant records.\nContact support if the issue persists.", "OK");
                    break;
                case DBStatus.ClearSuccess:
                    DisplayAlert("Success", "Contestant records cleared successfully.", "OK");
                    break;
                case DBStatus.ClearFail:
                    DisplayAlert("Error", "An error occurred while clearing contestant records.\nContact support if the issue persists.", "OK");
                    break;
                default:
                    Debug.WriteLine($"DBERROR: Invalid DBStatus \"{status}\" returned.");
                    DisplayAlert("Error", "An unknown error occurred.\nContact support if the issue persists.", "OK");
                    break;
            }
        }
    }
}


