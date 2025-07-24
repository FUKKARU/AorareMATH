using UnityEngine;
using General;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_SaveDataDefault", fileName = "SO_SaveDataDefault")]
    internal sealed class SO_SaveDataDefault : AResourceLoadableScriptableObject<SO_SaveDataDefault>
    {
        [SerializeField, Range(1, 1000), Header("正解数ランキングの要素数")] private int correctAmountRankingLength;
        internal int CorrectAmountRankingLength => correctAmountRankingLength;

        [SerializeField, Header("演出の高速化")] private bool doFastenDirections;
        internal bool DoFastenDirections => doFastenDirections;

        [SerializeField, Range(-20.0f, 20.0f), Header("BGMボリューム (dB)"), Tooltip("スライダーの値が整数なので注意")] private float bgmVolume;
        internal float BgmVolume => bgmVolume;

        [SerializeField, Range(-20.0f, 20.0f), Header("SEボリューム (dB)"), Tooltip("スライダーの値が整数なので注意")] private float seVolume;
        internal float SeVolume => seVolume;
    }
}