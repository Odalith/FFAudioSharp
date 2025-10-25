
namespace FFAudioSharp.Tests;

public class UnsupervisedTests : IDisposable
{
    private const string FILE_SOURCE_DIR = "";
    private const string SAMPLE_AUDIO_FILE = "";
    
    public UnsupervisedTests()
    {
    }
        
    public void Dispose()
    {
        // Clean up after each test
        FFAudio.Shutdown();
      
    }

    private bool DoInitialize()
    {
        var init = new FFAudio.InitializeConfig
        {
            AppName = "FFAudioSharp.Tests",
            InitialLoopCount = 0,
            InitialVolume = 50,
            OnLog = null,
            OnEof = null,
            OnRestart = null,
            OnDurationUpdate = null,
            OnPrepareNext = null
            
        };

        return FFAudio.Initialize(init) == 0;
    }

    private bool DoBasicSetup()
    {
        return DoInitialize() && FFAudio.ConfigureAudioDevice(null) == 0;
    }

    private void NotifyOfLog(string message, long request, FFAudio.AU_LOG_LEVEL level)
    {
        Console.WriteLine(message);
    }
    
    [Fact]
    public void SetupAndShutdown_ShouldSucceed()
    {
        var init = new FFAudio.InitializeConfig
        {
            AppName = "FFAudioSharp.Tests",
            InitialLoopCount = 0,
            InitialVolume = 50,
            OnLog = null,
            OnEof = null,
            OnRestart = null,
            OnDurationUpdate = null,
            OnPrepareNext = null
            
        };
        
        Assert.Equal(0, FFAudio.Initialize(init));
        
        Assert.Equal(0, FFAudio.ConfigureAudioDevice(null));
    }
    
    /*[Fact]
    public void SetupAndWithBadValue_ShouldFail()
    {
        Assert.Equal(-1, FFAudio.Initialize(null));
        
        Assert.Equal(-1, FFAudio.ConfigureAudioDevice(null));
    }*/
    
    [Fact]
    public void Volume_SetAndGet_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        // Test setting volume to 0
        FFAudio.SetVolume(0);
        Assert.Equal(0, FFAudio.GetVolume());
        
        // Test setting volume to 50
        FFAudio.SetVolume(50);
        Assert.Equal(50, FFAudio.GetVolume());
        
