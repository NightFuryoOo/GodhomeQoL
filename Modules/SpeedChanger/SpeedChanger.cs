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
	[GlobalSetting] public static float SpeedChangerTextScale = 1f;

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

	private const float Left = 16f;
	private const float Bottom = 12f;
	private const float Gap = 6f;

	private HudLabel? label;
	private HudLabel? inputLabel;

	public ModDisplay() => Create();

	internal HudLabel? MainLabel => label;

	private void Create() {
		if (label != null) {
			return;
		}

		label = new HudLabel("SpeedChangerCanvas", Vector2.zero, new Vector2(Left, Bottom), 24, () => HudTextScales.SpeedText);
		inputLabel = new HudLabel("SpeedChangerInputCanvas", Vector2.zero, new Vector2(Left, Bottom), 22, () => HudTextScales.SpeedText);
		inputLabel.AddUpdater(PlaceInput);
	}

	private void PlaceInput() {
		if (label != null && inputLabel != null) {
			inputLabel.Offset = new Vector2(Left, label.IsShown ? Bottom + label.ScaledHeight + Gap : Bottom);
		}
	}

	public void Destroy() {
		label?.Destroy();
		inputLabel?.Destroy();
		label = null;
		inputLabel = null;
	}

	public void DisplayInput(string value) {
		inputLabel?.Show(value);
		PlaceInput();
	}

	public void HideInput() {
		inputLabel?.Hide();
	}

	public void Display(string value) {
		label?.Show(value);
	}

	internal void ApplyScale() {
		label?.ApplyScale();
		inputLabel?.ApplyScale();
	}

	internal void Preview(string value) {
		label?.Preview(value);
	}
}

internal static class CanvasUtil {
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
}
