using UnityEngine;

namespace Main.Handler
{
    internal sealed class TimeCountDown : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI text;

        private float _t;

        // プロパティ
        internal float T { 
            get { return _t; }
            set { _t = Mathf.Clamp(value, byte.MinValue, byte.MaxValue); }
        }

        private void Start()
        {
            _t = SO.SO_Handler.Entity.InitTimeLimt;
        }

        private void Update()
        {
            if (GameManager.Instance.State == GameState.Stay)
            {
                if (_t > 0) _t -= Time.deltaTime;
                _t = Mathf.Max(0, _t);
                if (_t == 0) GameManager.Instance.State = GameState.Over;
            }

            if (text) text.text = $"{(int)_t:D2}";
        }
    }
}