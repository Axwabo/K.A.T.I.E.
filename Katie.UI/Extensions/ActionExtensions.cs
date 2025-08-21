namespace Katie.UI.Extensions;

public static class ActionExtensions
{

    public static void InvokeOnUIThread(this Action action) => Dispatcher.UIThread.Post(action);

}
