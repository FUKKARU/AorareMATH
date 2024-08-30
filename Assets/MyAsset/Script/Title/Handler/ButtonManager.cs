using Cysharp.Threading.Tasks;
using General.Extension;
using SO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Title.Handler
{
    internal sealed class ButtonManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite clickSprite;

        private bool isActive = true;

        private CancellationToken ct;

        private void OnEnable()
        {
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;

            ct = this.GetCancellationTokenOnDestroy();
        }

        private void OnDisable()
        {
            sr = null;
            normalSprite = null;
            hoverSprite = null;
            clickSprite = null;
        }

        // �}�E�X�̃|�C���^�[�����������
        public void OnEnter()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!hoverSprite) return;

            sr.sprite = hoverSprite;
        }

        // �}�E�X�̃|�C���^�[�����ꂽ
        public void OnExit()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;
        }

        // �}�E�X�̃|�C���^�[���I�u�W�F�N�g�̏�ŃN���b�N���ꂽ
        public void OnClick()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!clickSprite) return;

            sr.sprite = clickSprite;

            isActive = false;

            SO_Handler.Entity.WaitDurOnButtonPlaced.SecondsWaitAndDo
                (() => SceneManager.LoadScene(SO_SceneName.Entity.Main), ct).Forget();
        }
    }
}