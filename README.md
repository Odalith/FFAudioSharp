<img width="1815" height="150" alt="banner" src="https://github.com/user-attachments/assets/46466334-645f-4585-a24c-55f315cc892c" />

The C# bindings for [FFaudio](https://github.com/Odalith/FFaudio)

### FFaudio is a high-level audio player library using FFmpeg & SDL2. 
It is based on a heavily modified version of FFplay, so all credit for its general design goes to the FFmpeg team and contributors.

## Features
- Cross-platform. Can potentially support Windows, Linux, macOS, Android, and iOS. (Linux is the only one currently, more soon to come)
- Thanks to FFmpeg it can decode and play basically any file with an audio component:
  - Tested to work with mp3, flac, wav, aiff, m4a, wma, oga, ogg, aac, and dsf (dsd64)
- Plentiful list of options and filters for customizing playback:
  - Volume 0-100
  - Looping infinite, 0, or a specific number of times
  - Mute
  - Pause/Resume
  - Seek
  - Seek percentage
  - 10 band Equalizer
  - Crossfeed
  - EBU R128 audio normalization
  - More if you know how to set up FFmpeg filters (Send PR!)
- BEEFY callbacks with flags for easy integration into your application:
  - End of file callback
  - Logging callback
  - Stream restarted callback
  - Duration updated callback (Not implemeted yet)
  - Prepare next file callabck (Not implemeted yet)
- Simple API for integrating audio playback into you .NET application
- Performant and memory efficient thanks again to FFmpeg
- Supports playing audio through a custom audio device and runtime reconfiguration
- Licensed under the LGPLv2.1 and free to use

## Who It's For
Want audio playback and don't need to mix audio? Yes? Fabulous.

## Planned Features
- Fully support playback of rtp, rtsp, udp, and sdp (non-realtime) audio streams
- Gapless playback for non-realtime streams via 'soon to be done callback'
- Realtime updates to Equalizer
- Crossfade with custom crossfade time (Note that the currently supported Crossfeed is different from Crossfade)

## Possible Features
- Audio mixing of multiple streams
- Audio file conversion
- OS integration. Linux MPRIS support, for example.
- Custom channel layouts
- Support for audio formats with more than two channels
- Assuming compatible hardware, support playing DSD without conversion to PCM
- Generation of EBU R128 audio normalization data (Currently, you have to do this yourself. FFaudio only does the adjustment part)
- Multiple simultaneous audio devices


Note; this project is not affiliated with FFmpeg, FFplay, or their Authors.
