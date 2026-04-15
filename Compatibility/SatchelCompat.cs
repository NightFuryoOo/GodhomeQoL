using InControl;
using Modding.Menu;

namespace Satchel
{
    public sealed class GameObjectRef
    {
        public const int ANYWHERE = 0;
        public const int DONT_DESTROY_ON_LOAD = 1;

        private readonly int scope;
        private readonly string objectName;

        public GameObjectRef(int scope, string objectName)
        {
            this.scope = scope;
            this.objectName = objectName ?? string.Empty;
        }

        public bool MatchGameObject(GameObject? gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }

            if (!string.Equals(gameObject.name, objectName, StringComparison.Ordinal))
            {
                return false;
            }

            return scope != DONT_DESTROY_ON_LOAD
                || string.Equals(gameObject.scene.name, "DontDestroyOnLoad", StringComparison.Ordinal);
        }
    }

    public readonly struct SceneObjectRef
    {
        public string SceneName { get; }
        public IReadOnlyList<string> Path { get; }

        public SceneObjectRef(string sceneName, params string[] path)
        {
            SceneName = sceneName ?? string.Empty;
            Path = path ?? Array.Empty<string>();
        }
    }

    public sealed class SceneEdit
    {
        private readonly SceneObjectRef target;
        private readonly Action<GameObject> editAction;
        private readonly HashSet<int> patchedIds = [];
        private bool enabled;

        public SceneEdit(SceneObjectRef target, Action<GameObject> editAction)
        {
            this.target = target;
            this.editAction = editAction ?? (_ => { });
        }

        public void Enable()
        {
            if (enabled)
            {
                return;
            }

            enabled = true;
            patchedIds.Clear();
            USceneManager.activeSceneChanged += OnSceneChanged;
            On.PlayMakerFSM.OnEnable += OnFsmEnable;
            TryApplyToActiveScene();
        }

        public void Disable()
        {
            if (!enabled)
            {
                return;
            }

            enabled = false;
            USceneManager.activeSceneChanged -= OnSceneChanged;
            On.PlayMakerFSM.OnEnable -= OnFsmEnable;
            patchedIds.Clear();
        }

        private void OnSceneChanged(Scene from, Scene to)
        {
            patchedIds.Clear();
            TryApplyToScene(to);
        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            TryApplyToScene(self.gameObject.scene);
        }

        private void TryApplyToActiveScene()
        {
            Scene scene = USceneManager.GetActiveScene();
            TryApplyToScene(scene);
        }

        private void TryApplyToScene(Scene scene)
        {
            if (!enabled
                || !scene.IsValid()
                || !scene.isLoaded
                || !string.Equals(scene.name, target.SceneName, StringComparison.Ordinal)
                || target.Path.Count == 0)
            {
                return;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            GameObject? root = null;
            foreach (GameObject candidate in roots)
            {
                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(candidate.name, target.Path[0], StringComparison.Ordinal))
                {
                    root = candidate;
                    break;
                }

                Transform? deep = FindDescendant(candidate.transform, target.Path[0]);
                if (deep != null)
                {
                    root = deep.gameObject;
                    break;
                }
            }

            if (root == null)
            {
                return;
            }

            Transform? current = root.transform;
            for (int i = 1; i < target.Path.Count; i++)
            {
                current = current?.Find(target.Path[i]);
                if (current == null)
                {
                    return;
                }
            }

            GameObject targetObject = current.gameObject;
            int id = targetObject.GetInstanceID();
            if (!patchedIds.Add(id))
            {
                return;
            }

            try
            {
                editAction(targetObject);
            }
            catch (Exception ex)
            {
                Logger.LogError($"SceneEdit failed for {target.SceneName}: {ex}");
            }
        }

        private static Transform? FindDescendant(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (string.Equals(root.name, name, StringComparison.Ordinal))
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                Transform? found = FindDescendant(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }

    public static class GlobalCoroutineExecutor
    {
        private static CoroutineRunner? runner;

        public static Coroutine? Start(IEnumerator coroutine)
        {
            if (coroutine == null)
            {
                return null;
            }

            EnsureRunner();
            return runner?.StartCoroutine(coroutine);
        }

        private static void EnsureRunner()
        {
            if (runner != null)
            {
                return;
            }

            GameObject go = new("GodhomeQoL_GlobalCoroutineExecutor");
            UObject.DontDestroyOnLoad(go);
            runner = go.AddComponent<CoroutineRunner>();
        }

        private sealed class CoroutineRunner : MonoBehaviour
        {
        }
    }
}

namespace Satchel.Futils
{
    public static class FsmCompatExtensions
    {
        public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action)
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            if (state == null || action == null)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions?.ToList() ?? new List<FsmStateAction>();
            actions.Add(action);
            state.Actions = actions.ToArray();
        }

        public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index)
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            if (state == null || action == null)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions?.ToList() ?? new List<FsmStateAction>();
            index = Mathf.Clamp(index, 0, actions.Count);
            actions.Insert(index, action);
            state.Actions = actions.ToArray();
        }

        public static void RemoveAction(this PlayMakerFSM fsm, string stateName, int index)
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            state?.RemoveAction(index);
        }

        public static void AddCustomAction(this PlayMakerFSM fsm, string stateName, Action action) =>
            fsm.AddAction(stateName, new LambdaAction(action));

        public static void InsertCustomAction(this PlayMakerFSM fsm, string stateName, Action action, int index) =>
            fsm.InsertAction(stateName, new LambdaAction(action), index);

        public static TAction? GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index = 0)
            where TAction : FsmStateAction
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            if (state?.Actions == null || index < 0)
            {
                return null;
            }

            return state.Actions
                .OfType<TAction>()
                .ElementAtOrDefault(index);
        }

        public static TVariable? GetVariable<TVariable>(this PlayMakerFSM fsm, string variableName)
            where TVariable : NamedVariable
        {
            if (fsm == null || string.IsNullOrEmpty(variableName))
            {
                return null;
            }

            FsmVariables vars = fsm.FsmVariables;
            object? variable = typeof(TVariable) switch
            {
                var t when t == typeof(FsmBool) => vars.FindFsmBool(variableName),
                var t when t == typeof(FsmInt) => vars.FindFsmInt(variableName),
                var t when t == typeof(FsmFloat) => vars.FindFsmFloat(variableName),
                var t when t == typeof(FsmString) => vars.FindFsmString(variableName),
                var t when t == typeof(FsmArray) => vars.FindFsmArray(variableName),
                var t when t == typeof(FsmGameObject) => vars.FindFsmGameObject(variableName),
                var t when t == typeof(FsmVector2) => vars.FindFsmVector2(variableName),
                var t when t == typeof(FsmVector3) => vars.FindFsmVector3(variableName),
                _ => null
            };

            return variable as TVariable;
        }

        public static void ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState)
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            state?.ChangeTransition(eventName, toState);
        }

        public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName)
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            state?.RemoveTransition(eventName);
        }

        public static void AddCustomAction(this FsmState state, Action action)
        {
            if (state == null)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions?.ToList() ?? new List<FsmStateAction>();
            actions.Add(new LambdaAction(action));
            state.Actions = actions.ToArray();
        }

        public static void InsertCustomAction(this FsmState state, Action action, int index)
        {
            if (state == null)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions?.ToList() ?? new List<FsmStateAction>();
            index = Mathf.Clamp(index, 0, actions.Count);
            actions.Insert(index, new LambdaAction(action));
            state.Actions = actions.ToArray();
        }

        public static void RemoveAction(this FsmState state, int index)
        {
            if (state?.Actions == null || index < 0 || index >= state.Actions.Length)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions.ToList();
            actions.RemoveAt(index);
            state.Actions = actions.ToArray();
        }

        public static void RemoveAction<TAction>(this FsmState state) where TAction : FsmStateAction
        {
            if (state?.Actions == null)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions.Where(action => action is not TAction).ToList();
            state.Actions = actions.ToArray();
        }

        public static void ChangeTransition(this FsmState state, string eventName, string toState)
        {
            if (state == null || state.Transitions == null)
            {
                return;
            }

            foreach (FsmTransition transition in state.Transitions)
            {
                if (!string.Equals(transition.EventName, eventName, StringComparison.Ordinal))
                {
                    continue;
                }

                transition.ToState = toState;
                transition.ToFsmState = state.Fsm.GetState(toState);
            }
        }

        public static void RemoveTransition(this FsmState state, string eventName)
        {
            if (state?.Transitions == null)
            {
                return;
            }

            state.Transitions = state.Transitions
                .Where(transition => !string.Equals(transition.EventName, eventName, StringComparison.Ordinal))
                .ToArray();
        }

        private sealed class LambdaAction : FsmStateAction
        {
            private readonly Action action;

            public LambdaAction(Action action)
            {
                this.action = action ?? (() => { });
            }

            public override void OnEnter()
            {
                action.Invoke();
                Finish();
            }
        }
    }

    public sealed class InvokePredicate : FsmStateAction
    {
        private readonly Func<bool> predicate;

        public FsmEvent? trueEvent;
        public FsmEvent? falseEvent;

        public InvokePredicate(Func<bool> predicate)
        {
            this.predicate = predicate ?? (() => false);
        }

        public override void OnEnter()
        {
            bool result = predicate.Invoke();
            FsmEvent? target = result ? trueEvent : falseEvent;
            if (target != null)
            {
                Fsm?.Event(target.Name);
            }

            Finish();
        }
    }

    public sealed class InvokeCoroutine : FsmStateAction
    {
        private readonly Func<IEnumerator> coroutineFactory;

        public InvokeCoroutine(Func<IEnumerator> coroutineFactory)
        {
            this.coroutineFactory = coroutineFactory ?? (() => Empty());
        }

        public override void OnEnter()
        {
            IEnumerator coroutine = coroutineFactory.Invoke();
            if (coroutine != null)
            {
                if (HeroController.instance != null)
                {
                    HeroController.instance.StartCoroutine(coroutine);
                }
                else if (GameManager.instance != null)
                {
                    GameManager.instance.StartCoroutine(coroutine);
                }
                else
                {
                    _ = Satchel.GlobalCoroutineExecutor.Start(coroutine);
                }
            }

            Finish();
        }

        private static IEnumerator Empty()
        {
            yield break;
        }
    }
}

