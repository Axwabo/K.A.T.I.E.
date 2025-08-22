import { dotnet } from './_framework/dotnet.js'

const isBrowser = typeof window != "undefined";
if (!isBrowser)
    throw new Error(`Expected to be running in a browser`);

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

const exports = await dotnetRuntime.getAssemblyExports(config.mainAssemblyName);

globalThis.readFromProvider = exports.Katie.UI.Browser.WebAudioFunctions.ReadFromProvider;

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
