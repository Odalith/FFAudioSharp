using Xunit.Abstractions;

namespace FFAudioSharp.Tests;

public class SupervisedTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;

    private const string FILE_SOURCE_DIR = "";
    private const int PLAY_COUNT = 100;
    
    private IEnumerable<string> _filesToPlay;
    private int _fileIndex = 0;
    private bool _quitLoop = false;
    
    //PlayConfig. null to disable
    static double? _playDuration = 3.0;
    static double? _skipSeconds = null;
    static string? _loudnormSettings = null;
    static string? _crossfeedSetting = null;
    static string? _avFiltergraphOverride = null;

    public SupervisedTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _filesToPlay = Directory.EnumerateFiles(FILE_SOURCE_DIR, "*.*", SearchOption.AllDirectories);
    }
    
    public void Dispose()
    {
        // Clean up after each test
        FFAudio.Shutdown();
 
    }
    
    private void NotifyOfLog(string message, long request, FFAudio.AU_LOG_LEVEL level)
    {
        _testOutputHelper.WriteLine(message);
    }

    private void NotifyOfEndOfFile(bool isEofFromSkip, bool isFromError, int handle)
    {
        if (isEofFromSkip)
        {
            _testOutputHelper.WriteLine("EOF from skip");
            return;
        }
        
        if (isFromError) _testOutputHelper.WriteLine("EOF from error");

        if (_fileIndex + 1 >= PLAY_COUNT)
        {
            _testOutputHelper.WriteLine("End of play count");
            _quitLoop = true;
            return;      
        }

        if (_fileIndex + 1 >= _filesToPlay.Count())
        {
            _testOutputHelper.WriteLine("End of playlist");
            _quitLoop = true;
            return;       
        }
  
        var file = _filesToPlay.ElementAt(++_fileIndex);
        _testOutputHelper.WriteLine($"Playing next file: {file}");

        var config = new FFAudio.PlayAudioConfig()
        {
            PlayDuration = _playDuration ?? -1.0,
            SkipSeconds = _skipSeconds ?? -1.0,
            LoudnormSettings = _loudnormSettings, 
            CrossfeedSetting = _crossfeedSetting,
            AvFiltergraphOverride = _avFiltergraphOverride,
        };
            
        FFAudio.PlayAudio(file, config);
    }

    [Fact]
    public async void PlayPlaylist_ShouldPlayAllFiles()
    {
        var init = new FFAudio.InitializeConfig
        {
            AppName = "PlayPlaylist_ShouldPlayAllFiles",
            InitialLoopCount = 0,
            InitialVolume = 50,
            OnLog = NotifyOfLog,
            OnEof = NotifyOfEndOfFile,
            OnRestart = null,
            OnDurationUpdate = null,
            OnPrepareNext = null
            
        };

        Assert.Equal(0, FFAudio.Initialize(init));
        
        Assert.Equal(0, FFAudio.ConfigureAudioDevice(null));
        
        Assert.True(_filesToPlay.Any());
        
        var file = _filesToPlay.ElementAt(_fileIndex);
        _testOutputHelper.WriteLine($"Playing file: {file}");
        
        
        var config = new FFAudio.PlayAudioConfig()
        {
            PlayDuration = _playDuration ?? -1.0,
            SkipSeconds = _skipSeconds ?? -1.0,
            LoudnormSettings = _loudnormSettings, 
            CrossfeedSetting = _crossfeedSetting,
            AvFiltergraphOverride = _avFiltergraphOverride,
        };
        
        FFAudio.PlayAudio(file, config);

        while (!_quitLoop)
        {
            await Task.Delay(50);
        }
    }
}
