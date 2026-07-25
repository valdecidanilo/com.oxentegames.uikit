using TMPro;
using UnityEngine;

namespace OxenteGames.UI
{
    /// <summary>
    /// Optional TextMeshPro presenter for a <see cref="RangeSlider"/>.
    /// </summary>
    [AddComponentMenu("UI (Canvas)/Oxente UI/Range Slider Labels", 33)]
    [RequireComponent(typeof(RangeSlider))]
    public sealed class RangeSliderLabels : MonoBehaviour
    {
        [SerializeField] private RangeSlider rangeSlider;
        [SerializeField] private TMP_Text lowLabel;
        [SerializeField] private TMP_Text highLabel;

        private void Awake()
        {
            EnsureSlider();
        }

        private void OnEnable()
        {
            EnsureSlider();
            if (!rangeSlider) return;

            rangeSlider.ValueChanged += HandleValueChanged;
            Refresh();
        }

        private void OnDisable()
        {
            if (rangeSlider)
                rangeSlider.ValueChanged -= HandleValueChanged;
        }

        public void Refresh()
        {
            if (!rangeSlider) return;
            HandleValueChanged(rangeSlider.LowValue, rangeSlider.HighValue);
        }

        private void HandleValueChanged(int low, int high)
        {
            if (lowLabel)
                lowLabel.text = rangeSlider.LowFormatted;

            if (highLabel)
                highLabel.text = rangeSlider.HighFormatted;
        }

        private void EnsureSlider()
        {
            if (!rangeSlider)
                rangeSlider = GetComponent<RangeSlider>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureSlider();
            if (!Application.isPlaying)
                Refresh();
        }
#endif
    }
}
