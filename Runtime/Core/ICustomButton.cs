using UnityEngine.EventSystems;

namespace OxenteGames.UI
{
    public interface ICustomButton : ISubmitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
    {
        public void OnClick();
    }
}
