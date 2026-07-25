using UnityEngine;
using UnityEngine.UI;

namespace OxenteGames.UI
{
    /// <summary>
    /// Handler visual OPCIONAL para o <see cref="SlideSwitch"/>. Aplica COR (lerp com o
    /// progresso) e/ou troca de SPRITE conforme on/off. O jogo escolhe usar cor, sprite,
    /// os dois — ou nenhum e ligar direto nos UnityEvents do switch no Inspector.
    /// </summary>
    public class SlideSwitchVisual : MonoBehaviour
    {
        [SerializeField] private SlideSwitch slideSwitch;

        [Header("Cor (lerp com o progresso)")]
        [SerializeField] private bool applyColor = true;
        [SerializeField] private Graphic colorTarget;
        [SerializeField] private Color offColor = new(0.5f, 0.5f, 0.5f, 1f);
        [SerializeField] private Color onColor  = new(0.34f, 0.75f, 1f, 1f);

        [Header("Sprite (troca no on/off)")]
        [SerializeField] private bool applySprite;
        [SerializeField] private Image spriteTarget;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;

        private void Awake()
        {
            if (slideSwitch == null) slideSwitch = GetComponent<SlideSwitch>();
        }

        private void OnEnable()
        {
            if (slideSwitch == null) return;
            slideSwitch.OnAnimated     += HandleAnimated;
            slideSwitch.OnValueChanged += HandleValueChanged;

            // Estado inicial.
            HandleAnimated(slideSwitch.IsOn ? 1f : 0f);
            HandleValueChanged(slideSwitch.IsOn);
        }

        private void OnDisable()
        {
            if (slideSwitch == null) return;
            slideSwitch.OnAnimated     -= HandleAnimated;
            slideSwitch.OnValueChanged -= HandleValueChanged;
        }

        private void HandleAnimated(float t)
        {
            if (applyColor && colorTarget)
                colorTarget.color = Color.Lerp(offColor, onColor, t);
        }

        private void HandleValueChanged(bool on)
        {
            if (applySprite && spriteTarget)
                spriteTarget.sprite = on ? onSprite : offSprite;
        }
    }
}
