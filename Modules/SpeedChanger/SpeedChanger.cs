using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Satchel.BetterMenus;
using System.Globalization;
using UnityEngine.UI;

namespace GodhomeQoL.Modules.Tools;

public sealed partial class SpeedChanger : Module {
	private static readonly string[] ToggleKeyOptions = new[] {
		"F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12",
		"BackQuote", "Tab", "CapsLock", "LeftShift", "RightShift", "LeftControl", "RightControl", "LeftAlt", "RightAlt", "Space",
		"Alpha1", "Alpha2", "Alpha3", "Alpha4", "Alpha5", "Alpha6", "Alpha7", "Alpha8", "Alpha9", "Alpha0",
		"Minus", "Equals",
		"Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P",
		"A", "S", "D", "F", "G", "H", "J", "K", "L",
		"Z", "X", "C", "V", "B", "N", "M",
		"Insert", "Home", "PageUp", "Delete", "End", "PageDown",
		"UpArrow", "DownArrow", "LeftArrow", "RightArrow"
	};

	private static readonly string[] AllowedRoomsForToggle = new[] {
		"GG_Workshop",
		"GG_Atrium",
		"GG_Atrium_Roof"
	};

	private static readonly MethodInfo[] FreezeCoroutines = (
		from method in typeof(GameManager).GetMethods()
		where method.Name.StartsWith("FreezeMoment", StringComparison.Ordinal)
		where method.ReturnType == typeof(IEnumerator)
		select method.GetCustomAttribute<IteratorStateMachineAttribute>() into attr
		select attr.StateMachineType into type
		select type.GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance)
	).ToArray();

	internal static SpeedChanger? Instance { get; private set; }
	private static readonly Dictionary<int, float> timeScaleOverrideValues = new();
	private static readonly HashSet<int> timeScaleFreezeLocks = new();
	private static int nextTimeScaleOverrideHandle;
	private static int nextFreezeLockHandle;

	public override bool DefaultEnabled => false;
	public override bool Hidden => true;
	public override bool AlwaysEnabled => true;

	[GlobalSetting] public static bool globalSwitch = false;
	[GlobalSetting] public static bool restrictToggleToRooms = false;
	[GlobalSetting] public static bool unlimitedSpeed = false;
	[GlobalSetting] public static int displayStyle = 0;
	[GlobalSetting] public static string toggleKeybind = string.Empty;
	[GlobalSetting] public static string inputSpeedKeybind = string.Empty;
	[GlobalSetting] public static float speed = 1f;

	private KeyCode toggleKey;
	private KeyCode inputSpeedKey;
	private static KeyCode suppressToggleKey = KeyCode.None;
	private static KeyCode suppressInputKey = KeyCode.None;

	private bool togglePaused;
	private bool togglePrevOverlayVisible;
	private bool togglePrevDisplayVisible;

	private bool waitingForToggleRebind;
	private KeyCode currentToggleKeyBeforeRebind;
	private static readonly string[] toggleMenuValues = new string[1];

	private bool waitingForInputRebind;
	private KeyCode currentInputKeyBeforeRebind;
	private static readonly string[] inputMenuValues = new string[1];

	private bool waitingForSpeedInput;
	private string speedInputBuffer = string.Empty;
	private bool wasPausedLastTick;

	private bool isLoaded;
	private ILHook[]? coroutineHooks;
	private RebindListener? rebindListener;

	private static HorizontalOption? toggleOption;
	private static HorizontalOption? inputOption;
	private float SpeedMultiplier {
		get => globalSwitch ? speed : 1f;
		set {
			if (value <= 0f) {
				return;
			}

			float normalized = SanitizeSpeed(value);

			if (Time.timeScale != 0f) {
				Time.timeScale = normalized;
			}

			speed = normalized;
		}
	}
	private sealed class RebindListener : MonoBehaviour {
		public Action? Tick;

		private void Update() => Tick?.Invoke();
	}
}

internal sealed class ModDisplay {
	internal static ModDisplay? Instance;

	private string DisplayText = "";
	private Vector2 TextSize = new(800, 500);
	private Vector2 TextPosition = new(0.22f, 0.243f);
	private Vector2 InputTextSize = new(500, 80);
	private Vector2 InputTextPosition = new(0.02f, 0.08f);

	private GameObject? canvas;
	private UnityEngine.UI.Text? text;
	private UnityEngine.UI.Text? inputText;

