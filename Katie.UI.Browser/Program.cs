﻿using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Katie.UI;

internal sealed partial class Program
{

    private static Task Main(string[] args) => BuildAvaloniaApp()
        .WithInterFont()
        .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();

}
