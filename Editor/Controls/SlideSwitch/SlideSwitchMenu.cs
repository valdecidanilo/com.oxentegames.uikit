using TMPro;
using OxenteGames.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OxenteGames.UI.Editor
{
    /// <summary>
    /// Cria um "Slide Switch" (toggle on/off com lerp) pelo menu GameObject/UI, com os
    /// sprites padrão do Unity (Background/Knob) — mesma pegada do Slider/Toggle nativos.
    /// </summary>
    public static class SlideSwitchMenu
    {
        [MenuItem("GameObject/UI (Canvas)/Oxente UI/Slide Switch", false, 2037)]
        public static void CreateSlideSwitch(MenuCommand menuCommand)
        {
            var bgSprite   = LoadBuiltin("UI/Skin/Background.psd");
            var fillSprite = LoadBuiltin("UI/Skin/UISprite.psd");
            var knobSprite = LoadBuiltin("UI/Skin/Knob.psd");

            const float handleSize = 26f;

            // Root: trilho + recebe o clique (SlideSwitch + Image raycastTarget).
            var root = NewUI("Slide Switch");
            var rootRt = (RectTransform)root.transform;
            rootRt.sizeDelta = new Vector2(60f, 30f);

            var rootImg = root.AddComponent<Image>();
            rootImg.sprite = bgSprite;
            rootImg.type = Image.Type.Sliced;
            rootImg.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            rootImg.raycastTarget = true;

            var switchComp = root.AddComponent<SlideSwitch>();
            var visual     = root.AddComponent<SlideSwitchVisual>();
            var labelPresenter = root.AddComponent<SlideSwitchLabel>();
            switchComp.targetGraphic = rootImg;

            // SlideArea: área de deslize, com padding lateral = raio do handle (igual Slider).
            var slideArea = NewUI("SlideArea", root);
            var saRt = (RectTransform)slideArea.transform;
            saRt.anchorMin = new Vector2(0f, 0f);
            saRt.anchorMax = new Vector2(1f, 1f);
            saRt.offsetMin = new Vector2(handleSize / 2f, 0f);
            saRt.offsetMax = new Vector2(-handleSize / 2f, 0f);

            // Fill: cresce da esquerda até o handle.
            var fill = NewUI("Fill", slideArea);
            var fillImg = fill.AddComponent<Image>();
            fillImg.sprite = fillSprite;
            fillImg.type = Image.Type.Sliced;
            fillImg.color = new Color(0.34f, 0.75f, 1f, 1f);
            fillImg.raycastTarget = false;
            var fRt = (RectTransform)fill.transform;
            fRt.anchorMin = new Vector2(0f, 0.2f);
            fRt.anchorMax = new Vector2(0f, 0.8f);
            fRt.offsetMin = Vector2.zero;
            fRt.offsetMax = Vector2.zero;

            // Handle (knob), posição controlada por âncora (x = 0..1).
            var handle = NewUI("Handle", slideArea);
            var handleImg = handle.AddComponent<Image>();
            handleImg.sprite = knobSprite;
            handleImg.raycastTarget = false;
            var hRt = (RectTransform)handle.transform;
            hRt.anchorMin = new Vector2(0f, 0.5f);
            hRt.anchorMax = new Vector2(0f, 0.5f);
            hRt.sizeDelta = new Vector2(handleSize, handleSize);
            hRt.anchoredPosition = Vector2.zero;

            // Label TextMeshPro (começa "Off"). Stretch sobre o switch.
            var labelGo = NewUI("Label", root);
            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = "Off";
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = true;
            label.fontSizeMin = 6f;
            label.fontSizeMax = 18f;
            label.raycastTarget = false;
            label.color = Color.white;
            var lRt = (RectTransform)labelGo.transform;
            lRt.anchorMin = Vector2.zero;
            lRt.anchorMax = Vector2.one;
            lRt.offsetMin = Vector2.zero;
            lRt.offsetMax = Vector2.zero;

            // Liga referências no SlideSwitch.
            var so = new SerializedObject(switchComp);
            so.FindProperty("handle").objectReferenceValue = hRt;
            so.FindProperty("fill").objectReferenceValue   = fRt;
            so.ApplyModifiedPropertiesWithoutUndo();
            switchComp.SetIsOnWithoutNotify(false);

            var labelSo = new SerializedObject(labelPresenter);
            labelSo.FindProperty("slideSwitch").objectReferenceValue = switchComp;
            labelSo.FindProperty("label").objectReferenceValue = label;
            labelSo.ApplyModifiedPropertiesWithoutUndo();

            // Visual custom (opcional): já vem ligado ao switch, mas SEM efeito por padrão —
            // o fill já indica on/off. O jogo liga applyColor (cor) e/ou applySprite no
            // Inspector, ou usa direto os UnityEvents do switch.
            var vso = new SerializedObject(visual);
            vso.FindProperty("slideSwitch").objectReferenceValue = switchComp;
            vso.FindProperty("colorTarget").objectReferenceValue = rootImg;
            vso.FindProperty("applyColor").boolValue = false;
            vso.ApplyModifiedPropertiesWithoutUndo();

            PlaceUnderCanvas(root, menuCommand);

            Undo.RegisterCreatedObjectUndo(root, "Create Slide Switch");
            Selection.activeGameObject = root;
        }

        // ── helpers ───────────────────────────────────────────────────────────────
        private static Sprite LoadBuiltin(string path)
        {
            var s = AssetDatabase.GetBuiltinExtraResource<Sprite>(path);
            if (s == null) Debug.LogWarning($"[SlideSwitch] Sprite padrão não encontrado: {path}");
            return s;
        }

        private static GameObject NewUI(string name, GameObject parent = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            if (parent != null) go.transform.SetParent(parent.transform, false);
            return go;
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
