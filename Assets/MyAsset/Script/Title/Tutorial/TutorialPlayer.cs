using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Title.Tutorial
{
    internal sealed class TutorialPlayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _videoPlayerObj;
        [SerializeField] 
        private VideoPlayer _videoPlayer;
        [SerializeField]
        private Image _backToTitleButtonImage;
        [SerializeField]
        private TextMeshProUGUI _backToTitleButtonText;
        [SerializeField]
        private Button _backToTitleButton;

        internal void PlayTutorial()
        {
            if (_videoPlayer != null && _videoPlayerObj != null)
            {
                _videoPlayerObj.SetActive(true);
                _videoPlayer.enabled = true;
                _videoPlayer.frame = 0;
                _videoPlayer.Play();

            }
            ActivateButton(true);
        }

        private void StopTutorial()
        {
            if (_videoPlayer != null && _videoPlayer.isPlaying && _videoPlayerObj != null)
            {
                _videoPlayerObj.SetActive(false);
                _videoPlayer.Stop();
                _videoPlayer.enabled = false;
            }
            ActivateButton(false);
        }

        private void OnEnable() 
        {
            if ( _backToTitleButton == null) return;
            _backToTitleButton.onClick.RemoveAllListeners();
            _backToTitleButton.onClick.AddListener(StopTutorial);
        }

        private void ActivateButton(bool state)
        {
            if (_backToTitleButtonText == null || _backToTitleButtonImage == null || _backToTitleButton == null) return;

            _backToTitleButtonText.enabled 
                = _backToTitleButtonImage.enabled
                = _backToTitleButton.interactable 
                = state;
        }
    }

}