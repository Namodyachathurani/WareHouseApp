using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace WareHouseApp_Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;
            if (desktop.Args != null && System.Linq.Enumerable.Contains(desktop.Args, "--autodemo"))
            {
                mainWindow.TriggerAutoDemoOnStart();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}