using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

internal static class HudTextScales
{
    internal const float Min = 0.5f;
    internal const float Max = 2f;

    internal static float SpeedText
    {
        get => SpeedChanger.SpeedChangerTextScale;
        set => SpeedChanger.SpeedChangerTextScale = value;
    }

    internal static float MaskDamageText
    {
        get => MaskDamage.MaskDamageTextScale;
        set => MaskDamage.MaskDamageTextScale = value;
    }

    internal static float NailDamageText
    {
        get => NailDamageCheck.NailDamageTextScale;
        set => NailDamageCheck.NailDamageTextScale = value;
    }

    internal static float InvincibleText
    {
        get => Modules.QoL.InvincibleIndicator.InvincibleTextScale;
        set => Modules.QoL.InvincibleIndicator.InvincibleTextScale = value;
    }

    internal static float FreezeHint
    {
        get => FreezeHitboxes.FreezeHintTextScale;
        set => FreezeHitboxes.FreezeHintTextScale = value;
    }

    internal static float Clamp(float value) => Mathf.Clamp(value, Min, Max);

    internal static int ToPercent(float value) => Mathf.RoundToInt(Clamp(value) * 100f);

    internal static float FromPercent(float percent) => Mathf.Clamp(Mathf.Round(percent), Min * 100f, Max * 100f) / 100f;

    internal static void ResetAll()
    {
        SpeedText = 1f;
        MaskDamageText = 1f;
        NailDamageText = 1f;
        InvincibleText = 1f;
        FreezeHint = 1f;
    }
}

internal sealed class HudLabel
{
    internal const int BelowReplayLoggerSortOrder = -100;
    private const int PreviewSortOrder = 32000;
    private const float PreviewSeconds = 3f;
    private const float PaddingX = 14f;
    private const float PaddingY = 5f;
    private const string ValueHex = "E6BF62";

    private static readonly Color PlateColor = new(0.02f, 0.015f, 0.01f, 0.72f);
    private static readonly Color TextColor = new(0.93f, 0.9f, 0.84f, 1f);
    private static readonly Color HighlightColor = new(0.902f, 0.749f, 0.384f, 1f);

    private readonly GameObject canvasObject;
    private readonly Canvas canvas;
    private readonly int sortOrder;
    private readonly RectTransform plate;
    private readonly Text text;
    private readonly Func<float> scale;
    private readonly Updater updater;
    private bool shown;
    private bool previewing;
    private int previewGeneration;

    internal HudLabel(string name, Vector2 anchor, Vector2 offset, int fontSize, Func<float> scale, int sortOrder = BelowReplayLoggerSortOrder, bool highlight = false)
    {
        this.scale = scale;
        this.sortOrder = sortOrder;
        canvasObject = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080), name, sortOrder);
        canvas = canvasObject.GetComponent<Canvas>();
        CanvasGroup group = canvasObject.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
        UObject.DontDestroyOnLoad(canvasObject);

        GameObject plateObject = new("Plate");
        plateObject.transform.SetParent(canvasObject.transform, false);
        plate = plateObject.AddComponent<RectTransform>();
        plate.anchorMin = anchor;
        plate.anchorMax = anchor;
        plate.pivot = anchor;
        plate.anchoredPosition = offset;

        Image image = plateObject.AddComponent<Image>();
        image.sprite = QuickMenu.HkUi.SoftPlate;
        image.type = Image.Type.Sliced;
        image.color = PlateColor;
        image.raycastTarget = false;

        GameObject textObject = new("Text");
        textObject.transform.SetParent(plate, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(PaddingX, PaddingY);
        textRect.offsetMax = new Vector2(-PaddingX, -PaddingY);

        text = textObject.AddComponent<Text>();
        text.font = QuickMenu.HkUi.BodyFont;
        text.fontSize = fontSize;
        text.color = highlight ? HighlightColor : TextColor;
        text.alignment = TextAnchor.MiddleLeft;
        text.supportRichText = true;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        updater = canvasObject.AddComponent<Updater>();
        updater.Tick = ApplyScale;
        canvasObject.SetActive(false);
    }

    internal bool IsShown => shown;

    internal Vector2 Offset
    {
        get => plate.anchoredPosition;
        set => plate.anchoredPosition = value;
    }

    internal float ScaledWidth => plate.sizeDelta.x * plate.localScale.x;

    internal float ScaledHeight => plate.sizeDelta.y * plate.localScale.y;

    internal void AddUpdater(Action tick)
    {
        updater.Tick += tick;
    }

    internal void Show(string value)
    {
        shown = true;
        Refresh();
        SetText(value);
    }

    internal void Hide()
    {
        shown = false;
        Refresh();
    }

    internal void ApplyScale()
    {
        float value = HudTextScales.Clamp(scale());
        plate.localScale = new Vector3(value, value, 1f);
    }

    internal void Preview(string value)
    {
        previewing = true;
        int generation = ++previewGeneration;
        Refresh();
        if (!shown)
        {
            SetText(value);
        }

        _ = GlobalCoroutineExecutor.Start(EndPreview(generation));
    }

    internal void Destroy()
    {
        previewGeneration++;
        if (canvasObject != null)
        {
            UObject.Destroy(canvasObject);
        }
    }

    private IEnumerator EndPreview(int generation)
    {
        yield return new WaitForSecondsRealtime(PreviewSeconds);
        if (generation == previewGeneration && canvasObject != null)
        {
            previewing = false;
            Refresh();
        }
    }

    private void SetText(string value)
    {
        text.text = Format(value ?? string.Empty);
        plate.sizeDelta = new Vector2(text.preferredWidth + (PaddingX * 2f), text.preferredHeight + (PaddingY * 2f));
    }

    private void Refresh()
    {
        if (canvasObject == null)
        {
            return;
        }

        ApplyScale();
        canvas.sortingOrder = previewing ? PreviewSortOrder : sortOrder;
        canvasObject.SetActive(shown || previewing);
    }

    private static string Format(string value)
    {
        string trimmed = value.Trim();
        int colon = trimmed.IndexOf(": ", StringComparison.Ordinal);
        if (colon <= 0 || colon + 2 >= trimmed.Length)
        {
            return trimmed;
        }

        return trimmed.Substring(0, colon + 1) + " <color=#" + ValueHex + ">" + trimmed.Substring(colon + 2) + "</color>";
    }

    private sealed class Updater : MonoBehaviour
    {
        internal Action? Tick;

        private void LateUpdate() => Tick?.Invoke();
    }
}
