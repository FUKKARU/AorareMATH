using UnityEngine;
using UnityEngine.Audio;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Sound", fileName = "SO_Sound")]
    public class SO_Sound : ScriptableObject
    {
        #region
        public const string PATH = "SO_Sound";

        private static SO_Sound _entity = null;
        public static SO_Sound Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Sound>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField] private AudioMixer _audioMixer;
        public AudioMixer AudioMixer => _audioMixer;
        [SerializeField] private AudioMixerGroup _AMGroupBGM;
        public AudioMixerGroup AMGroupBGM => _AMGroupBGM;
        [SerializeField] private AudioMixerGroup _AMGroupSE;
        public AudioMixerGroup AMGroupSE => _AMGroupSE;
        [SerializeField] private AudioMixerGroup _AMGroupMaster;
        public AudioMixerGroup AMGroupMaster => _AMGroupMaster;
        [Space(25)]
        [Header("BGM")]
        [SerializeField] private AudioClip _titleBGM;
        public AudioClip TitleBGM => _titleBGM;
        [SerializeField] private AudioClip _mainBGM;
        public AudioClip MainBGM => _mainBGM;
        [Space(25)]
        [Header("SE")]
        [SerializeField] private AudioClip _attackSE;
        public AudioClip AttackSE => _attackSE;
        [SerializeField] private AudioClip _enemyCarSE;
        public AudioClip EnemyCarSE => _enemyCarSE;
        [SerializeField] private AudioClip _enemyMotorbikeSE1;
        public AudioClip EnemyMotorbikeSE1 => _enemyMotorbikeSE1;
        [SerializeField] private AudioClip _enemyMotorbikeSE2;
        public AudioClip EnemyMotorbikeSE2 => _enemyMotorbikeSE2;
        [SerializeField] private AudioClip _resultSE;
        public AudioClip ResultSE => _resultSE;
    }
}
