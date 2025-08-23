using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Katie.UI.Browser;

internal static partial class CacheFunctions
{

    public const string Module = "cache";

    [JSImport("save", Module)]
    public static partial Task Save(string name, [JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> data);

    // [JSImport("list", Module)]

    [DebuggerNonUserCode]
    private static Task Load(Action<string, byte[]> callback)
    {
        if (__signature_Load_291778807 == null)
        {
            __signature_Load_291778807 = JSFunctionBinding.BindJSFunction("list", "cache", new[] {JSMarshalerType.Task(), JSMarshalerType.Action(JSMarshalerType.String, JSMarshalerType.Array(JSMarshalerType.Byte))});
        }

        Span<JSMarshalerArgument> __arguments_buffer = stackalloc JSMarshalerArgument[3];
        ref JSMarshalerArgument __arg_exception = ref __arguments_buffer[0];
        __arg_exception.Initialize();
        ref JSMarshalerArgument __arg_return = ref __arguments_buffer[1];
        __arg_return.Initialize();
        Task __retVal;
        // Setup - Perform required setup.
        ref JSMarshalerArgument __callback_native__js_arg = ref __arguments_buffer[2];
        // PinnedMarshal - Convert managed data to native data that requires the managed data to be pinned.
        __callback_native__js_arg.ToJS(callback, static (ref JSMarshalerArgument __delegate_arg_arg1, out string __delegate_arg1) => { __delegate_arg_arg1.ToManaged(out __delegate_arg1); }, static (ref JSMarshalerArgument __delegate_arg_arg2, out byte[] __delegate_arg2) => { __delegate_arg_arg2.ToManaged(out __delegate_arg2); });
        JSFunctionBinding.InvokeJS(__signature_Load_291778807, __arguments_buffer);
        // UnmarshalCapture - Capture the native data into marshaller instances in case conversion to managed data throws an exception.
        __arg_return.ToManaged(out __retVal);
        return __retVal;
    }

    static JSFunctionBinding? __signature_Load_291778807;

    public static async Task<Dictionary<string, byte[]>> LoadMemoryStream()
    {
        var list = new Dictionary<string, byte[]>();
        await Load(list.Add);
        return list;
    }

}
