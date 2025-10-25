using FFAudioSharp;

namespace PlaylistExample;

class Program
{
    /// <summary>
    /// Set this to the directory containing the files you want to play.
    /// </summary>
    private const string FILE_SOURCE_DIR = "";
    /// <summary>
    /// How many files to play before quitting.
    /// </summary>
    private const int PLAY_COUNT = 100;
    
    
    private static List<string> _filesToPlay;
    private static int _fileIndex = 0;
    private static bool _quitLoop = false;
    
    static async Task Main(string[] args)
    {
        // Get all files in the directory
        _filesToPlay = Directory.EnumerateFiles(FILE_SOURCE_DIR, "*.*", SearchOption.AllDirectories).ToList();
        
        // Make sure we have files to play
        if (_filesToPlay.Count == 0) return;
        
        // Initialize FFAudio with struct. Take note of the callbacks `OnLog` `NotifyOfEndOfFile`, especially the latter
        var init = new FFAudio.InitializeConfig
        {
            AppName = "PlaylistExample",
            InitialLoopCount = 0,
            InitialVolume = 50,
            OnLog = NotifyOfLog,
            OnEof = NotifyOfEndOfFile,
            OnRestart = null,
            OnDurationUpdate = null,
            OnPrepareNext = null
            
        };

        // Initialize FFAudio with struct.
        if (FFAudio.Initialize(init) < 0) return;
        
        // Configure an audio device. This is *required* before calling `PlayAudio`
        if (FFAudio.ConfigureAudioDevice(null) < 0) return;
        
        // Play the first file. The rest of the files are played in the callback `NotifyOfEndOfFile`
        var file = _filesToPlay.ElementAt(_fileIndex);
        Console.WriteLine($"Playing file: {file}");
        
        FFAudio.PlayAudio(file, null);
        
        // Wait for playlist to finish
        while (!_quitLoop)
        {
            await Task.Delay(50);
        }
        
        // Finally, cleanup resources
        FFAudio.Shutdown();
    }
    
    /// <summary>
    /// Gets log messages from FFAudio. Includes error messages.
    /// </summary>
    /// <param name="message">Message description</param>
    /// <param name="request">Request number</param>
    /// <param name="level">Log level</param>
    private static void NotifyOfLog(string message, long request, FFAudio.AU_LOG_LEVEL level)
    {
        Console.WriteLine(message);
    }

    /// <summary>
    /// Is called when FFAudio has reached the end of a file.
    /// </summary>
    /// <param name="isEofFromSkip">Is true when a file ends because another file was played</param>
    /// <param name="isFromError">Is true when a file ends because of an error</param>
    /// <param name="handle">The handle of the file that ended. Not supper useful as of now</param>
    private static void NotifyOfEndOfFile(bool isEofFromSkip, bool isFromError, int handle)
    {
        if (isEofFromSkip)
        {
            //It is important to return here. In this example it is not used,
            //but it is necessary for this callback to not interfere by
            //playing the next file when a file is skipped. For example:
            //When OnEof is used to play the next file, like it is here,
            //and PlayAudio() is called when a file is already playing,
            //it will skip the current file and play the next one but will
            //also produce an EOF event with isEofFromSkip = true.
            //Therefore, we skip that event here.
            Console.WriteLine("EOF from skip");
            return;
        }
        
        // EOF from error, play next file
        if (isFromError) Console.WriteLine("EOF from error");

        // Leave if we have reached the end of the desired play count
        if (_fileIndex + 1 >= PLAY_COUNT)
        {
            Console.WriteLine("End of play count");
            _quitLoop = true;
            return;      
        }

        // Leave if we have reached the end of the playlist
        if (_fileIndex + 1 >= _filesToPlay.Count())
        {
            Console.WriteLine("End of playlist");
            _quitLoop = true;
            return;       
        }
  
        // Play the next file in the playlist
        var file = _filesToPlay.ElementAt(++_fileIndex);
        Console.WriteLine($"Playing next file: {file}");
        
        FFAudio.PlayAudio(file, null);
    }
}
