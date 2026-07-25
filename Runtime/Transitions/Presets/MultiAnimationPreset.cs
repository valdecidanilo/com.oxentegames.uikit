using UnityEngine;

namespace OxenteGames.UI.Transitions
{
    [CreateAssetMenu(fileName = "MultiAnimationPreset", menuName = "Oxente UI/Transitions/Multi Animation Preset")]
    public class MultiAnimationPreset : AnimationPreset
    {
        [Header("Above parameters don't work"),SerializeField, Space(10)] AnimationPreset[] presets;

        public override void StartAnimation(MonoBehaviour button)
        {
            for (int i = 0; i < presets.Length; i++)
            {
                if (presets[i] != this) //To prevent stackOverflow
                {
                    presets[i].StartAnimation(button);
                }
            }
        }

        public override void StopAnimation(MonoBehaviour button)
        {
            for (int i = 0; i < presets.Length; i++)
            {
                if (presets[i] != this) //To prevent stackOverflow
                {
                    presets[i].StopAnimation(button);
                }
            }
        }
    }
}
