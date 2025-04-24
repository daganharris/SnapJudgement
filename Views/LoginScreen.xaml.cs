using Microsoft.Maui.Controls;
using SnapJudgement.Models;

namespace SnapJudgement;

public partial class LoginScreen : ContentPage
{
	public LoginScreen()
	{
		InitializeComponent();
    }

    private async void CorrectUser(object sender, EventArgs e)
    {
        await Task.Delay(2000);
        Window main = new Window(new MainPage())
        {
            Width = 900,
            Height = 550,
            MaximumWidth = 900,
            MaximumHeight = 550,
            MinimumWidth = 900,
            MinimumHeight = 550,
            TitleBar = new TitleBar()
            {
                Title = "Snap Judgement - Score Entry"
            }
        };
        Application.Current.OpenWindow(main);
        await Task.Delay(100); 
        Application.Current.CloseWindow(this.Window);
    }

    private void Login_Clicked(object sender, EventArgs e)
    {
        if (!User.IsValidUsername(txtUsername.Text))
        {
            DisplayAlert("Error", "Please enter a valid username.", "OK");
            return;
        }
        if (!User.IsValidPassword(txtPassword.Text))
        {
            DisplayAlert("Error", "Please enter a valid password.", "OK");
            return;
        }
        if (User.VerifyPassword(txtUsername.Text, txtPassword.Text) == UserStatus.PasswordCorrect)
        {
            GC.Collect(); // remove plaintext password from memory forcefully using the gc
            CorrectUser(sender, e);
        } else
        {
            DisplayAlert("Error", "Incorrect username or password.", "OK");
        }

    }

    private async void CreateAccount_Clicked(object sender, EventArgs e)
    {
        if (!User.IsValidUsername(txtUsername.Text))
        {
            await DisplayAlert("Error", "Please enter a valid username.", "OK");
            return;
        }
        if (!User.IsValidPassword(txtPassword.Text))
        {
            await DisplayAlert("Error", "Please enter a valid password.", "OK");
            return;
        }
        UserStatus status = User.CreateUser(new User(txtUsername.Text, txtPassword.Text));
        if (status == UserStatus.CreateSuccess)
        {
            GC.Collect();
            await DisplayAlert("Success", "Account created successfully.", "OK");
            CorrectUser(sender, e);
        } else if (status == UserStatus.UserTaken)
        {
            await DisplayAlert("Error", "This username is already taken.", "OK");
        }
    }
}