using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace FFaudio.Net;

internal static partial class FFAudio
{
    private const string DllName = "ffaudio";
    
    public enum AU_LOG_LEVEL
    {
        INFO = 0,
        WARNING = 1,
        ERROR = 2,
        FATAL = 3
    }

    // Delegates (callbacks)
    // typedef void (*NotifyOfLog)(const char* message, int64_t request, enum AU_LOG_LEVEL level);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NotifyOfLog([MarshalAs(UnmanagedType.LPUTF8Str)] string message, long request, AU_LOG_LEVEL level);

    // typedef void (*NotifyOfEndOfFile)(bool is_eof_from_skip, bool is_from_error, int32_t handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NotifyOfEndOfFile([MarshalAs(UnmanagedType.I1)] bool is_eof_from_skip,
                                           [MarshalAs(UnmanagedType.I1)] bool is_from_error,
                                           int handle);

    // typedef void (*NotifyOfRestart)(double position, bool is_from_looping, int32_t remaining_loop_count);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NotifyOfRestart(double position,
                                         [MarshalAs(UnmanagedType.I1)] bool is_from_looping,
                                         int remaining_loop_count);

    // typedef void (*NotifyOfDurationUpdate)(double new_duration);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NotifyOfDurationUpdate(double new_duration);

    // typedef void (*NotifyOfPrepareNext)();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NotifyOfPrepareNext();
    

    // Structs
    [NativeMarshalling(typeof(InitializeConfigMarshaller))]
    [StructLayout(LayoutKind.Sequential)]
    public struct InitializeConfig
    {
        public string? AppName;
        public int InitialVolume;  // 0..100
        public int InitialLoopCount;  // -1 for infinite
        public NotifyOfLog? OnLog;
        public NotifyOfEndOfFile? OnEof;
        public NotifyOfRestart? OnRestart;
        public NotifyOfDurationUpdate? OnDurationUpdate;
        public NotifyOfPrepareNext? OnPrepareNext;
    }

    [NativeMarshalling(typeof(AudioDeviceConfigMarshaller))]
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioDeviceConfig
    {
        public string? AudioDevice;
        public int AudioDeviceIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EqualizerConfig
    {
        public double one_31Hz;
        public double two_63Hz;
        public double three_125Hz;
        public double four_250Hz;
        public double five_500Hz;
        public double six_1000Hz;
        public double seven_2000Hz;
        public double eight_4000Hz;
        public double nine_8000Hz;
        public double ten_16000Hz;
    }

    [NativeMarshalling(typeof(PlayAudioConfigMarshaller))]
    [StructLayout(LayoutKind.Sequential)]
    public struct PlayAudioConfig
    {
        // Optional; Seek this many seconds before starting playback. <= 0 == plays from the start
        public double SkipSeconds;
        // Optional; How many seconds to play audio before quiting. <= 0 == plays to the end
        public double PlayDuration;
        // Optional; NULL to disable. Add loudness normalization filter.
        // Ex: "I=-16:TP=-1.5:LRA=11:measured_I=-8.9:measured_LRA=5.2:measured_TP=1.1:measured_thresh=-19.1:offset=-0.8"
        public string? LoudnormSettings;
        // Optional; NULL to disable. Add crossfeed filter. Ex: "0.5"
        public string? CrossfeedSetting;
        // Optional; NULL to disable.
        // Used to insert your own audio filtergraph between the source `abuffer` and the `abuffersink`.
        // Setting this to anything will override any `loudnorm`,`crossfeed`, or `equalizer` values. See filtergraph.c
        //
        // Note that `abuffersink` will always resample to the preferred format of the current audio device, so
        // something like "aresample=44100, aformat=sample_fmts=s16:channel_layouts=stereo" would be pointless.
        //
        // Note 2, A new filtergraph is created for each call to au_play_audio() (or on_prepare_next),
        // effectively setting this back to NULL.
        //
        // Filters can be found here https://ffmpeg.org/ffmpeg-filters.html
        public string? AvFiltergraphOverride;
    }

    // Functions
    [LibraryImport(DllName, EntryPoint = "au_shutdown")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_shutdown();

    [LibraryImport(DllName, EntryPoint = "au_initialize")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial int au_initialize(InitializeConfig config);
    
    [LibraryImport(DllName, EntryPoint = "au_initialize")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial int au_initialize(IntPtr nullConfig);

    [LibraryImport(DllName, EntryPoint = "au_configure_audio_device")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial int au_configure_audio_device(AudioDeviceConfig custom_config);

    [LibraryImport(DllName, EntryPoint = "au_configure_audio_device")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial int au_configure_audio_device(IntPtr nullConfig);

    [LibraryImport(DllName, EntryPoint = "au_play_audio", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_play_audio(string filename, PlayAudioConfig config);

    [LibraryImport(DllName, EntryPoint = "au_play_audio", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_play_audio(string filename, IntPtr nullConfig);

    [LibraryImport(DllName, EntryPoint = "au_stop_audio")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_stop_audio();

    [LibraryImport(DllName, EntryPoint = "au_pause_audio")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_pause_audio([MarshalAs(UnmanagedType.I1)] bool value);

    [LibraryImport(DllName, EntryPoint = "au_seek_percent")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_seek_percent(double percentPos);

    [LibraryImport(DllName, EntryPoint = "au_seek_time")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_seek_time(long milliseconds); // int64_t

    [LibraryImport(DllName, EntryPoint = "au_set_audio_volume")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_set_audio_volume(int volume);

    [LibraryImport(DllName, EntryPoint = "au_get_audio_volume")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial int au_get_audio_volume();

    [LibraryImport(DllName, EntryPoint = "au_mute_audio")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_mute_audio([MarshalAs(UnmanagedType.I1)] bool value);

    [LibraryImport(DllName, EntryPoint = "au_set_loop_count")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_set_loop_count(int loop_count);

    [LibraryImport(DllName, EntryPoint = "au_get_loop_count")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial int au_get_loop_count();

    [LibraryImport(DllName, EntryPoint = "au_get_audio_play_time")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial double au_get_audio_play_time();

    [LibraryImport(DllName, EntryPoint = "au_get_audio_duration")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial double au_get_audio_duration();

    [LibraryImport(DllName, EntryPoint = "au_wait_loop")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void au_wait_loop();

    // int au_get_audio_devices(int *out_total, char ***out_devices);
    [LibraryImport(DllName, EntryPoint = "au_get_audio_devices")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static unsafe partial int au_get_audio_devices(int* out_total, sbyte*** out_devices);

    [LibraryImport(DllName, EntryPoint = "au_set_equalizer")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool au_set_equalizer(EqualizerConfig values);

    // Public wrappers
    public static int Initialize(InitializeConfig? cfg)
    {
        return cfg.HasValue ? au_initialize(cfg.Value) : au_initialize(IntPtr.Zero);
    }

    public static int ConfigureAudioDevice(AudioDeviceConfig? cfg)
    {
        return cfg.HasValue ? au_configure_audio_device(cfg.Value) : au_configure_audio_device(IntPtr.Zero);
    }

    public static void PlayAudio(string filename, PlayAudioConfig? cfg)
    {
        if (cfg.HasValue)
        {
            au_play_audio(filename, cfg.Value);
        }
        else
        {
            au_play_audio(filename, IntPtr.Zero);
        }
    }
    
    public static void Shutdown() => au_shutdown();

    public static void StopAudio() => au_stop_audio();

    public static void PauseAudio(bool pause) => au_pause_audio(pause);

    public static void SeekPercent(double percent) => au_seek_percent(percent);

    public static void SeekTime(long milliseconds) => au_seek_time(milliseconds);

    public static void SetVolume(int volume) => au_set_audio_volume(volume);

    public static int GetVolume() => au_get_audio_volume();

    public static void MuteAudio(bool mute) => au_mute_audio(mute);

    public static void SetLoopCount(int count) => au_set_loop_count(count);

    public static int GetLoopCount() => au_get_loop_count();

    public static double GetPlayTime() => au_get_audio_play_time();

    public static double GetDuration() => au_get_audio_duration();
    
    public static (int result, string[] devices) GetAudioDevices()
    {
        unsafe
        {
            int total;
            sbyte** devicesPtrs;
            int result = au_get_audio_devices(&total, &devicesPtrs);
        
            if (result != 0 || total <= 0 || devicesPtrs == null)
                return (result, Array.Empty<string>());

            var devices = new string[total];
            for (int i = 0; i < total; i++)
            {
                devices[i] = Marshal.PtrToStringUTF8((IntPtr)devicesPtrs[i]) ?? string.Empty;
            }
        
            return (result, devices);
        }
    }
    
    public static bool SetEqualizer(EqualizerConfig config) => au_set_equalizer(config);
}
