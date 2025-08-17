using System;
using Avalonia.Threading;

namespace Katie.UI;

public static class Extensions
{

    public static void InvokeOnUIThread(this Action action) => Dispatcher.UIThread.Post(action);

}
