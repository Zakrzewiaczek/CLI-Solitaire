using NAudio.Wave;

namespace Solitaire
{
    public static class SoundsManager
    {
        public static class SoundFX
        {
            public enum SoundType
            {
                Operation,
                StartGame,
                GamePause,
                GameResume,
                CardMove,
                CardShuffle,
                CardPlacedInFoundation,
                Victory,
                PointsSound
            }

            /// <summary>
            /// Dictionary mapping sound effect types to their corresponding file names.
            /// </summary>
            private static readonly Dictionary<SoundType, string> _soundPaths = new()
            {
                { SoundType.Operation, "operation.wav" },
                { SoundType.StartGame, "start_game.wav" },
                { SoundType.GamePause, "pause.wav" },
                { SoundType.GameResume, "resume.wav" },
                { SoundType.CardMove, "card_move.wav" },
                { SoundType.CardShuffle, "card_shuffle.wav" },
                { SoundType.CardPlacedInFoundation, "card_placed_in_foundation.wav" },
                { SoundType.Victory, "victory.wav" },
                { SoundType.PointsSound, "points_sound.wav" }
            };

            /// <summary>
            /// Indicates whether sound effects are enabled.
            /// </summary>
            private static bool _isSoundFXEnabled = true;
            /// <summary>
            /// Gets or sets whether sound effects are enabled.
            /// </summary>
            public static bool IsSoundFXEnabled
            {
                get => _isSoundFXEnabled;
                set => _isSoundFXEnabled = value;
            }

            /// <summary>
            /// Plays a sound effect of the specified type if sound effects are enabled.
            /// </summary>
            /// <param name="sound">The type of sound effect to play.</param>
            public static void PlaySound(SoundType sound)
            {
                if (!_isSoundFXEnabled || !_soundPaths.TryGetValue(sound, out var filePath))
                    return;

                // Play the sound asynchronously and dispose resources after playback.
                var reader = new AudioFileReader($"sounds/{filePath}");
                var waveOut = new WaveOutEvent();
                waveOut.Init(reader);
                waveOut.Play();
                waveOut.PlaybackStopped += (s, e) =>
                {
                    waveOut.Dispose();
                    reader.Dispose();
                };
            }
        }

        public static class Music
        {
            /// <summary>
            /// Path to the background music file.
            /// </summary>
            private static readonly string _musicPath = "background_music.wav";
            /// <summary>
            /// Default music volume (0-100).
            /// </summary>
            private static readonly int _volume = 75;

            /// <summary>
            /// The current WaveOutEvent instance for music playback.
            /// </summary>
            private static WaveOutEvent? _waveOut;
            /// <summary>
            /// The current AudioFileReader instance for music playback.
            /// </summary>
            private static AudioFileReader? _audioFileReader;

            /// <summary>
            /// Indicates whether music is enabled.
            /// </summary>
            private static bool _isMusicEnabled = true;
            /// <summary>
            /// Gets or sets whether music is enabled.
            /// </summary>
            public static bool IsMusicEnabled
            {
                get => _isMusicEnabled;
                set
                {
                    if (_isMusicEnabled == value)
                        return;
                    _isMusicEnabled = value;
                    if (_isMusicEnabled)
                    {
                        if (_waveOut == null)
                            PlayMusic();
                        else
                            _waveOut.Play();
                    }
                    else
                    {
                        _waveOut?.Pause();
                    }
                }
            }

            /// <summary>
            /// Plays or pauses the background music depending on the enabled state.
            /// </summary>
            public static void PlayMusic()
            {
                if (_waveOut != null)
                    return;

                _audioFileReader = new AudioFileReader($"music/{_musicPath}")
                {
                    Position = 0,
                    Volume = _volume / 100f
                };
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioFileReader);
                _waveOut.PlaybackStopped += OnPlaybackStopped;
                if (_isMusicEnabled)
                    _waveOut.Play();
            }

            /// <summary>
            /// Handles the event when music playback stops, restarting if music is enabled.
            /// </summary>
            /// <param name="sender">The sender object.</param>
            /// <param name="e">The event arguments.</param>
            private static void OnPlaybackStopped(object? sender, StoppedEventArgs e)
            {
                if (_audioFileReader != null && _waveOut != null && _isMusicEnabled)
                {
                    _audioFileReader.Position = 0;
                    _waveOut.Play();
                }
            }
        }
    }
}