	public ModDisplay() => Create();

	private void Create() {
		if (canvas != null) {
			return;
		}

		canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));

		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		UObject.DontDestroyOnLoad(canvas);

		text = CanvasUtil.CreateTextPanel(
			canvas, "", 24, TextAnchor.LowerLeft,
			new CanvasUtil.RectData(TextSize, Vector2.zero, TextPosition, TextPosition),
			CanvasUtil.GetFont("Perpetua")
		).GetComponent<UnityEngine.UI.Text>();

		inputText = CanvasUtil.CreateTextPanel(
			canvas, "", 22, TextAnchor.LowerLeft,
			new CanvasUtil.RectData(InputTextSize, new Vector2(30, 30), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)),
			CanvasUtil.GetFont("Perpetua")
		).GetComponent<UnityEngine.UI.Text>();
		inputText.color = new Color(1f, 1f, 1f, 0.9f);
		inputText.gameObject.SetActive(false);
	}

	public void Destroy() {
		if (canvas != null) {
			UObject.Destroy(canvas);
		}

		canvas = null;
		text = null;
		inputText = null;
	}

	public void Update() {
		if (text != null && canvas != null) {
			text.text = DisplayText;
			canvas.SetActive(true);
		}
	}

	public void DisplayInput(string value) {
		if (inputText == null) {
			return;
		}

		inputText.text = value;
		inputText.gameObject.SetActive(true);
	}

	public void HideInput() {
		if (inputText == null) {
			return;
		}

		inputText.gameObject.SetActive(false);
	}

	public void Display(string value) {
		DisplayText = value.Trim();
		Update();
	}
}

internal static class CanvasUtil {
	internal struct RectData {
		internal Vector2 size;
		internal Vector2 position;
		internal Vector2 anchorMin;
		internal Vector2 anchorMax;
		internal Vector2 pivot;

		internal RectData(Vector2 size, Vector2 position, Vector2 anchorMin, Vector2 anchorMax, Vector2? pivot = null) {
			this.size = size;
			this.position = position;
			this.anchorMin = anchorMin;
			this.anchorMax = anchorMax;
			this.pivot = pivot ?? new Vector2(0.5f, 0.5f);
		}
	}

	internal static GameObject CreateCanvas(RenderMode renderMode, Vector2 referenceResolution, string name = "SpeedChangerCanvas", int sortOrder = 9999) {
		GameObject canvasObject = new(name);
		Canvas canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = renderMode;
		canvas.sortingOrder = sortOrder;
		canvas.pixelPerfect = false;

		CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = referenceResolution;
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

		canvasObject.AddComponent<GraphicRaycaster>();
		canvasObject.AddComponent<CanvasGroup>();

		return canvasObject;
	}

	internal static GameObject CreateTextPanel(GameObject parent, string text, int fontSize, TextAnchor anchor, RectData rectData, Font? font = null) {
		GameObject go = new("SpeedChangerText");
		go.transform.SetParent(parent.transform, false);

		UnityEngine.UI.Text txt = go.AddComponent<UnityEngine.UI.Text>();
		txt.text = text;
		txt.fontSize = fontSize;
		txt.alignment = anchor;
		txt.horizontalOverflow = HorizontalWrapMode.Overflow;
		txt.verticalOverflow = VerticalWrapMode.Overflow;
		txt.font = font ?? GetFont("Arial");
		txt.color = Color.white;

		RectTransform rect = go.GetComponent<RectTransform>();
		rect.sizeDelta = rectData.size;
		rect.pivot = rectData.pivot;
		rect.anchorMin = rectData.anchorMin;
		rect.anchorMax = rectData.anchorMax;
		rect.anchoredPosition = rectData.position;

		return go;
	}

	internal static Font GetFont(string fontName) {
		Font? existing = Resources.FindObjectsOfTypeAll<Font>().FirstOrDefault(f => f != null && f.name.Equals(fontName, StringComparison.OrdinalIgnoreCase));
		if (existing != null) {
			return existing;
		}

		try {
			Font? builtin = Resources.GetBuiltinResource<Font>("Arial.ttf");
			if (builtin != null) {
				return builtin;
			}
		} catch (Exception swallowed) {
			LogSuppressed(swallowed, "SpeedChanger.cs");
		}

		return Font.CreateDynamicFontFromOSFont("Arial", 14);
	}
}
