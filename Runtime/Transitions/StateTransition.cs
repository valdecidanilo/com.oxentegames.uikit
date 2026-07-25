using OxenteGames.UI.Transitions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OxenteGames.UI
{
    [Serializable]
    public class StateTransition<T> where T : Enum
    {
        public Graphic targetGraphic;

        #region Activators
        public bool colorTintTransition = true;
        public bool spriteSwapTransition;
        public bool animationTransition;
        public bool changeChildrenColor;
        public bool changeChildrenAlpha;
        public bool invertColorOnTexts;
        #endregion

        public float fadeDuration = .1f;

        public List<Graphic> childGraphics = new();
        public List<string> stateNames = new();
        public List<T> stateValues = new();
        public List<GraphicState> states = new();

        private GraphicState currentState;
        private AnimationPreset currentAnimation;
        private bool wasChangingChildrenColor;
        private bool wasChangingChildrenAlpha;
        private bool wasInvertingTextColors;

        public StateTransition()
        {
            SetupStates();
        }

        public void SetupStates()
        {
            var values = Enum.GetValues(typeof(T));
            foreach (var state in values)
            {
                stateNames.Add(state.ToString());
                stateValues.Add((T)state);
                states.Add(new());
            }
        }

        public void UpdateState(T state)
        {
            ResetTransitions();
            var stateIndex = stateValues.IndexOf(state);
            if (stateIndex < 0) return;
            currentState = states[stateIndex];
            if (colorTintTransition)
            {
                currentState.ColorTransition(targetGraphic, fadeDuration);
                UpdateChildGraphicsColor(currentState.color);
                UpdateTextColors(currentState.color);
            }
            if (spriteSwapTransition)
                currentState.SpriteTransition(targetGraphic as Image);
            if (animationTransition)
            {
                if (!Application.isPlaying || !targetGraphic.gameObject.activeInHierarchy) return;
                currentAnimation?.StopAnimation(targetGraphic);
                currentAnimation = currentState.AnimationTransition(targetGraphic);
            }
        }
        public void ResetTransitions()
        {
            if (!colorTintTransition)
            {
                GraphicState.TransitionToColor(targetGraphic, Color.white, 0, true);
                if (changeChildrenColor || changeChildrenAlpha || wasChangingChildrenColor || wasChangingChildrenAlpha)
                    ResetChildGraphicsColor();
                if (invertColorOnTexts || wasInvertingTextColors)
                    ResetTextColors();
            }
            if (!spriteSwapTransition)
                GraphicState.TransitionToSprite(targetGraphic as Image, null);

#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (!animationTransition) currentAnimation?.StopAnimation(targetGraphic);
        }

        private void UpdateChildGraphicsColor(Color targetColor)
        {
            if (childGraphics == null) return;

            for (int i = 0; i < childGraphics.Count; i++)
            {
                Graphic graphic = childGraphics[i];
                if (!graphic || graphic == targetGraphic) continue;

                if (changeChildrenColor)
                    GraphicState.TransitionToColor(graphic, targetColor, fadeDuration, false);
                else if (wasChangingChildrenColor)
                    GraphicState.TransitionToColor(graphic, Color.white, fadeDuration, false);

                if (changeChildrenAlpha)
                    graphic.CrossFadeAlpha(targetColor.a, fadeDuration, fadeDuration > 0);
                else if (wasChangingChildrenAlpha)
                    graphic.CrossFadeAlpha(1f, fadeDuration, fadeDuration > 0);
            }

            wasChangingChildrenColor = changeChildrenColor;
            wasChangingChildrenAlpha = changeChildrenAlpha;
        }

        private void UpdateTextColors(Color targetColor)
        {
            if (!targetGraphic) return;

            if (!invertColorOnTexts)
            {
                if (wasInvertingTextColors) ResetTextColors(targetColor, fadeDuration);
                return;
            }

            Color invertedColor = Color.white - targetColor;
            invertedColor.a = targetColor.a;

            TMP_Text[] childTexts = targetGraphic.GetComponentsInChildren<TMP_Text>();
            for (int i = 0; i < childTexts.Length; i++)
                GraphicState.TransitionToColor(childTexts[i], invertedColor, fadeDuration, true);

            wasInvertingTextColors = true;
        }

        private void ResetChildGraphicsColor()
        {
            if (childGraphics == null) return;

            bool resetColor = changeChildrenColor || wasChangingChildrenColor;
            bool resetAlpha = changeChildrenAlpha || wasChangingChildrenAlpha;

            for (int i = 0; i < childGraphics.Count; i++)
            {
                Graphic graphic = childGraphics[i];
                if (!graphic || graphic == targetGraphic) continue;

                if (resetColor)
                    GraphicState.TransitionToColor(graphic, Color.white, 0, false);
                if (resetAlpha)
                    graphic.CrossFadeAlpha(1f, 0, false);
            }

            wasChangingChildrenColor = false;
            wasChangingChildrenAlpha = false;
        }

        private void ResetTextColors(Color? targetColor = null, float duration = 0)
        {
            if (!targetGraphic) return;

            TMP_Text[] childTexts = targetGraphic.GetComponentsInChildren<TMP_Text>();
            for (int i = 0; i < childTexts.Length; i++)
            {
                TMP_Text text = childTexts[i];
                bool isListedChild = childGraphics != null && childGraphics.Contains(text);
                Color color = targetColor.HasValue && isListedChild && changeChildrenColor
                    ? targetColor.Value
                    : Color.white;
                float alpha = targetColor.HasValue && isListedChild && changeChildrenAlpha
                    ? targetColor.Value.a
                    : 1f;

                GraphicState.TransitionToColor(text, color, duration, false);
                text.CrossFadeAlpha(alpha, duration, duration > 0);
            }

            wasInvertingTextColors = false;
        }
    }
}
