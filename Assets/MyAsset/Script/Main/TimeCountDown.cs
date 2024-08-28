using UnityEngine;
using TMPro;

namespace Main.Handler
{
    internal sealed class TimeCountDown : MonoBehaviour
    {
        private float elapsedTime;

        // TextMeshProUGUI の参照をインスペクタで設定
        [SerializeField] private TextMeshProUGUI timerText;

        // 初期時間を設定（例: 60秒）
        [SerializeField] private float startTime = 60f;

        private void Start()
        {
            elapsedTime = startTime;
        }

        private void Update()
        {
            // 時間を減らす
            elapsedTime -= Time.deltaTime;

            // 0以下にならないように制限
            if (elapsedTime < 0)
            {
                elapsedTime = 0;
            }

            // テキストに残り時間を整数値で表示（例: "60", "37", "08"）
            if (timerText != null)
            {
                timerText.text = $"{(int)elapsedTime:D2}";
            }
        }
    }
}