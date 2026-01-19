# K.A.T.I.E.

**K**ollektív **A**udio**T**ábla **I**nformációs **E**lemekből

**K**ollektive **A**udio **T**able of **I**nformational **E**lements

---

A phrase-based concatenating speech synthesis program.

Detects the longest phrase possible (by iterating through words) and plays the audio clips one by one.

> [!TIP]
> See also: [usage](#usage)

---

This project has a number of open-source dependencies. See [Attributions](ATTRIBUTIONS.md)

# Original Idea

I wanted to replace the announcer "C.A.S.S.I.E." in SCP: Secret Laboratory
with the MÁV (Hungarian State Railways) announcer's voice (Mátyus Katalin, hence the name of the project).

At this time I don't have the rights to use Kati's voice, so the app requires audio samples from other sources.

# Web Version

> [!IMPORTANT]
> The app is not compatible with portrait screens.
> Make sure your device/display is in a landscape (wide) orientation.
> Currently, the app only supports `wav` files.

The web app is available [here](https://axwabo.github.io/K.A.T.I.E./)

It has some limitations, most notably performance-wise.

A sample phrase pack is available:

1. Download the zip from [here](https://drive.google.com/file/d/1JN5E3bGD6bHFEtJVA5a32WDXQDB3bsPe/view?usp=sharing)
2. Go into the app's `Cache` tab
3. Click `Load From Archive`
4. Select the file, and wait for it to load

> [!CAUTION]
> Leaving the page will discard any phrases/signals that were not cached.
> Click `Cache Everything` to make sure they'll be restored in later sessions.

> [!NOTE]
> If the app crashes, it does so silently.
> When the UI becomes unresponsive, give it a few seconds (it might be processing your request).
> After multiple seconds of unresponsiveness, reload the page.

# Installation

## Desktop UI

1. Make sure you have [.NET Desktop Runtime 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) installed
2. Download the platform-specific archive from the [releases page](https://github.com/Axwabo/K.A.T.I.E./releases)
3. Extract the `bin/Katie.UI.Desktop` (.exe on Windows) file
4. Run the executable
    - You'll probably need to `chmod +x` it on Linux

> [!TIP]
> See also: [desktop usage](#desktop-usage)

## SecretLab

> [!NOTE]
> This part of the setup guide is for SCP: Secret Laboratory servers.
> Check out [this guide](https://techwiki.scpslgame.com/books/server-guides/page/1-how-to-create-a-dedicated-server)
> on how to set up an SCP:SL server.

1. Install [SecretLabNAudio](https://github.com/Axwabo/SecretLabNAudio)
2. Download the following files from the [releases page](https://github.com/Axwabo/K.A.T.I.E./releases):
    - `Katie.Core.dll`
    - `Katie.NAudio.dll`
3. Download and extract [Harmony 2.2.2](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0)
    1. Download the `Harmony.2.2.2.0.zip` asset
    2. Extract the `net48/0Harmony.dll` file from the archive
4. Place the downloaded files in the **dependencies** directory
    - Linux: `.config/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
    - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
5. Download the `Katie.SecretLab.dll` file from the releases page
6. Place the file in the **plugins** directory
    - Linux: `.config/SCP Secret Laboratory/LabAPI/plugins/<port>/`
    - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/<port>/`
7. Restart the server
8. [Configure](#config) phrases
9. Restart the server again

### Config

The configuration root directory is:

- Linux: `.config/SCP Secret Laboratory/LabAPI/config/<port>/K.A.T.I.E. TTS`
- Windows: `%appdata%/SCP Secret Laboratory/LabAPI/dependencies/<port>/K.A.T.I.E. TTS`

The `config.yml` file contains some properties:

- Enabling `ReplaceCassie` will override base-game announcements to be spoken by K.A.T.I.E. (not fully supported yet)
- `DefaultLanguage` sets the language of replaced announcements
- `DefaultSignal` sets the signal to be played before replaced announcements

Place phrases in `Phrases` subdirectories (`Hungarian` `English` `Global`).

To assign aliases, create an `aliases.txt` file in the language subdirectory.
Aliases are defined on every line like so: `alias = original` (spaces are trimmed).

Place signals in the `Signals` directory.

> [!TIP]
> The full installation of SecretLabNAudio allows for `mp3` and `ogg` files to be read as well.
> If only the Core is installed, reading is limited to `wav` files.
> Neither sample rate nor channel count matter, regardless of file format.

The C.A.S.S.I.E. Remote Admin GUI supports announcing with K.A.T.I.E., even if `ReplaceCassie` is set to false.
Examples:

- Announcement: `[Hungarian]`<br>Custom subtitles: `Az 1. vágányra szerelvény érkezik.`
- Announcement: `[English] [EC] `<br>Custom subtitles: `cassie_sl EuroCity train Semmelweis is arriving at platform two.`

The language must be specified first, which may be followed by an optional signal.

> [!NOTE]
> If an announcement is played with a signal, background noise will be disabled.

> [!TIP]
> See also: [usage](#usage)

## Unity

For Unity projects (where you have access to the editor), download the
`Katie.Core.dll` and `Katie.Unity.dll` files, and place them in the `Assets/Plugins` directory.

Then, you can create scriptable objects: `PhrasePack` and `Signal`

Use the `QueuePlayer` script and the `QueuePlayerExtensions.EnqueueAnnouncement`
extension method to play announcements through an `AudioSource`

# Desktop Usage

> [!IMPORTANT]
> Currently, the app only supports `wav` files.

To automatically load phrases and signals, create directories in the same folder the executable is in.

Place phrases in `Phrases` subdirectories (`Hungarian` `English` `Global`).

Place signals in the `Signals` directory.

Caching will only load phrases into memory, it won't save them to any "special" folder.

# Usage

First, add phrases to be loaded, or load them at runtime if using the app.

Global phrases will be considered when parsing either language.

Phrase names are case-insensitive, however, spaces and dashes must be taken into account.
The file name is used to set phrase text, however, you can define aliases in the app by clicking
the ✏ or `Alias` button.

Examples:

- `passenger train` - valid
- `passenger  train` - invalid (two consecutive spaces)
- `Budapest-Keleti` - will match `Budapest-Keleti` but not `Budapest Keleti` nor `Budapest -Keleti`

The parser steps word-by-word (delimited by spaces or `.` or `,`)
and tries to add a known phrase with the longest joined text.

If a word doesn't map to any known phrase, it will be replaced with silence.

## Numbers

### Hungarian

Numbers 0-9999 can be parsed, suffixes are supported.

Phrases must be loaded as follows:

- 0-9 digits separately like `egy` `kettő` `három`
- Ordinal digits separately like `első` `második` `harmadik`
- Tens like `tíz` `húsz` `harminc`
- Tenths like `tizen` `huszon` `harminc` (must be separated from the ones)
- Hundred as `száz`
- Thousand as `ezer`
- `óra` and `perc` literals for time parsing

### English

Numbers 0-999 can be parsed, suffixes are not supported.
Numbers above 1000 are parsed per digit as ones.

Phrases must be loaded as follows:

- 0-9 digits separately like `one` `two` `three`
- Ordinal digits separately like `first` `second` `third`
- Tens like `ten` `twenty` `thirty`
- Tenths like `eleven` `twelve` `thirteen` (must be separated from the ones)
- Hundred as `hundred`
- `o'clock` literal for time parsing

## Time

The is `hh:mm` for time phrases.

### Hungarian

The leading zero is trimmed, suffixes are supported.

- `07:50` maps to `hét` `óra` `ötven` `perc`
- `12:00-kor` maps to `tizen` `két` `órakor`

### English

Suffixes are not supported.

- `07:50` maps to `oh` `seven` `fifty`
- `12:00` maps to `twelve` `o'clock`
