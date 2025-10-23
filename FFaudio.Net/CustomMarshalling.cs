using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace FFaudio.Net;

[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(Utf8StringMarshaller))]
internal static class Utf8StringMarshaller
{
    public static IntPtr ConvertToUnmanaged(string? managed)
    {
        return managed is null ? IntPtr.Zero : Marshal.StringToCoTaskMemUTF8(managed);
    }

    public static string? ConvertToManaged(IntPtr unmanaged)
    {
        return unmanaged == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(unmanaged);
    }

    public static void Free(IntPtr unmanaged)
    {
        if (unmanaged != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(unmanaged);
        }
    }
}

[CustomMarshaller(typeof(FFAudio.NotifyOfLog), MarshalMode.Default, typeof(NotifyOfLogMarshaller))]
internal static class NotifyOfLogMarshaller
{
    public static IntPtr ConvertToUnmanaged(FFAudio.NotifyOfLog? managed)
    {
        return managed is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(managed);
    }

    public static FFAudio.NotifyOfLog? ConvertToManaged(IntPtr unmanaged)
    {
        return unmanaged == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<FFAudio.NotifyOfLog>(unmanaged);
    }

    public static void Free(IntPtr unmanaged) { /* No-op: Runtime manages delegate lifetime. */ }
}

[CustomMarshaller(typeof(FFAudio.NotifyOfEndOfFile), MarshalMode.Default, typeof(NotifyOfEndOfFileMarshaller))]
internal static class NotifyOfEndOfFileMarshaller
{
    public static IntPtr ConvertToUnmanaged(FFAudio.NotifyOfEndOfFile? managed) =>
        managed is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(managed);

    public static FFAudio.NotifyOfEndOfFile? ConvertToManaged(IntPtr unmanaged) =>
        unmanaged == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<FFAudio.NotifyOfEndOfFile>(unmanaged);

    public static void Free(IntPtr unmanaged) { /* No-op */ }
}

[CustomMarshaller(typeof(FFAudio.NotifyOfRestart), MarshalMode.Default, typeof(NotifyOfRestartMarshaller))]
internal static class NotifyOfRestartMarshaller
{
    public static IntPtr ConvertToUnmanaged(FFAudio.NotifyOfRestart? managed) =>
        managed is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(managed);

    public static FFAudio.NotifyOfRestart? ConvertToManaged(IntPtr unmanaged) =>
        unmanaged == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<FFAudio.NotifyOfRestart>(unmanaged);

    public static void Free(IntPtr unmanaged) { /* No-op */ }
}

[CustomMarshaller(typeof(FFAudio.NotifyOfDurationUpdate), MarshalMode.Default, typeof(NotifyOfDurationUpdateMarshaller))]
internal static class NotifyOfDurationUpdateMarshaller
{
    public static IntPtr ConvertToUnmanaged(FFAudio.NotifyOfDurationUpdate? managed) =>
        managed is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(managed);

    public static FFAudio.NotifyOfDurationUpdate? ConvertToManaged(IntPtr unmanaged) =>
        unmanaged == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<FFAudio.NotifyOfDurationUpdate>(unmanaged);

    public static void Free(IntPtr unmanaged) { /* No-op */ }
}

[CustomMarshaller(typeof(FFAudio.NotifyOfPrepareNext), MarshalMode.Default, typeof(NotifyOfPrepareNextMarshaller))]
internal static class NotifyOfPrepareNextMarshaller
{
    public static IntPtr ConvertToUnmanaged(FFAudio.NotifyOfPrepareNext? managed) =>
        managed is null ? IntPtr.Zero : Marshal.GetFunctionPointerForDelegate(managed);

    public static FFAudio.NotifyOfPrepareNext? ConvertToManaged(IntPtr unmanaged) =>
        unmanaged == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<FFAudio.NotifyOfPrepareNext>(unmanaged);

    public static void Free(IntPtr unmanaged) { /* No-op */ }
}

