using UnityEngine;

namespace Title.Button
{
    internal sealed class ButtonManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite clickSprite;

        private bool isActive = true;

        private void OnEnable()
        {
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;
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

            // �X�v���C�g���z�o�[�ɂ���
        }

        // �}�E�X�̃|�C���^�[�����ꂽ
        public void OnExit()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!normalSprite) return;

            // �X�v���C�g���m�[�}���ɂ���
        }

        // �}�E�X�̃|�C���^�[���I�u�W�F�N�g�̏�ŃN���b�N���ꂽ
        public void OnClick()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!clickSprite) return;

            // �X�v���C�g���N���b�N�ɂ���

            isActive = false;

            // �N���b�N���̏���
        }
    }
}
