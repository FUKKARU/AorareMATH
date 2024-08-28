using UnityEngine;
using TMPro;

namespace Main.Handler
{
    internal sealed class TimeCountDown : MonoBehaviour
    {
        private float elapsedTime;

        // TextMeshProUGUI �̎Q�Ƃ��C���X�y�N�^�Őݒ�
        [SerializeField] private TextMeshProUGUI timerText;

        // �������Ԃ�ݒ�i��: 60�b�j
        [SerializeField] private float startTime = 60f;

        private void Start()
        {
            elapsedTime = startTime;
        }

        private void Update()
        {
            // ���Ԃ����炷
            elapsedTime -= Time.deltaTime;

            // 0�ȉ��ɂȂ�Ȃ��悤�ɐ���
            if (elapsedTime < 0)
            {
                elapsedTime = 0;
            }

            // �e�L�X�g�Ɏc�莞�Ԃ𐮐��l�ŕ\���i��: "60", "37", "08"�j
            if (timerText != null)
            {
                timerText.text = $"{(int)elapsedTime:D2}";
            }
        }
    }
}