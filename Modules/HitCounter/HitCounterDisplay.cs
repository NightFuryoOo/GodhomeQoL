using System.IO;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

internal static class HitCounterDisplay
{
    private const float PanelWidth = 420f;
    private const float Padding = 18f;
    private const float TitleHeight = 34f;
    private const float HeaderHeight = 20f;
    private const float RowHeight = 32f;
    private const float IconSize = 26f;
    private const float ColumnWidth = 52f;
    private const int PreviewSortOrder = 32000;
    private const int BelowReplayLoggerSortOrder = -100;
    private const float ScreenMargin = 32f;
    private const float MoveHintHeight = 24f;
    private const float BrowseRowHeight = 28f;
    private const float ArrowButtonWidth = 52f;
    private const float ArrowIconWidth = 16f;
    private const float ArrowIconHeight = 12f;
    private const int ArrowTextureSize = 32;

    private static readonly Color PlateColor = new(0.02f, 0.015f, 0.01f, 0.84f);
    private static readonly Color GoldColor = new(0.902f, 0.749f, 0.384f, 1f);
    private static readonly Color NameColor = new(0.93f, 0.9f, 0.84f, 1f);
    private static readonly Color HintColor = new(0.66f, 0.64f, 0.6f, 1f);
    private static readonly Color DoneNameColor = new(0.72f, 0.7f, 0.66f, 1f);
    private static readonly Color AheadColor = new(0.55f, 0.85f, 0.55f, 1f);
    private static readonly Color BehindColor = new(0.92f, 0.45f, 0.4f, 1f);
    private static readonly Color LineColor = new(0.902f, 0.749f, 0.384f, 0.55f);
    private static readonly Color CurrentRowColor = new(0.902f, 0.749f, 0.384f, 0.14f);
    private static readonly Color ArrowPlateColor = new(0.16f, 0.13f, 0.09f, 0.95f);
    private static readonly Color ArrowDisabledColor = new(0.45f, 0.43f, 0.4f, 0.6f);

    private static readonly Dictionary<string, Sprite?> icons = new(StringComparer.OrdinalIgnoreCase);

    private static GameObject? canvasObject;
    private static Canvas? canvas;
    private static CanvasGroup? group;
    private static int baseSortOrder;
    private static GameObject? panel;
    private static Sprite? arrowSprite;
    private static int manualFirst = -1;
    private static int shownFirst;
    private static int shownMaxFirst;

    internal static void ResetScroll() => manualFirst = -1;

    internal static void ScrollBy(int delta)
    {
        if (shownMaxFirst <= 0 || delta == 0)
        {
            return;
        }

        int next = NextFirst(shownFirst, delta, shownMaxFirst);
        if (next == shownFirst)
        {
            return;
        }

        manualFirst = next;
        HitCounter.Render();
    }

    internal static int NextFirst(int first, int delta, int maxFirst) =>
        Math.Min(Math.Max(first + delta, 0), Math.Max(maxFirst, 0));

    internal static int ChooseFirst(int count, int visible, int current, int manual, bool browsable)
    {
        int automatic = FirstVisibleSplit(count, visible, current);
        int maxFirst = Math.Max(0, count - visible);
        return browsable && maxFirst > 0 && manual >= 0
            ? Math.Min(manual, maxFirst)
            : automatic;
    }

    internal static void Render(HitCounterSession? session, bool onTop, bool movable, bool browsable)
    {
        if (session == null)
        {
            Hide();
            return;
        }

        EnsureCanvas();
        if (canvasObject == null || canvas == null)
        {
            return;
        }

        canvas.sortingOrder = onTop ? PreviewSortOrder : baseSortOrder;
        if (group != null)
        {
            group.blocksRaycasts = movable || browsable;
            group.interactable = browsable;
        }

        if (panel != null)
        {
            UObject.Destroy(panel);
        }

        panel = Build(session, movable, browsable && !movable);
        canvasObject.SetActive(true);
        ApplyStyle();
    }

