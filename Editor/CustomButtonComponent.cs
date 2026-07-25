#if UNITY_EDITOR
using OxenteGames.UI.Transitions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace OxenteGames.UI.Editor
{
    public abstract class CustomButtonComponent : EditorWindow
    {
        [MenuItem("GameObject/UI (Canvas)/Oxente UI/Custom Button", false, 31)]
        private static void AddCustomButton(MenuCommand menuCommand)
        {
            /* TODO: Verificação para substituir
            var shouldDestroy = EditorUtility.DisplayDialog("Aviso", "Já existe uma instância do CustomButton. Deseja substituí-la?", "Sim", "Não");
            if (shouldDestroy)
            {
                // Destruir a instância mais recente
                var existingButtons = FindObjectsOfType<CustomButton>();
                if (existingButtons.Length > 0)
                {
                    DestroyImmediate(existingButtons[existingButtons.Length - 1].gameObject);
                }
            }
            */
            var obj = menuCommand.context as GameObject;
            var rectTransform = obj?.GetComponent<RectTransform>();
            var canvas = FindCanvasInHierarchy(menuCommand);

            if (rectTransform != null)
            {
                if (canvas != null && RectTransformUtility.RectangleContainsScreenPoint(canvas.GetComponent<RectTransform>(), rectTransform.position))
                    menuCommand.context = rectTransform.gameObject;
                else
                    menuCommand.context = canvas.gameObject;
            }

            var customButtonObject = new GameObject("Custom Button");
            var textObject = new GameObject("Text (TMP)");

            EditorGUIUtility.PingObject(customButtonObject);
            EditorApplication.delayCall += () => Selection.activeGameObject = customButtonObject;

            var buttonObjectRT = customButtonObject.AddComponent<RectTransform>();
            var textRT = textObject.AddComponent<RectTransform>();
            
            ApplySpriteButton(customButtonObject.TryGetComponent(out Image image) ? image : customButtonObject.AddComponent<Image>());
            buttonObjectRT.sizeDelta = new Vector2(160f, 30f);
            textRT.sizeDelta = Vector2.zero;

            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;

            textRT.sizeDelta = Vector2.zero;

            var parentObject = menuCommand.context as GameObject;
            if (parentObject != null)
            {
                customButtonObject.transform.SetParent(parentObject.transform, false);
                textObject.transform.SetParent(customButtonObject.transform, false);
            }

            ConfigureLabel(textObject);
            
            var custombutton = customButtonObject.AddComponent<CustomButton>();
            const string assetShake = "DefaultPresets/ShakePreset";
            var defaultPreset = Resources.Load<AnimationPreset>(assetShake);

            custombutton.Transition = new() { targetGraphic = customButtonObject.GetComponent<Image>() };
            Undo.RegisterCreatedObjectUndo(customButtonObject, "Create " + customButtonObject.name);
            custombutton.OnTransformChildrenChanged();
            
        }

        private static void ConfigureLabel(GameObject textObject)
        {
            var color = ColorUtility.TryParseHtmlString("#323232", out var parsedColor)
                ? parsedColor
                : Color.black;

            var tmpSettings = Resources.Load<TMP_Settings>("TMP Settings");
            if (tmpSettings)
            {
                var text = textObject.AddComponent<TextMeshProUGUI>();
                text.fontSize = 17;
                text.alignment = TextAlignmentOptions.Center;
                text.color = color;
                text.text = "Custom Button";
                return;
            }

            var legacyText = textObject.AddComponent<Text>();
            legacyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            legacyText.fontSize = 17;
            legacyText.alignment = TextAnchor.MiddleCenter;
            legacyText.color = color;
            legacyText.text = "Custom Button";

            Debug.LogWarning(
                "[Oxente UI] TMP Essential Resources were not found. " +
                "The Custom Button label was created with Unity UI Text.");
        }
        private static void ApplySpriteButton(Image image)
        {
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 7f;
            image.fillCenter = true;
            
            const string assetPath = "Textures/UISprite-CB-Base";
            var sprite = Resources.Load<Sprite>(assetPath);

            if (sprite != null)
                image.sprite = sprite;
        }
        private static Canvas FindCanvasInHierarchy(MenuCommand menuCommand)
        {
            Canvas foundCanvas = null;
            var canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude);
            foreach (var canvas in canvases)
            {
                if (!canvas.isActiveAndEnabled) continue;
                menuCommand.context = canvas.gameObject;
                foundCanvas = canvas;
            }

            if (!foundCanvas)// if canvas not found, create one
            {
                foundCanvas = CreateCanvas(menuCommand);
            }
            
            //look for eventSystem
            if(!FindAnyObjectByType<EventSystem>()) CreateEventSystem();
            
            return foundCanvas;
        }
        private static Canvas CreateCanvas(MenuCommand menuCommand)
        {
            var canvasObject = new GameObject("Canvas");
            menuCommand.context = canvasObject;

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystem = new("Event System");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
    
}
#endif