[CustomMarshaller(typeof(FFAudio.InitializeConfig), MarshalMode.ManagedToUnmanagedIn, typeof(InitializeConfigMarshaller))]
internal static class InitializeConfigMarshaller
{
    // Unmanaged layout: Exact match to C struct (use Sequential for packing).
    [StructLayout(LayoutKind.Sequential)]
    internal struct InitializeConfigUnmanaged
    {
        public IntPtr AppName; // char*
        public int InitialVolume;
        public int InitialLoopCount;
        public IntPtr OnLog;
        public IntPtr OnEof;
        public IntPtr OnRestart;
        public IntPtr OnDurationUpdate;
        public IntPtr OnPrepareNext;
    }

    public static InitializeConfigUnmanaged ConvertToUnmanaged(FFAudio.InitializeConfig managed)
    {
        return new InitializeConfigUnmanaged
        {
            AppName = Utf8StringMarshaller.ConvertToUnmanaged(managed.AppName),
            InitialVolume = managed.InitialVolume,
            InitialLoopCount = managed.InitialLoopCount,
            OnLog = NotifyOfLogMarshaller.ConvertToUnmanaged(managed.OnLog),
            OnEof = NotifyOfEndOfFileMarshaller.ConvertToUnmanaged(managed.OnEof),
            OnRestart = NotifyOfRestartMarshaller.ConvertToUnmanaged(managed.OnRestart),
            OnDurationUpdate = NotifyOfDurationUpdateMarshaller.ConvertToUnmanaged(managed.OnDurationUpdate),
            OnPrepareNext = NotifyOfPrepareNextMarshaller.ConvertToUnmanaged(managed.OnPrepareNext)
        };
    }

    public static void Free(InitializeConfigUnmanaged unmanaged)
    {
        Utf8StringMarshaller.Free(unmanaged.AppName);
    }
}

[CustomMarshaller(typeof(FFAudio.AudioDeviceConfig), MarshalMode.ManagedToUnmanagedIn, typeof(AudioDeviceConfigMarshaller))]
internal static class AudioDeviceConfigMarshaller
{
    // Unmanaged layout: Exact match to C struct (use Sequential for packing).
    [StructLayout(LayoutKind.Sequential)]
    internal struct AudioDeviceConfigUnmanaged
    {
        public IntPtr AudioDevice; // char*
        public int AudioDeviceIndex;
    }

    public static AudioDeviceConfigUnmanaged ConvertToUnmanaged(FFAudio.AudioDeviceConfig managed)
    {
        return new AudioDeviceConfigUnmanaged
        {
            AudioDevice = Utf8StringMarshaller.ConvertToUnmanaged(managed.AudioDevice),
            AudioDeviceIndex = managed.AudioDeviceIndex
        };
    }

    public static void Free(AudioDeviceConfigUnmanaged unmanaged)
    {
        Utf8StringMarshaller.Free(unmanaged.AudioDevice);
    }
}

[CustomMarshaller(typeof(FFAudio.PlayAudioConfig), MarshalMode.ManagedToUnmanagedIn, typeof(PlayAudioConfigMarshaller))]
internal static class PlayAudioConfigMarshaller
{
    // Unmanaged layout: Exact match to C struct (use Sequential for packing).
    [StructLayout(LayoutKind.Sequential)]
    public struct PlayAudioConfigUnmanaged
    {
        public double SkipSeconds;
        public double PlayDuration;
        public IntPtr LoudnormSettings; // char*
        public IntPtr CrossfeedSetting; // char*
        public IntPtr AvFiltergraphOverride; // char*
    }

    public static PlayAudioConfigUnmanaged ConvertToUnmanaged(FFAudio.PlayAudioConfig managed)
    {
        return new PlayAudioConfigUnmanaged
        {
            SkipSeconds = managed.SkipSeconds,
            PlayDuration = managed.PlayDuration,
            LoudnormSettings = Utf8StringMarshaller.ConvertToUnmanaged(managed.LoudnormSettings),
            CrossfeedSetting = Utf8StringMarshaller.ConvertToUnmanaged(managed.CrossfeedSetting),
            AvFiltergraphOverride = Utf8StringMarshaller.ConvertToUnmanaged(managed.AvFiltergraphOverride)
        };
    }

    public static void Free(PlayAudioConfigUnmanaged unmanaged)
    {
        Utf8StringMarshaller.Free(unmanaged.LoudnormSettings);
        Utf8StringMarshaller.Free(unmanaged.CrossfeedSetting);
        Utf8StringMarshaller.Free(unmanaged.AvFiltergraphOverride);
    }
}