    internal static void DragBy(Vector2 screenDelta)
    {
        if (panel == null || canvas == null || panel.transform is not RectTransform rect || canvas.transform is not RectTransform canvasRect)
        {
            return;
        }

        float factor = canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f;
        Vector2 delta = screenDelta / factor;
        float scale = HitCounter.ScaleClamped;
        float spanX = canvasRect.rect.width - (ScreenMargin * 2f) - (rect.rect.width * scale);
        float spanY = canvasRect.rect.height - (ScreenMargin * 2f) - (rect.rect.height * scale);
        float x = HitCounter.PositionXClamped;
        float y = HitCounter.PositionYClamped;
        if (spanX > 0f)
        {
            x = ((x * spanX) + delta.x) / spanX;
        }

        if (spanY > 0f)
        {
            y = ((y * spanY) + delta.y) / spanY;
        }

        HitCounter.SetPositionRaw(x, y);
        ApplyStyle();
    }

    private sealed class DragHandle : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                DragBy(eventData.delta);
            }
        }
    }

    private sealed class ScrollHandle : MonoBehaviour, IScrollHandler
    {
        public void OnScroll(PointerEventData eventData)
        {
            float wheel = eventData.scrollDelta.y;
            if (wheel > 0f)
            {
                ScrollBy(-1);
            }
            else if (wheel < 0f)
            {
                ScrollBy(1);
            }
        }
    }

    internal static void ApplyStyle()
    {
        if (group != null)
        {
            group.alpha = HitCounter.OpacityClamped;
        }

        if (panel != null)
        {
            float scale = HitCounter.ScaleClamped;
            panel.transform.localScale = new Vector3(scale, scale, 1f);
            if (panel.transform is RectTransform rect)
            {
                Vector2 point = new(HitCounter.PositionXClamped, HitCounter.PositionYClamped);
                rect.anchorMin = point;
                rect.anchorMax = point;
                rect.pivot = point;
                rect.anchoredPosition = new Vector2(Mathf.Lerp(ScreenMargin, -ScreenMargin, point.x), Mathf.Lerp(ScreenMargin, -ScreenMargin, point.y));
            }
        }
    }

    internal static void Destroy()
    {
        if (canvasObject != null)
        {
            UObject.Destroy(canvasObject);
        }

        canvasObject = null;
        canvas = null;
        group = null;
        panel = null;
    }

    private static void Hide()
    {
        if (panel != null)
        {
            UObject.Destroy(panel);
            panel = null;
        }

        if (canvasObject != null)
        {
            canvasObject.SetActive(false);
        }
    }

    private static void EnsureCanvas()
    {
        if (canvasObject != null)
        {
            return;
        }

        canvasObject = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080), "HitCounterCanvas", BelowReplayLoggerSortOrder);
        canvas = canvasObject.GetComponent<Canvas>();
        baseSortOrder = canvas != null ? canvas.sortingOrder : 0;
        group = canvasObject.GetComponent<CanvasGroup>() ?? canvasObject.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
        UObject.DontDestroyOnLoad(canvasObject);
    }

    private static GameObject Build(HitCounterSession session, bool movable, bool browsable)
    {
        GameObject root = new("HitCounterPanel");
        root.transform.SetParent(canvasObject!.transform, false);
        RectTransform rootRect = root.AddComponent<RectTransform>();

        int count = session.Splits.Count;
        int visible = Math.Min(count, HitCounter.VisibleSplitsClamped);
        int maxFirst = Math.Max(0, count - visible);
        bool canBrowse = browsable && maxFirst > 0;
        int first = ChooseFirst(count, visible, session.Current, manualFirst, canBrowse);
        shownFirst = first;
        shownMaxFirst = canBrowse ? maxFirst : 0;

        Image plate = root.AddComponent<Image>();
        plate.sprite = QuickMenu.HkUi.SoftPlate;
        plate.type = Image.Type.Sliced;
        plate.color = PlateColor;
        plate.raycastTarget = movable || canBrowse;
        if (movable)
        {
            root.AddComponent<DragHandle>();
        }
        else if (canBrowse)
        {
            root.AddComponent<ScrollHandle>();
        }

        float y = Padding - 4f;
        y = AddTitle(root, session, y);
        y = AddHeader(root, y);

        for (int i = first; i < first + visible; i++)
        {
            y = AddSplitRow(root, session, i, y);
        }

        if (canBrowse)
        {
            y = AddBrowseRow(root, first, visible, count, maxFirst, y + 4f);
        }

        if (!session.HallOfGods)
        {
            y = AddLine(root, y + 4f);
            y = AddTotalRow(root, session, y + 4f);
        }

        if (movable)
        {
            Text hint = CreateText(root, "MoveHint", "HitCounter/MoveHintShort".Localize(), QuickMenu.HkUi.BodyFont, 15, TextAnchor.MiddleCenter, GoldColor);
            Place(hint.rectTransform, Padding, y + 6f, PanelWidth - (Padding * 2f), MoveHintHeight);
            y += MoveHintHeight + 6f;
        }

        rootRect.sizeDelta = new Vector2(PanelWidth, y + Padding - 6f);

        if (movable)
        {
            Image frame = CreateImage(root, "MoveFrame", GoldColor);
            frame.sprite = QuickMenu.HkUi.ChamferLine;
            frame.type = Image.Type.Sliced;
            RectTransform frameRect = frame.rectTransform;
            frameRect.anchorMin = Vector2.zero;
            frameRect.anchorMax = Vector2.one;
            frameRect.offsetMin = new Vector2(-2f, -2f);
            frameRect.offsetMax = new Vector2(2f, 2f);
        }

        return root;
    }

    internal static int FirstVisibleSplit(int count, int visible, int current)
    {
        if (visible <= 0 || count <= visible)
        {
            return 0;
        }

        int upcoming = visible >= 2 ? 1 : 0;
        return Mathf.Clamp(Math.Max(current, 0) + upcoming - (visible - 1), 0, count - visible);
    }

    private static float AddTitle(GameObject root, HitCounterSession session, float y)
    {
        float innerWidth = PanelWidth - (Padding * 2f);
        string progress = session.HallOfGods
            ? HitCounter.LevelName(session.Tier)
            : $"{Math.Min(Math.Max(session.Current, 0) + 1, session.Splits.Count)}/{session.Splits.Count}";

        Text title = CreateText(root, "Title", session.Title, QuickMenu.HkUi.TitleFont, 20, TextAnchor.MiddleLeft, Color.white);
        title.resizeTextForBestFit = true;
        title.resizeTextMaxSize = 20;
        title.resizeTextMinSize = 12;
        Place(title.rectTransform, Padding, y, innerWidth - 96f, TitleHeight);

        Text counter = CreateText(root, "Progress", progress, QuickMenu.HkUi.BodyFont, 18, TextAnchor.MiddleRight, GoldColor);
        Place(counter.rectTransform, PanelWidth - Padding - 96f, y, 96f, TitleHeight);

        Image line = CreateImage(root, "TitleLine", LineColor);
        Place(line.rectTransform, Padding, y + TitleHeight + 2f, innerWidth, 2f);
        return y + TitleHeight + 8f;
    }

    private static float AddHeader(GameObject root, float y)
    {
        Text hits = CreateText(root, "HitsHeader", "Hits", QuickMenu.HkUi.BodyFont, 14, TextAnchor.MiddleRight, HintColor);
        Place(hits.rectTransform, PanelWidth - Padding - (ColumnWidth * 2f), y, ColumnWidth, HeaderHeight);
        Text pb = CreateText(root, "PbHeader", "PB", QuickMenu.HkUi.BodyFont, 14, TextAnchor.MiddleRight, HintColor);
        Place(pb.rectTransform, PanelWidth - Padding - ColumnWidth, y, ColumnWidth, HeaderHeight);
        return y + HeaderHeight;
    }

    private static float AddSplitRow(GameObject root, HitCounterSession session, int index, float y)
    {
        HitCounterSplit split = session.Splits[index];
        bool current = index == session.Current && !session.Ended;
        float innerWidth = PanelWidth - (Padding * 2f);

        if (current)
        {
            Image highlight = CreateImage(root, "Current", CurrentRowColor);
            Place(highlight.rectTransform, Padding - 6f, y, innerWidth + 12f, RowHeight);
        }

        float nameLeft = Padding;
        Sprite? icon = GetIcon(split.Image);
        if (icon != null)
        {
            Image iconImage = CreateImage(root, "Icon", Color.white);
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            Place(iconImage.rectTransform, Padding, y + ((RowHeight - IconSize) / 2f), IconSize, IconSize);
            nameLeft += IconSize + 8f;
        }

        string label = session.HallOfGods ? HitCounter.LevelName(session.Tier) : split.Key;
        Color nameColor = current ? GoldColor : split.Done ? DoneNameColor : NameColor;
        Text name = CreateText(root, "Name", label, QuickMenu.HkUi.BodyFont, 18, TextAnchor.MiddleLeft, nameColor);
        name.resizeTextForBestFit = true;
        name.resizeTextMaxSize = 18;
        name.resizeTextMinSize = 11;
        Place(name.rectTransform, nameLeft, y, PanelWidth - Padding - (ColumnWidth * 2f) - nameLeft - 4f, RowHeight);

        bool reached = split.Done || index <= session.Current;
        string hitsText = reached ? split.Hits.ToString() : "-";
        Text hits = CreateText(root, "Hits", hitsText, QuickMenu.HkUi.BodyFont, 18, TextAnchor.MiddleRight, HitsColor(split, reached));
        Place(hits.rectTransform, PanelWidth - Padding - (ColumnWidth * 2f), y, ColumnWidth, RowHeight);

        Text pb = CreateText(root, "Pb", split.Pb >= 0 ? split.Pb.ToString() : "-", QuickMenu.HkUi.BodyFont, 18, TextAnchor.MiddleRight, HintColor);
        Place(pb.rectTransform, PanelWidth - Padding - ColumnWidth, y, ColumnWidth, RowHeight);
        return y + RowHeight;
    }

    private static Color HitsColor(HitCounterSplit split, bool reached)
    {
        if (!reached)
        {
            return HintColor;
        }

        if (split.Done && split.BestAtStart >= 0 && split.Hits < split.BestAtStart)
        {
            return GoldColor;
        }

        if (split.Pb < 0)
        {
            return Color.white;
        }

        if (split.Hits > split.Pb)
        {
            return BehindColor;
        }

        return split.Done && split.Hits < split.Pb ? AheadColor : Color.white;
    }

    private static float AddTotalRow(GameObject root, HitCounterSession session, float y)
    {
        int total = session.Total;
        Color totalColor = session.NewPb
            ? GoldColor
            : session.PbTotal >= 0 && total > session.PbTotal ? BehindColor : Color.white;

        Text label = CreateText(root, "TotalLabel", "Total", QuickMenu.HkUi.TitleFont, 18, TextAnchor.MiddleLeft, Color.white);
        Place(label.rectTransform, Padding, y, PanelWidth - (Padding * 2f) - (ColumnWidth * 2f), RowHeight);

        Text hits = CreateText(root, "TotalHits", total.ToString(), QuickMenu.HkUi.BodyFont, 20, TextAnchor.MiddleRight, totalColor);
        Place(hits.rectTransform, PanelWidth - Padding - (ColumnWidth * 2f), y, ColumnWidth, RowHeight);

        Text pb = CreateText(root, "TotalPb", session.PbTotal >= 0 ? session.PbTotal.ToString() : "-", QuickMenu.HkUi.BodyFont, 20, TextAnchor.MiddleRight, GoldColor);
        Place(pb.rectTransform, PanelWidth - Padding - ColumnWidth, y, ColumnWidth, RowHeight);
        return y + RowHeight;
    }

    private static float AddBrowseRow(GameObject root, int first, int visible, int count, int maxFirst, float y)
    {
        AddArrowButton(root, "ScrollUp", Padding, y, up: true, enabled: first > 0);
        AddArrowButton(root, "ScrollDown", PanelWidth - Padding - ArrowButtonWidth, y, up: false, enabled: first < maxFirst);

        string range = $"{first + 1}-{first + visible} / {count}";
        Text label = CreateText(root, "ScrollRange", range, QuickMenu.HkUi.BodyFont, 15, TextAnchor.MiddleCenter, HintColor);
        Place(label.rectTransform, Padding + ArrowButtonWidth, y, PanelWidth - (Padding * 2f) - (ArrowButtonWidth * 2f), BrowseRowHeight);
        return y + BrowseRowHeight;
    }

    private static void AddArrowButton(GameObject root, string name, float left, float top, bool up, bool enabled)
    {
        Image back = CreateImage(root, name, ArrowPlateColor);
        back.sprite = QuickMenu.HkUi.SoftPlate;
        back.type = Image.Type.Sliced;
        back.raycastTarget = true;
        Place(back.rectTransform, left, top, ArrowButtonWidth, BrowseRowHeight);

        Button button = back.gameObject.AddComponent<Button>();
        button.targetGraphic = back;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.9f, 1.9f, 1.9f, 1f);
        colors.pressedColor = new Color(1.4f, 1.4f, 1.4f, 1f);
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
        colors.colorMultiplier = 1f;
        button.colors = colors;
        button.interactable = enabled;
        int step = up ? -1 : 1;
        button.onClick.AddListener(() => ScrollBy(step));

        Image arrow = CreateImage(back.gameObject, "Arrow", enabled ? GoldColor : ArrowDisabledColor);
        arrow.sprite = GetArrowSprite();
        RectTransform arrowRect = arrow.rectTransform;
        arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRect.pivot = new Vector2(0.5f, 0.5f);
        arrowRect.anchoredPosition = Vector2.zero;
        arrowRect.sizeDelta = new Vector2(ArrowIconWidth, ArrowIconHeight);
        arrowRect.localEulerAngles = up ? Vector3.zero : new Vector3(0f, 0f, 180f);
    }

    private static Sprite GetArrowSprite()
    {
        if (arrowSprite != null)
        {
            return arrowSprite;
        }

        int size = ArrowTextureSize;
        float center = size / 2f;
        float bottom = 4f;
        float top = size - 4f;
        float halfBase = center - 2f;
        Texture2D texture = new(size, size, TextureFormat.ARGB32, false);
        for (int py = 0; py < size; py++)
        {
            float rowY = py + 0.5f;
            float t = Mathf.InverseLerp(bottom, top, rowY);
            float half = rowY < bottom || rowY > top ? -1f : (1f - t) * halfBase;
            for (int px = 0; px < size; px++)
            {
                float alpha = half < 0f ? 0f : Mathf.Clamp01(half - Mathf.Abs(px + 0.5f - center) + 0.5f);
                texture.SetPixel(px, py, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.Apply();
        arrowSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        return arrowSprite;
    }

    private static float AddLine(GameObject root, float y)
    {
        Image line = CreateImage(root, "Line", LineColor);
        Place(line.rectTransform, Padding, y, PanelWidth - (Padding * 2f), 1f);
        return y + 1f;
    }

    private static Text CreateText(GameObject parent, string name, string content, Font font, int size, TextAnchor alignment, Color color)
    {
        GameObject textObject = new(name);
        textObject.transform.SetParent(parent.transform, false);
        textObject.AddComponent<RectTransform>();
        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = color;
        text.text = content;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static Image CreateImage(GameObject parent, string name, Color color)
    {
        GameObject imageObject = new(name);
        imageObject.transform.SetParent(parent.transform, false);
        imageObject.AddComponent<RectTransform>();
        Image image = imageObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static void Place(RectTransform rect, float left, float top, float width, float height)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(left, -top);
        rect.sizeDelta = new Vector2(width, height);
    }

    private static Sprite? GetIcon(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        if (icons.TryGetValue(fileName, out Sprite? cached))
        {
            return cached;
        }

        Sprite? sprite = null;
        try
        {
            Assembly assembly = typeof(HitCounterDisplay).Assembly;
            string? resource = assembly.GetManifestResourceNames()
                .FirstOrDefault(name => name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
            if (resource != null)
            {
                using Stream? stream = assembly.GetManifestResourceStream(resource);
                if (stream != null)
                {
                    using MemoryStream buffer = new();
                    stream.CopyTo(buffer);
                    Texture2D texture = new(2, 2, TextureFormat.ARGB32, false);
                    if (texture.LoadImage(buffer.ToArray()))
                    {
                        texture.wrapMode = TextureWrapMode.Clamp;
                        texture.filterMode = FilterMode.Bilinear;
                        sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "HitCounterDisplay.cs/Icon");
        }

        icons[fileName] = sprite;
        return sprite;
    }
}
