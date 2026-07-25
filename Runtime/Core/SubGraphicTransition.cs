using UnityEngine;
using UnityEngine.UI;

namespace OxenteGames.UI
{
    public class SubGraphicTransition : MonoBehaviour
    {
        [SerializeField] private CustomButton customButton;
        [Space, SerializeField] private StateTransition<SelectionState> Transition;

        private void Reset()
        {
            customButton = GetComponentInParent<CustomButton>();
            Transition = new StateTransition<SelectionState>();
            TryGetComponent(out Transition.targetGraphic);
        }

        private void Awake() => customButton.onStateChange += UpdateStage;

        private void UpdateStage(SelectionState state) => Transition.UpdateState(state);
    }
}
