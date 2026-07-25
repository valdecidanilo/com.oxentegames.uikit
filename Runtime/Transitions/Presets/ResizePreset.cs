using System.Collections;
using UnityEngine;

namespace OxenteGames.UI.Transitions
{
    [CreateAssetMenu(
        fileName = "ResizePreset",
        menuName = "Oxente UI/Transitions/Resize Preset")]
    public class ResizePreset : CoroutineAnimationPreset
    {
        [SerializeField] private Vector2 resizeDirection = Vector2.one;

        public override void StartAnimation(MonoBehaviour button)
        {
            var rectTransform = (RectTransform)button.transform;
            var originalSize = rectTransform.sizeDelta;
            base.StartAnimation(button);

            stopSequence[button] += () => rectTransform.sizeDelta = originalSize;
        }

        protected override IEnumerator AnimationCoroutine(MonoBehaviour button)
        {
            var rectTransform = (RectTransform)button.transform;
            var originalSize = rectTransform.sizeDelta;
            var targetSize = originalSize + resizeDirection * magnitude;
            var elapsedTime = 0f;
            var startOffset = curveStart;
            var animationDuration = curveDuration;

            while (elapsedTime < duration || loopAnimation)
            {
                var currentTime = elapsedTime / duration;
                var t = curve.Evaluate(currentTime / animationDuration + startOffset);
                rectTransform.sizeDelta =
                    originalSize + (targetSize - originalSize) * t;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            StopAnimation(button);
        }
    }
}
