namespace SnapJudgement
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new SplashScreen())
            {
                Width = 475,
                Height = 400,
                MaximumWidth = 475,
                MaximumHeight = 400,
                MinimumWidth = 475,
                MinimumHeight = 400,
                TitleBar = new TitleBar()
                {
                    IsVisible = false,
                    IsEnabled = false,

                }
            };

            return window;
        }
    }

}