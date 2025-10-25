using FFAudioSharp;

namespace SimpleExample;

class Program
{
    /// <summary>
    /// Set this to the file you want to play.
    /// </summary>
    private const string FILE_SOURCE = "";
    
    private static bool _quitLoop = false;
    
    static async Task Main(string[] args)
    {
        // Make sure we have a file to play
        if (FILE_SOURCE == "")
        {
            Console.WriteLine("No file specified");
            return;
        }
        
        // Initialize FFAudio with struct. Take note of the callbacks `OnLog` `NotifyOfEndOfFile`
        var init = new FFAudio.InitializeConfig
        {
            AppName = "SimpleExample",
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
        
        // Play the file.
        Console.WriteLine($"Playing file: {FILE_SOURCE}");
        
        FFAudio.PlayAudio(FILE_SOURCE, null);
        
        // Wait for file to finish
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
        Console.WriteLine("File ended");
        _quitLoop = true;
    }
}
