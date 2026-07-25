using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OxenteGames.UI
{
    [Serializable]
    public sealed class RangeChangedEvent : UnityEvent<int, int>
    {
    }

    /// <summary>
    /// Integer range selector with two handles. Values are always normalized so
    /// <c>MinValue &lt;= LowValue &lt;= HighValue &lt;= MaxValue</c>.
    /// </summary>
    [AddComponentMenu("UI (Canvas)/Oxente UI/Range Slider", 32)]
    [RequireComponent(typeof(RectTransform))]
    public class RangeSlider : Selectable, IDragHandler, IInitializePotentialDragHandler
    {
        [Header("Range")]
        [SerializeField] private int minValue;
        [SerializeField] private int maxValue = 100;
        [Min(0), SerializeField] private int minSpan;
        [Tooltip("Zero or less disables the maximum span limit.")]
        [SerializeField] private int maxSpan;
        [Min(1), SerializeField] private int digits = 1;

        [Header("References")]
        [SerializeField] private RectTransform slideArea;
        [SerializeField] private RectTransform lowHandle;
        [SerializeField] private RectTransform highHandle;
        [SerializeField] private RectTransform fill;

        [Header("Interaction")]
        [Tooltip("Dragging inside the selected range moves both handles together.")]
        [SerializeField] private bool dragBothFromFill = true;

        [Header("Value")]
        [SerializeField] private int lowValue = 25;
        [SerializeField] private int highValue = 75;

        [Header("Events")]
        [FormerlySerializedAs("onValueChanged")]
        [SerializeField] private RangeChangedEvent valueChanged = new();

        public event Action<int, int> ValueChanged;

        public int MinValue => minValue;
        public int MaxValue => maxValue;
        public int MinSpan => minSpan;
        public int MaxSpan => maxSpan;
        public int LowValue => lowValue;
        public int HighValue => highValue;
        public int Span => highValue - lowValue;
        public string LowFormatted => Format(lowValue);
        public string HighFormatted => Format(highValue);
        public RangeChangedEvent OnValueChanged => valueChanged;

        public int Digits
        {
            get => digits;
            set
            {
                var normalized = Mathf.Max(1, value);
                if (digits == normalized) return;
                digits = normalized;
                UpdateVisuals();
            }
        }

        private enum ActiveHandle
        {
            None,
            Low,
            High,
            Both
        }

        private ActiveHandle activeHandle;
        private int bothAnchorValue;
        private int bothLowAtStart;

        protected override void Awake()
        {
            base.Awake();
            EnsureTargetGraphic();
            NormalizeSerializedValues();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            NormalizeSerializedValues();
            UpdateVisuals();
        }

        public void Configure(int min, int max, int minimumSpan, int maximumSpan, bool recenter = true)
        {
            minValue = min;
            maxValue = Mathf.Max(min, max);
            minSpan = Mathf.Max(0, minimumSpan);
            maxSpan = maximumSpan;

            if (recenter)
                Center();
            else
                SetValuesWithoutNotify(lowValue, highValue);
        }

        public void SetBounds(int min, int max)
        {
            minValue = min;
            maxValue = Mathf.Max(min, max);
            SetValuesWithoutNotify(lowValue, highValue);
        }

        public void SetSpanLimits(int minimum, int maximum)
        {
            minSpan = Mathf.Max(0, minimum);
            maxSpan = maximum;
            SetValuesWithoutNotify(lowValue, highValue);
        }

        public void SetValues(int low, int high)
        {
            SetValuesInternal(low, high, true);
        }

        public void SetValuesWithoutNotify(int low, int high)
        {
            SetValuesInternal(low, high, false);
        }

        public void Center()
        {
            var span = ClampSpan(minSpan);
            var available = maxValue - minValue - span;
            var low = minValue + Mathf.RoundToInt(available * 0.5f);
            SetValues(low, low + span);
        }

        public void CenterWithoutNotify()
        {
            var span = ClampSpan(minSpan);
            var available = maxValue - minValue - span;
            var low = minValue + Mathf.RoundToInt(available * 0.5f);
            SetValuesWithoutNotify(low, low + span);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (!MayDrag(eventData)) return;

            var pointerValue = PointerToValue(eventData);
            var distanceToLow = Mathf.Abs(pointerValue - lowValue);
            var distanceToHigh = Mathf.Abs(pointerValue - highValue);
            var lowMargin = HandleMarginValue(lowHandle);
            var highMargin = HandleMarginValue(highHandle);

            if (distanceToLow <= lowMargin && distanceToLow <= distanceToHigh)
            {
                activeHandle = ActiveHandle.Low;
            }
            else if (distanceToHigh <= highMargin)
            {
                activeHandle = ActiveHandle.High;
            }
            else if (dragBothFromFill && pointerValue > lowValue && pointerValue < highValue)
            {
                activeHandle = ActiveHandle.Both;
                bothAnchorValue = pointerValue;
                bothLowAtStart = lowValue;
                return;
            }
            else
            {
                activeHandle = distanceToLow <= distanceToHigh
                    ? ActiveHandle.Low
                    : ActiveHandle.High;
            }

            MoveActiveHandle(pointerValue);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            activeHandle = ActiveHandle.None;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData)) return;

            var pointerValue = PointerToValue(eventData);
            if (activeHandle == ActiveHandle.Both)
                MoveBothHandles(pointerValue);
            else
                MoveActiveHandle(pointerValue);
        }

        private void SetValuesInternal(int low, int high, bool notify)
        {
            NormalizeValues(ref low, ref high);
            var changed = lowValue != low || highValue != high;

            lowValue = low;
            highValue = high;
            UpdateVisuals();

            if (!notify || !changed) return;
            valueChanged?.Invoke(lowValue, highValue);
            ValueChanged?.Invoke(lowValue, highValue);
        }

        private void NormalizeSerializedValues()
        {
            maxValue = Mathf.Max(minValue, maxValue);
            minSpan = Mathf.Max(0, minSpan);
            digits = Mathf.Max(1, digits);

            var low = lowValue;
            var high = highValue;
            NormalizeValues(ref low, ref high);
            lowValue = low;
            highValue = high;
        }

        private void NormalizeValues(ref int low, ref int high)
        {
            low = Mathf.Clamp(low, minValue, maxValue);
            high = Mathf.Clamp(high, minValue, maxValue);

            if (high < low)
                (low, high) = (high, low);

            var span = ClampSpan(high - low);
            high = low + span;

            if (high > maxValue)
            {
                high = maxValue;
                low = high - span;
            }

            if (low < minValue)
            {
                low = minValue;
                high = low + span;
            }
        }

        private int ClampSpan(int span)
        {
            var total = Mathf.Max(0, maxValue - minValue);
            var minimum = Mathf.Clamp(minSpan, 0, total);
            var maximum = maxSpan > 0 ? Mathf.Clamp(maxSpan, minimum, total) : total;
            return Mathf.Clamp(span, minimum, maximum);
        }

        private void MoveBothHandles(int pointerValue)
        {
            var span = highValue - lowValue;
            var delta = pointerValue - bothAnchorValue;
            var newLow = Mathf.Clamp(bothLowAtStart + delta, minValue, maxValue - span);
            SetValuesInternal(newLow, newLow + span, true);
        }

        private void MoveActiveHandle(int pointerValue)
        {
            var minimumSpan = ClampSpan(minSpan);

            if (activeHandle == ActiveHandle.Low)
            {
                var minimumLow = maxSpan > 0
                    ? Mathf.Max(minValue, highValue - maxSpan)
                    : minValue;
                var maximumLow = highValue - minimumSpan;
                SetValuesInternal(Mathf.Clamp(pointerValue, minimumLow, maximumLow), highValue, true);
            }
            else if (activeHandle == ActiveHandle.High)
            {
                var minimumHigh = lowValue + minimumSpan;
                var maximumHigh = maxSpan > 0
                    ? Mathf.Min(maxValue, lowValue + maxSpan)
                    : maxValue;
                SetValuesInternal(lowValue, Mathf.Clamp(pointerValue, minimumHigh, maximumHigh), true);
            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive()
                   && IsInteractable()
                   && eventData.button == PointerEventData.InputButton.Left
                   && GetSlideArea();
        }

        private int PointerToValue(PointerEventData eventData)
        {
            var area = GetSlideArea();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                area,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint);

            var rect = area.rect;
            var normalized = Mathf.Clamp01(
                (localPoint.x - rect.xMin) / Mathf.Max(0.0001f, rect.width));

            return Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, normalized));
        }

        private float HandleMarginValue(RectTransform handle)
        {
            var area = GetSlideArea();
            if (!handle || !area) return 0f;

            var width = Mathf.Max(0.0001f, area.rect.width);
            var range = Mathf.Max(1, maxValue - minValue);
            return handle.rect.width * 0.5f / width * range;
        }

        private RectTransform GetSlideArea()
        {
            return slideArea ? slideArea : transform as RectTransform;
        }

        private void UpdateVisuals()
        {
            var range = Mathf.Max(1, maxValue - minValue);
            var lowNormalized = (float)(lowValue - minValue) / range;
            var highNormalized = (float)(highValue - minValue) / range;

            PlaceHandle(lowHandle, lowNormalized);
            PlaceHandle(highHandle, highNormalized);

            if (!fill) return;

            fill.anchorMin = new Vector2(lowNormalized, fill.anchorMin.y);
            fill.anchorMax = new Vector2(highNormalized, fill.anchorMax.y);
            fill.offsetMin = new Vector2(0f, fill.offsetMin.y);
            fill.offsetMax = new Vector2(0f, fill.offsetMax.y);
        }

        private static void PlaceHandle(RectTransform handle, float normalized)
        {
            if (!handle) return;

            handle.anchorMin = new Vector2(normalized, handle.anchorMin.y);
            handle.anchorMax = new Vector2(normalized, handle.anchorMax.y);
            handle.anchoredPosition = new Vector2(0f, handle.anchoredPosition.y);
        }

        private string Format(int value)
        {
            return value.ToString().PadLeft(Mathf.Max(1, digits), '0');
        }

        private void EnsureTargetGraphic()
        {
            if (!targetGraphic)
                targetGraphic = GetComponent<Graphic>();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            EnsureTargetGraphic();
            NormalizeSerializedValues();

            if (!Application.isPlaying)
                UpdateVisuals();
        }

        [ContextMenu("Center Range (Preview)")]
        private void CenterPreview()
        {
            CenterWithoutNotify();
        }
#endif
    }
}