namespace Satchel.BetterMenus
{
    public abstract class Element
    {
        public bool isVisible = true;

        public event Action<Element, EventArgs>? OnUpdate;

        internal abstract IMenuMod.MenuEntry BuildEntry(MenuScreen parent);

        public virtual void Show()
        {
            isVisible = true;
            Update();
        }

        public virtual void Hide()
        {
            isVisible = false;
            Update();
        }

        public virtual void Update() => OnUpdate?.Invoke(this, EventArgs.Empty);
    }

    public sealed class Menu
    {
        private readonly string title;
        private readonly List<Element> elements = [];

        public MenuScreen menuScreen = null!;

        public Menu(string title, IEnumerable<Element> elements)
        {
            this.title = title ?? string.Empty;
            if (elements != null)
            {
                this.elements.AddRange(elements);
            }
        }

        public Menu(string title, params Element[] elements)
            : this(title, (IEnumerable<Element>)elements)
        {
        }

        public void AddElement(Element element)
        {
            if (element != null)
            {
                elements.Add(element);
            }
        }

        public MenuScreen GetMenuScreen(MenuScreen parent)
        {
            List<IMenuMod.MenuEntry> entries = [];
            foreach (Element element in elements)
            {
                element.Update();
                if (!element.isVisible)
                {
                    continue;
                }

                entries.Add(element.BuildEntry(parent));
            }

            menuScreen = MenuUtils.CreateMenuScreen(title, entries, parent);
            return menuScreen;
        }
    }

