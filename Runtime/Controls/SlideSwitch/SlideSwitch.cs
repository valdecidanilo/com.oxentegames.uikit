using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OxenteGames.UI
{
    [Serializable]
    public sealed class SwitchBoolEvent : UnityEvent<bool>
    {
    }

    [Serializable]
    public sealed class SwitchFloatEvent : UnityEvent<float>
    {
    }

    /// <summary>
    /// Toggle-style switch with a normalized handle animation and no external
    /// tweening dependency.
    /// </summary>
    [AddComponentMenu("UI (Canvas)/Oxente UI/Slide Switch", 34)]
    [RequireComponent(typeof(RectTransform))]
    public class SlideSwitch : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform handle;
        [SerializeField] private RectTransform fill;

        [Header("Animation")]
        [Min(0f), SerializeField] private float transitionDuration = 0.15f;
        [SerializeField] private AnimationCurve transitionCurve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private bool useUnscaledTime = true;

        [Header("Value")]
        [SerializeField] private bool isOn;

        [Header("Events")]
        [FormerlySerializedAs("onSwitch")]
        [SerializeField] private SwitchBoolEvent valueChanged = new();
        [FormerlySerializedAs("onAnimate")]
        [SerializeField] private SwitchFloatEvent transitionProgressChanged = new();

        public event Action<bool> OnValueChanged;
        public event Action<float> OnAnimated;

        public SwitchBoolEvent OnValueChangedUnityEvent => valueChanged;
        public SwitchFloatEvent OnTransitionProgressUnityEvent => transitionProgressChanged;

        public bool IsOn
        {
            get => isOn;
            set => SetOn(value, true, true);
        }

        private float visualPosition;
        private Coroutine transitionCoroutine;

        protected override void Awake()
        {
            base.Awake();
            EnsureTargetGraphic();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnsureTargetGraphic();
            ApplyValue(false);
        }

        protected override void OnDisable()
        {
            StopTransition();
            base.OnDisable();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            Toggle();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Toggle();
        }

        public void Toggle()
        {
            if (!IsActive() || !IsInteractable()) return;
            SetOn(!isOn, true, true);
        }

        /// <summary>
        /// Sets the switch value.
        /// </summary>
        /// <param name="value">New on/off value.</param>
        /// <param name="animate">Whether to animate the handle and fill.</param>
        /// <param name="notify">Whether to invoke Unity and C# events.</param>
        public void SetOn(bool value, bool animate = true, bool notify = false)
        {
            var changed = isOn != value;
            isOn = value;
            ApplyValue(animate);

            if (!notify || !changed) return;
            valueChanged?.Invoke(isOn);
            OnValueChanged?.Invoke(isOn);
        }

        public void SetIsOnWithoutNotify(bool value, bool animate = false)
        {
            SetOn(value, animate, false);
        }

        private void ApplyValue(bool animate)
        {
            StopTransition();
            var target = isOn ? 1f : 0f;

            if (!animate
                || !Application.isPlaying
                || !isActiveAndEnabled
                || transitionDuration <= 0f)
            {
                SetVisualPosition(target);
                return;
            }

            transitionCoroutine = StartCoroutine(AnimateTo(target));
        }

        private IEnumerator AnimateTo(float target)
        {
            var start = visualPosition;
            var elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                var normalized = Mathf.Clamp01(elapsed / transitionDuration);
                var curved = transitionCurve == null
                    ? normalized
                    : transitionCurve.Evaluate(normalized);
                SetVisualPosition(Mathf.LerpUnclamped(start, target, curved));
                yield return null;
            }

            SetVisualPosition(target);
            transitionCoroutine = null;
        }

        private void SetVisualPosition(float normalized)
        {
            visualPosition = Mathf.Clamp01(normalized);

            if (handle)
            {
                handle.anchorMin = new Vector2(visualPosition, handle.anchorMin.y);
                handle.anchorMax = new Vector2(visualPosition, handle.anchorMax.y);
                handle.anchoredPosition =
                    new Vector2(0f, handle.anchoredPosition.y);
            }

            if (fill)
            {
                fill.anchorMin = new Vector2(0f, fill.anchorMin.y);
                fill.anchorMax = new Vector2(visualPosition, fill.anchorMax.y);
                fill.offsetMin = new Vector2(0f, fill.offsetMin.y);
                fill.offsetMax = new Vector2(0f, fill.offsetMax.y);
            }

            transitionProgressChanged?.Invoke(visualPosition);
            OnAnimated?.Invoke(visualPosition);
        }

        private void StopTransition()
        {
            if (transitionCoroutine == null) return;
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
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
            transitionDuration = Mathf.Max(0f, transitionDuration);
            EnsureTargetGraphic();

            if (!Application.isPlaying)
                SetVisualPosition(isOn ? 1f : 0f);
        }

        [ContextMenu("Preview On")]
        private void PreviewOn()
        {
            SetOn(true, false, false);
        }

        [ContextMenu("Preview Off")]
        private void PreviewOff()
        {
            SetOn(false, false, false);
        }
#endif
    }
}
