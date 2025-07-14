using General;
using SO;

namespace Title.Handler
{
    internal sealed class BGMPlayer : AFadeableBgmPlayer
    {
        private void OnEnable()
            => _audioSource.Raise(SO_Sound.Entity.TitleBGM, SoundType.BGM, time: SO_Handler.Entity.DoFastenDirections ? 1.0f : 0.0f);
    }
}