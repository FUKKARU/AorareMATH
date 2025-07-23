using UnityEngine;
using General;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Handler", fileName = "SO_Handler")]
    public class SO_Handler : AResourceLoadableScriptableObject<SO_Handler>
    {
        [SerializeField, Header("音量の最小値/最大値(db)")] private Vector2 _soundVolumeRange;
        internal float MinVolume => _soundVolumeRange.x;
        internal float MaxVolume => _soundVolumeRange.y;

        [SerializeField, Header("問題数（最大まで行くと、強制クリア）")] private int _questionAmount;
        internal int QuestionAmount => _questionAmount;

        [SerializeField, Header("スキップ可能数")] private int _skipAmount;
        internal int SkipAmount => _skipAmount;

        [SerializeField, Header("ゲームの初期制限時間(秒)")] private float _initTimeLimit;
        internal float InitTimeLimt => _initTimeLimit;

        [SerializeField, Header("ピッタリ時の時間増加量")] private float _timeIncreaseAmount;
        internal float TimeIncreaseAmount => _timeIncreaseAmount;

        internal static readonly float DiffLimit = 1e-8f;
    }
}