using General;
using SO;

namespace Title.Handler
{
    internal sealed class BGMPlayer : AFadeableBgmPlayer
    {
        private void OnEnable() => _audioSource.Raise(SO_Sound.Entity.TitleBGM, SoundType.BGM);
    }
}