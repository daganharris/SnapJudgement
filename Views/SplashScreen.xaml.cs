using System.ComponentModel;
using SnapJudgement;

namespace SnapJudgement;

public partial class SplashScreen : ContentPage
{
	public SplashScreen()
	{
		InitializeComponent();

        this.Loaded += SplashScreen_Loaded;
    }

    private async void SplashScreen_Loaded(object? sender, EventArgs e)
    {
        await Task.Delay(2000);
        Window main = new Window(new LoginScreen())
        {
            Width = 400,
            Height = 600,
            MaximumWidth = 400,
            MaximumHeight = 600,
            MinimumWidth = 400,
            MinimumHeight = 600,
            TitleBar = new TitleBar()
            {
                Title = "Snap Judgement - Login"
            }
        };
        Application.Current.OpenWindow(main);
        await Task.Delay(100); // give the win32 api chance to pull itself together
        Application.Current.CloseWindow(this.Window);
    }

}