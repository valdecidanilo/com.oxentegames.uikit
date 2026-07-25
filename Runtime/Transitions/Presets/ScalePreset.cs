using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace OxenteGames.UI.Transitions
{
    [CreateAssetMenu(fileName = "ScalePreset", menuName = "Oxente UI/Transitions/Scale Preset")]
    public class ScalePreset : CoroutineAnimationPreset
    {
        [SerializeField] private Vector3 scaleDirection = Vector3.one;

        public override void StartAnimation(MonoBehaviour button)
        {
            RectTransform rectTransform = (RectTransform)button.transform;
            var originalScale = rectTransform.localScale;
            base.StartAnimation(button);

            stopSequence[button] += () => rectTransform.localScale = originalScale;
        }

        protected override IEnumerator AnimationCoroutine(MonoBehaviour button)
        {
            RectTransform rectTransform = (RectTransform)button.transform;
            var originalScale = rectTransform.localScale;
            var targetScale = originalScale + (scaleDirection * magnitude);
            var elapsedTime = 0f;
            float startOffset = curveStart;
            float animationDuration = curveDuration;

            while (elapsedTime < duration || loopAnimation)
            {
                float currentTime = elapsedTime / duration;
                float t = curve.Evaluate((currentTime / animationDuration) + startOffset);
                rectTransform.localScale = originalScale + (targetScale - originalScale) * t;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            StopAnimation(button);
        }
    }
}
