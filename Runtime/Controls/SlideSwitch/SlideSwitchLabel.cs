using TMPro;
using UnityEngine;

namespace OxenteGames.UI
{
    /// <summary>
    /// Optional TextMeshPro presenter for a <see cref="SlideSwitch"/>.
    /// </summary>
    [AddComponentMenu("UI (Canvas)/Oxente UI/Slide Switch Label", 35)]
    [RequireComponent(typeof(SlideSwitch))]
    public sealed class SlideSwitchLabel : MonoBehaviour
    {
        [SerializeField] private SlideSwitch slideSwitch;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string onText = "On";
        [SerializeField] private string offText = "Off";

        private void Awake()
        {
            EnsureSwitch();
        }

        private void OnEnable()
        {
            EnsureSwitch();
            if (!slideSwitch) return;

            slideSwitch.OnValueChanged += HandleValueChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (slideSwitch)
                slideSwitch.OnValueChanged -= HandleValueChanged;
        }

        public void Refresh()
        {
            if (slideSwitch)
                HandleValueChanged(slideSwitch.IsOn);
        }

        private void HandleValueChanged(bool value)
        {
            if (label)
                label.text = value ? onText : offText;
        }

        private void EnsureSwitch()
        {
            if (!slideSwitch)
                slideSwitch = GetComponent<SlideSwitch>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureSwitch();
            if (!Application.isPlaying)
                Refresh();
        }
#endif
    }
}