    public sealed class MenuButton : Element
    {
        private readonly Action<object?> submitAction;
        private readonly bool proceed;
        private readonly string description;

        public string Name { get; set; }

        public MenuButton(string name, string description, Action<object?> submitAction, bool proceed)
        {
            Name = name ?? string.Empty;
            this.description = description ?? string.Empty;
            this.submitAction = submitAction ?? (_ => { });
            this.proceed = proceed;
        }

        internal override IMenuMod.MenuEntry BuildEntry(MenuScreen parent)
        {
            return new IMenuMod.MenuEntry(
                Name,
                new[] { string.Empty },
                description,
                _ => submitAction(null),
                () => 0
            );
        }
    }

    public class HorizontalOption : Element
    {
        private readonly string description;
        private readonly Action<int> saver;
        private readonly Func<int> loader;

        public GameObject? gameObject;
        public string Name { get; set; }
        public string[] Values { get; set; }

        public HorizontalOption(string name, string description, string[] values, Action<int> saver, Func<int> loader)
        {
            Name = name ?? string.Empty;
            this.description = description ?? string.Empty;
            Values = values ?? Array.Empty<string>();
            this.saver = saver ?? (_ => { });
            this.loader = loader ?? (() => 0);
        }

        internal override IMenuMod.MenuEntry BuildEntry(MenuScreen parent)
        {
            string[] values = Values.Length == 0 ? new[] { string.Empty } : Values;
            return new IMenuMod.MenuEntry(
                Name,
                values,
                description,
                idx =>
                {
                    int safe = Mathf.Clamp(idx, 0, values.Length - 1);
                    saver(safe);
                    Update();
                },
                () => Mathf.Clamp(loader(), 0, values.Length - 1)
            );
        }
    }

