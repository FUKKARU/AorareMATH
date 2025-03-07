using UnityEngine;
using UnityEngine.Audio;
using General;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Sound", fileName = "SO_Sound")]
    internal sealed class SO_Sound : AResourceLoadableScriptableObject<SO_Sound>
    {
        [SerializeField] private AudioMixer _audioMixer;
        internal AudioMixer AudioMixer => _audioMixer;
        [SerializeField] private AudioMixerGroup _AMGroupBGM;
        internal AudioMixerGroup AMGroupBGM => _AMGroupBGM;
        [SerializeField] private AudioMixerGroup _AMGroupSE;
        internal AudioMixerGroup AMGroupSE => _AMGroupSE;
        [SerializeField] private AudioMixerGroup _AMGroupMaster;
        internal AudioMixerGroup AMGroupMaster => _AMGroupMaster;
        [Space(25)]
        [Header("BGM")]
        [SerializeField] private AudioClip _titleBGM;
        internal AudioClip TitleBGM => _titleBGM;
        [SerializeField] private AudioClip _mainBGM;
        internal AudioClip MainBGM => _mainBGM;
        [Space(25)]
        [Header("SE")]
        [SerializeField] private AudioClip _countDownSE;
        internal AudioClip CountDownSE => _countDownSE;
        [SerializeField] private AudioClip _symbolSE;
        internal AudioClip SymbolSE => _symbolSE;
        [SerializeField] private AudioClip _attackSE;
        internal AudioClip AttackSE => _attackSE;
        [SerializeField] private AudioClip _attackFailedSE;
        internal AudioClip AttackFailedSE => _attackFailedSE;
        [SerializeField] private AudioClip _justAttackedSE;
        internal AudioClip JustAttackedSE => _justAttackedSE;
        [SerializeField] private AudioClip _enemyCarSE;
        internal AudioClip EnemyCarSE => _enemyCarSE;
        [SerializeField] private AudioClip _resultSE;
        internal AudioClip ResultSE => _resultSE;
    }
}