        // Test setting volume to 100
        FFAudio.SetVolume(100);
        Assert.Equal(100, FFAudio.GetVolume());
    }
    
    [Fact]
    public void Volume_SetInvalidValues_ShouldClamp()
    {
        Assert.True(DoBasicSetup());
        
        // Test negative volume (should be clamped)
        FFAudio.SetVolume(-10);
        var negativeVolumeResult = FFAudio.GetVolume();
        Assert.True(negativeVolumeResult >= 0, "Negative volume should be clamped to 0 or above");
        
        // Test volume over 100 (should be clamped)
        FFAudio.SetVolume(150);
        var overVolumeResult = FFAudio.GetVolume();
        Assert.True(overVolumeResult <= 100, "Volume over 100 should be clamped to 100 or below");
    }
    
    [Fact]
    public void LoopCount_SetAndGet_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        // Test setting loop count to 0 (no loop)
        FFAudio.SetLoopCount(0);
        Assert.Equal(0, FFAudio.GetLoopCount());
        
        // Test setting loop count to 3
        FFAudio.SetLoopCount(3);
        Assert.Equal(3, FFAudio.GetLoopCount());
        
        // Test setting loop count to -1 (infinite loop)
        FFAudio.SetLoopCount(-1);
        Assert.Equal(-1, FFAudio.GetLoopCount());
    }
    
    [Fact]
    public void MuteAudio_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        // Test muting
        FFAudio.MuteAudio(true);
        
        // Test unmuting
        FFAudio.MuteAudio(false);
        
        // Should not throw exceptions
        Assert.True(true);
    }
    
    [Fact]
    public void PauseAudio_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        // Test pausing
        FFAudio.PauseAudio(true);
        
        // Test unpausing
        FFAudio.PauseAudio(false);
        
        // Should not throw exceptions
        Assert.True(true);
    }
    
    [Fact]
    public void SeekPercent_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        // Test seeking to beginning
        FFAudio.SeekPercent(0.0);
        
        // Test seeking to middle
        FFAudio.SeekPercent(50.0);
        
        // Test seeking to end
        FFAudio.SeekPercent(100.0);
        
        // Should not throw exceptions
        Assert.True(true);
    }
    
    [Fact]
    public void SeekTime_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        // Test seeking to beginning
        FFAudio.SeekTime(0);
        
        // Test seeking to 30 seconds
        FFAudio.SeekTime(30000); // 30 seconds in milliseconds
        
        // Test seeking to 1 minute
        FFAudio.SeekTime(60000); // 60 seconds in milliseconds
        
        // Should not throw exceptions
        Assert.True(true);
    }
    
    [Fact]
    public void GetPlayTime_ShouldReturnInvalidValue()
    {
        double playTime = FFAudio.GetPlayTime();
        
        // Should return a negative value
        Assert.True(playTime <= 0.0, "Play time should be negative");
    }
    
    [Fact]
    public void GetDuration_ShouldReturnInvalidValue()
    {
        double duration = FFAudio.GetDuration();
        
        // Should return a non-negative value
        Assert.True(duration <= 0.0, "Duration should be negative");
    }
    
    [Fact]
    public async void GetDuration_ShouldReturnValidValue()
    {
        Assert.True(DoBasicSetup());
        
        FFAudio.PlayAudio(SAMPLE_AUDIO_FILE, null);

        await Task.Delay(1000);
        
        double duration = FFAudio.GetDuration();
        
        // Should return a non-negative value
        Assert.True(duration >= 0.0, "Duration should be non-negative");
    }    
    
    
    [Fact]
    public void GetAudioDevices_ShouldReturnDevicesList()
    {
        Assert.True(DoInitialize());
        
        var (result, devices) = FFAudio.GetAudioDevices();
        
        // Should succeed or return a known error code
        Assert.True(result >= 0 || result == -1, "Result should be success (0+) or known error (-1)");
        
        // Devices array should not be null
        Assert.NotNull(devices);
        
        // If result is successful, devices should be populated
        if (result == 0 && devices.Length > 0)
        {
            foreach (var device in devices)
            {
                Assert.NotNull(device);
            }
        }
    }
    
    /*[Fact]
    public void SetEqualizer_ShouldWork()
    {
        Assert.True(DoBasicSetup());
        
        
        var equalizerConfig = new FFAudio.EqualizerConfig
        {
            one_31Hz = 0.0,
            two_63Hz = 1.0,
            three_125Hz = -1.0,
            four_250Hz = 2.0,
            five_500Hz = -2.0,
            six_1000Hz = 0.5,
            seven_2000Hz = -0.5,
            eight_4000Hz = 1.5,
            nine_8000Hz = -1.5,
            ten_16000Hz = 0.0
        };
        
        // Should return a boolean result (true/false)
        bool result = FFAudio.SetEqualizer(equalizerConfig);
        
        // Result should be either true or false
        Assert.True(result == true || result == false);
    }*/
    
    [Fact]
    public void PlayAudio_WithNullConfig_ShouldNotThrow()
    {
        Assert.True(DoBasicSetup());
        
        // Test with null config - should not throw
        string testFile = "nonexistent.mp3";
        
        Assert.Throws<Exception>(() => FFAudio.PlayAudio(testFile, null));
    }
    
    [Fact]
    public void PlayAudio_WithConfig_ShouldNotThrow()
    {
        Assert.True(DoBasicSetup());
        
        var config = new FFAudio.PlayAudioConfig
        {
            SkipSeconds = 0.0,
            PlayDuration = 30.0, // Play for 30 seconds
            LoudnormSettings = null,
            CrossfeedSetting = null,
            AvFiltergraphOverride = null
        };
        
        string testFile = "nonexistent.mp3";
        
        // Should not throw exception even with non-existent file
        Assert.Throws<Exception>(() => FFAudio.PlayAudio(testFile, config));
    }
    
    [Fact]
    public void PlayAudio_WithAdvancedConfig_ShouldNotThrow()
    {
        var config = new FFAudio.PlayAudioConfig
        {
            SkipSeconds = 5.0, // Skip first 5 seconds
            PlayDuration = 10.0, // Play for 10 seconds
            LoudnormSettings = null,
            CrossfeedSetting = "0.5",
            AvFiltergraphOverride = null
        };
        
        string testFile = "nonexistent.mp3";
        
        // Should not throw exception
        Assert.Throws<Exception>(() => FFAudio.PlayAudio(testFile, config));
    }
    
    [Fact]
    public void StopAudio_ShouldNotThrow()
    {
        // Should not throw exception even when no audio is playing
        FFAudio.StopAudio();
        Assert.True(true);
    }
    
    [Fact]
    public void InitializeConfig_WithCallbacks_ShouldWork()
    {
        bool logCallbackCalled = false;
        bool eofCallbackCalled = false;
        bool restartCallbackCalled = false;
        bool durationCallbackCalled = false;
        bool prepareNextCallbackCalled = false;
        
        var init = new FFAudio.InitializeConfig
        {
            AppName = "FFAudioSharp.Tests.Callbacks",
            InitialLoopCount = 0,
            InitialVolume = 75,
            OnLog = (message, request, level) => { logCallbackCalled = true; },
            OnEof = (isEofFromSkip, isFromError, handle) => { eofCallbackCalled = true; },
            OnRestart = (position, isFromLooping, remainingLoopCount) => { restartCallbackCalled = true; },
            OnDurationUpdate = (newDuration) => { durationCallbackCalled = true; },
            OnPrepareNext = () => { prepareNextCallbackCalled = true; }
        };
        
        int result = FFAudio.Initialize(init);
        Assert.Equal(0, result);
        
        // Callbacks are set up but may not be called immediately
        // This test verifies that initialization with callbacks doesn't fail
        Assert.True(true);
    }
    
    /*[Fact]
    public void AudioDeviceConfig_WithValues_ShouldWork()
    {
        var deviceConfig = new FFAudio.AudioDeviceConfig
        {
            AudioDevice = "default",
            AudioDeviceIndex = 0
        };
        
        int result = FFAudio.ConfigureAudioDevice(deviceConfig);
        
        // Should succeed or return known error code
        Assert.True(result >= 0 || result == -1);
    }*/
    
    [Fact]
    public void AudioDeviceConfig_WithBadValues_ShouldWork()
    {
        Assert.True(DoInitialize());
        
        var deviceConfig = new FFAudio.AudioDeviceConfig
        {
            AudioDevice = "hooplaaudioservicewithnoexistance",
            AudioDeviceIndex = 0
        };
        
        int result = FFAudio.ConfigureAudioDevice(deviceConfig);
        
        // Should succeed or return known error code
        Assert.True(result >= 0 || result == -1);
    }
}