    public sealed class CustomSlider : Element
    {
        private readonly string name;
        private readonly Action<float> saver;
        private readonly Func<float> loader;
        private readonly float min;
        private readonly float max;
        private readonly bool wholeNumbers;

        public CustomSlider(string name, Action<float> saver, Func<float> loader, float min, float max, bool wholeNumbers)
        {
            this.name = name ?? string.Empty;
            this.saver = saver ?? (_ => { });
            this.loader = loader ?? (() => min);
            this.min = min;
            this.max = Mathf.Max(min, max);
            this.wholeNumbers = wholeNumbers;
        }

        internal override IMenuMod.MenuEntry BuildEntry(MenuScreen parent)
        {
            if (wholeNumbers)
            {
                int minInt = Mathf.RoundToInt(min);
                int maxInt = Mathf.RoundToInt(max);
                if (maxInt < minInt)
                {
                    maxInt = minInt;
                }

                string[] values = Enumerable.Range(minInt, maxInt - minInt + 1)
                    .Select(v => v.ToString())
                    .ToArray();

                return new IMenuMod.MenuEntry(
                    name,
                    values,
                    string.Empty,
                    idx =>
                    {
                        int value = minInt + Mathf.Clamp(idx, 0, values.Length - 1);
                        saver(value);
                        Update();
                    },
                    () => Mathf.Clamp(Mathf.RoundToInt(loader()) - minInt, 0, values.Length - 1)
                );
            }

            const int steps = 100;
            float span = Mathf.Max(0.0001f, max - min);
            string[] options = Enumerable.Range(0, steps + 1)
                .Select(i =>
                {
                    float value = min + (span * i / steps);
                    return value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
                })
                .ToArray();

            return new IMenuMod.MenuEntry(
                name,
                options,
                string.Empty,
                idx =>
                {
                    int safe = Mathf.Clamp(idx, 0, steps);
                    float value = min + (span * safe / steps);
                    saver(value);
                    Update();
                },
                () =>
                {
                    float current = Mathf.Clamp(loader(), min, max);
                    return Mathf.Clamp(Mathf.RoundToInt((current - min) / span * steps), 0, steps);
                }
            );
        }
    }

    public sealed class KeyBind : Element
    {
        private readonly string name;
        private readonly PlayerAction action;
        private readonly string id;
        private static readonly Key[] KeyOptions = Enum.GetValues(typeof(Key)).Cast<Key>().ToArray();

        public KeyBind(string name, PlayerAction action, string id)
        {
            this.name = name ?? string.Empty;
            this.action = action;
            this.id = id ?? string.Empty;
        }

        internal override IMenuMod.MenuEntry BuildEntry(MenuScreen parent)
        {
            string[] values = new string[KeyOptions.Length + 1];
            values[0] = "Not Set";
            for (int i = 0; i < KeyOptions.Length; i++)
            {
                values[i + 1] = KeyOptions[i].ToString();
            }

            return new IMenuMod.MenuEntry(
                name,
                values,
                id,
                idx =>
                {
                    if (action == null)
                    {
                        return;
                    }

                    action.ClearBindings();
                    if (idx > 0 && idx - 1 < KeyOptions.Length)
                    {
                        action.AddBinding(new KeyBindingSource(KeyOptions[idx - 1]));
                    }

                    Update();
                },
                () =>
                {
                    if (action == null || action.UnfilteredBindings.Count == 0)
                    {
                        return 0;
                    }

                    BindingSource source = action.UnfilteredBindings[0];
                    if (source is KeyBindingSource keySource)
                    {
                        int found = Array.IndexOf(KeyOptions, keySource.Control);
                        if (found >= 0)
                        {
                            return found + 1;
                        }
                    }

                    return 0;
                }
            );
        }
    }

    public static class Blueprints
    {
        public static Element HorizontalBoolOption(string name, string description, Action<bool> setter, Func<bool> getter)
        {
            return new HorizontalOption(
                name,
                description,
                new[] { "OFF", "ON" },
                idx => setter(idx == 1),
                () => getter() ? 1 : 0
            );
        }

        public static Element IntInputField(
            string name,
            Action<int> setter,
            Func<int> getter,
            int defaultValue = 0,
            string suffix = "",
            int charLimit = 3)
        {
            return new IntStepperOption(name, setter, getter, defaultValue, suffix, charLimit);
        }

        public static Element NavigateToMenu(string name, string description, Func<MenuScreen> menuBuilder)
        {
            return new MenuButton(
                name,
                description,
                _ =>
                {
                    MenuScreen next = menuBuilder();
                    if (next != null)
                    {
                        UIManager.instance?.UIGoToDynamicMenu(next);
                    }
                },
                proceed: true
            );
        }

