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
    }
}
