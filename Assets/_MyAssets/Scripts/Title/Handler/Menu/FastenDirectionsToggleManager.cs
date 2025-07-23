using UnityEngine;
using UnityEngine.UI;
using General;
using SO;

namespace Title.Handler.Menu
{
    internal sealed class FastenDirectionsToggleManager : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;

        private void Start()
        {
            toggle.isOn = SaveDataHolder.Data.DoFastenDirections;
            toggle.onValueChanged.AddListener(isOn =>
            {
                SaveDataHolder.Data.DoFastenDirections = isOn;
                AudioSourceManager.Instance.Play(SO_Sound.Entity.ClickSE, SoundType.SE, pitch: Pitch.Hover);
            });
        }
    }
}