        public static Element GenericHorizontalOption<T>(
            string name,
            string description,
            IEnumerable<T> options,
            Action<T> setter,
            Func<T> getter)
        {
            return new GenericHorizontalOptionElement<T>(name, description, options, setter, getter);
        }

        private sealed class GenericHorizontalOptionElement<T> : Element
        {
            private readonly string name;
            private readonly string description;
            private readonly T[] options;
            private readonly Action<T> setter;
            private readonly Func<T> getter;

            public GenericHorizontalOptionElement(
                string name,
                string description,
                IEnumerable<T> options,
                Action<T> setter,
                Func<T> getter)
            {
                this.name = name ?? string.Empty;
                this.description = description ?? string.Empty;
                this.options = options?.ToArray() ?? Array.Empty<T>();
                this.setter = setter ?? (_ => { });
                this.getter = getter ?? (() => default!);
            }

            internal override IMenuMod.MenuEntry BuildEntry(MenuScreen parent)
            {
                string[] values = options.Length == 0
                    ? new[] { string.Empty }
                    : options.Select(option => option?.ToString() ?? string.Empty).ToArray();

                return new IMenuMod.MenuEntry(
                    name,
                    values,
                    description,
                    idx =>
                    {
                        if (options.Length == 0)
                        {
                            return;
                        }

                        int safe = Mathf.Clamp(idx, 0, options.Length - 1);
                        setter(options[safe]);
                        Update();
                    },
                    () =>
                    {
                        if (options.Length == 0)
                        {
                            return 0;
                        }

                        T current = getter();
                        int index = Array.FindIndex(options, option => Equals(option, current));
                        return index < 0 ? 0 : index;
                    }
                );
            }
        }

        private sealed class IntStepperOption : Element
        {
            private readonly string name;
            private readonly Action<int> setter;
            private readonly Func<int> getter;
            private readonly int defaultValue;
            private readonly string suffix;
            private readonly int[] deltas;
            private readonly string[] optionTexts;
            private readonly int centerIndex;

            public IntStepperOption(
                string name,
                Action<int> setter,
                Func<int> getter,
                int defaultValue,
                string suffix,
                int charLimit)
            {
                this.name = name ?? string.Empty;
                this.setter = setter ?? (_ => { });
                this.getter = getter ?? (() => defaultValue);
                this.defaultValue = defaultValue;
                this.suffix = suffix ?? string.Empty;

                int limit = Mathf.Clamp(charLimit, 1, 9);
                List<int> scales = [];
                int power = 1;
                for (int i = 1; i < limit; i++)
                {
                    power *= 10;
                }

                for (int i = 0; i < limit; i++)
                {
                    scales.Add(power);
                    power = Math.Max(1, power / 10);
                }

                scales = scales.Distinct().ToList();

                List<int> builtDeltas = [];
                foreach (int scale in scales)
                {
                    builtDeltas.Add(-scale);
                }

                builtDeltas.Add(0);
                foreach (int scale in scales.AsEnumerable().Reverse())
                {
                    builtDeltas.Add(scale);
                }

                deltas = builtDeltas.ToArray();
                optionTexts = deltas
                    .Select(delta => delta switch
                    {
                        > 0 => $"+{delta}",
                        < 0 => delta.ToString(),
                        _ => "0"
                    })
                    .ToArray();
                centerIndex = Array.IndexOf(deltas, 0);
            }

            internal override IMenuMod.MenuEntry BuildEntry(MenuScreen parent)
            {
                string currentSuffix = string.IsNullOrWhiteSpace(suffix) ? string.Empty : $" {suffix}";
                string displayName = $"{name}: {getter()}{currentSuffix}";

                return new IMenuMod.MenuEntry(
                    displayName,
                    optionTexts,
                    string.Empty,
                    idx =>
                    {
                        int safe = Mathf.Clamp(idx, 0, deltas.Length - 1);
                        int delta = deltas[safe];
                        if (delta == 0)
                        {
                            return;
                        }

                        int current = getter();
                        long next = (long)current + delta;
                        if (next > int.MaxValue)
                        {
                            next = int.MaxValue;
                        }
                        else if (next < int.MinValue)
                        {
                            next = int.MinValue;
                        }

                        setter((int)next);
                        Update();
                    },
                    () => centerIndex >= 0 ? centerIndex : 0
                );
            }
        }
    }
}

namespace Satchel.BetterMenus.Config
{
    internal static class Placeholder
    {
    }
}
