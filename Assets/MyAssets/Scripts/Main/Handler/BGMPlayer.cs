using General;
using SO;

namespace Main.Handler
{
    internal sealed class BGMPlayer : AFadeableBgmPlayer
    {
        private bool hasPlayed = false;

        internal void Play()
        {
            if (hasPlayed) return;
            if (_audioSource == null) return;
            hasPlayed = true;
            _audioSource.Raise(SO_Sound.Entity.MainBGM, SoundType.BGM);
        }
    }
}