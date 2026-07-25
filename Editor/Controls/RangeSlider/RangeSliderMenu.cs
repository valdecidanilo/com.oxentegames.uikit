using OxenteGames.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OxenteGames.UI.Editor
{
    /// <summary>
    /// Cria um "Slider Range" (2 handles) pelo menu GameObject/UI, com os sprites padrão
    /// do Unity (Background/UISprite/Knob) — mesma pegada visual do Slider nativo.
    /// </summary>
    public static class RangeSliderMenu
    {
        [MenuItem("GameObject/UI (Canvas)/Oxente UI/Range Slider", false, 2036)]
        public static void CreateSliderRange(MenuCommand menuCommand)
        {
            var bgSprite   = LoadBuiltin("UI/Skin/Background.psd");
            var fillSprite = LoadBuiltin("UI/Skin/UISprite.psd");
            var knobSprite = LoadBuiltin("UI/Skin/Knob.psd");

            // Root: trilho + recebe o arraste (RangeSlider + Image raycastTarget).
            var root = NewUI("Slider Range");
            var rootRt = (RectTransform)root.transform;
            rootRt.sizeDelta = new Vector2(200f, 24f);

            var rootImg = root.AddComponent<Image>();
            rootImg.sprite = bgSprite;
            rootImg.type = Image.Type.Sliced;
            rootImg.raycastTarget = true;

            var slider = root.AddComponent<RangeSlider>();
            slider.targetGraphic = rootImg;

            // SlideArea: área de deslize, com padding lateral = raio do handle.
            var slideArea = NewUI("SlideArea", root);
            var saRt = (RectTransform)slideArea.transform;
            saRt.anchorMin = new Vector2(0f, 0f);
            saRt.anchorMax = new Vector2(1f, 1f);
            saRt.offsetMin = new Vector2(10f, 0f);
            saRt.offsetMax = new Vector2(-10f, 0f);

            // Fill entre os handles.
            var fill = NewUI("Fill", slideArea);
            var fillImg = fill.AddComponent<Image>();
            fillImg.sprite = fillSprite;
            fillImg.type = Image.Type.Sliced;
            fillImg.color = new Color(0.34f, 0.75f, 1f, 1f);
            fillImg.raycastTarget = false;
            var fillRt = (RectTransform)fill.transform;
            fillRt.anchorMin = new Vector2(0.4f, 0.25f);
            fillRt.anchorMax = new Vector2(0.6f, 0.75f);
            fillRt.offsetMin = Vector2.zero;
            fillRt.offsetMax = Vector2.zero;

            // Handles (knob padrão), já posicionados em 0.4/0.6 (≈ centralizado).
            var lowRt  = NewHandle("LowHandle", slideArea, knobSprite, 0.4f);
            var highRt = NewHandle("HighHandle", slideArea, knobSprite, 0.6f);

            // Liga referências no componente.
            var so = new SerializedObject(slider);
            so.FindProperty("slideArea").objectReferenceValue  = saRt;
            so.FindProperty("fill").objectReferenceValue       = fillRt;
            so.FindProperty("lowHandle").objectReferenceValue  = lowRt;
            so.FindProperty("highHandle").objectReferenceValue = highRt;
            so.ApplyModifiedPropertiesWithoutUndo();
            slider.SetValuesWithoutNotify(25, 75);

            // Coloca sob o Canvas (cria Canvas/EventSystem se preciso), igual ao Slider.
            PlaceUnderCanvas(root, menuCommand);

            Undo.RegisterCreatedObjectUndo(root, "Create Slider Range");
            Selection.activeGameObject = root;
        }

        // ── helpers ───────────────────────────────────────────────────────────────
        private static Sprite LoadBuiltin(string path)
        {
            var s = AssetDatabase.GetBuiltinExtraResource<Sprite>(path);
            if (s == null) Debug.LogWarning($"[SliderRange] Sprite padrão não encontrado: {path}");
            return s;
        }

        private static GameObject NewUI(string name, GameObject parent = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            if (parent != null) go.transform.SetParent(parent.transform, false);
            return go;
        }

        private static RectTransform NewHandle(string name, GameObject parent, Sprite knob, float t)
        {
            var h = NewUI(name, parent);
            var img = h.AddComponent<Image>();
            img.sprite = knob;
            img.raycastTarget = false;
            var rt = (RectTransform)h.transform;
            rt.anchorMin = new Vector2(t, 0f);
            rt.anchorMax = new Vector2(t, 1f);
            rt.sizeDelta = new Vector2(20f, 0f);
            rt.anchoredPosition = Vector2.zero;
            return rt;
        }

        private static void PlaceUnderCanvas(GameObject element, MenuCommand menuCommand)
        {
            var parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
                parent = GetOrCreateCanvas();

            GameObjectUtility.SetParentAndAlign(element, parent);
            ((RectTransform)element.transform).anchoredPosition = Vector2.zero;

            EnsureEventSystem();
        }

        private static GameObject GetOrCreateCanvas()
        {
            var existing = Object.FindAnyObjectByType<Canvas>();
            if (existing != null) return existing.gameObject;

            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.layer = LayerMask.NameToLayer("UI");
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
            return go;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
        }
    }
}
