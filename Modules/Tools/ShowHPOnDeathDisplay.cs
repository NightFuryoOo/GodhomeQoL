using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

internal sealed class ShowHPOnDeathDisplay
{
    internal static ShowHPOnDeathDisplay? Instance;

    private string displayText = "";
    private readonly Vector2 textSize = new(800, 500);
    private readonly Vector2 textPosition = new(0.78f, 0.243f);

    private GameObject? canvas;
    private UnityEngine.UI.Text? text;

    public ShowHPOnDeathDisplay()
    {
        Instance = this;
        Create();
    }

    private void Create()
    {
        if (canvas != null)
        {
            return;
        }

        canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080), "ShowHPOnDeathCanvas");

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        UObject.DontDestroyOnLoad(canvas);

        text = CanvasUtil.CreateTextPanel(
            canvas,
            "",
            24,
            TextAnchor.LowerRight,
            new CanvasUtil.RectData(textSize, Vector2.zero, textPosition, textPosition),
            CanvasUtil.GetFont("Perpetua")
        ).GetComponent<UnityEngine.UI.Text>();
    }

    public void Destroy()
    {
        if (canvas != null)
        {
            UObject.Destroy(canvas);
        }

        canvas = null;
        text = null;
        Instance = null;
    }

    private void Update()
    {
        if (text == null || canvas == null)
        {
            return;
        }

        text.text = displayText;
        canvas.SetActive(true);
    }

    public void Display(string value)
    {
        displayText = value.Trim();
        Update();
    }
}
