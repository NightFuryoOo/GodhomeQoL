namespace Osmi.Utils
{
    public static class StringCompatExtensions
    {
        public static string? StripStart(this string? value, string? prefix)
        {
            if (value == null || string.IsNullOrEmpty(prefix))
            {
                return value;
            }

            string normalizedPrefix = prefix!;
            return value.StartsWith(normalizedPrefix, StringComparison.Ordinal)
                ? value.Substring(normalizedPrefix.Length)
                : value;
        }

        public static string? StripEnd(this string? value, string? suffix)
        {
            if (value == null || string.IsNullOrEmpty(suffix))
            {
                return value;
            }

            string normalizedSuffix = suffix!;
            return value.EndsWith(normalizedSuffix, StringComparison.Ordinal)
                ? value.Substring(0, value.Length - normalizedSuffix.Length)
                : value;
        }
    }
}

namespace Osmi
{
    public enum Charm
    {
        FuryOfTheFallen = 6
    }

    public static class CharmUtil
    {
        public static bool EquippedCharm(Charm charm)
        {
            PlayerData? playerData = PlayerData.instance;
            return playerData != null && playerData.GetBool($"equippedCharm_{(int)charm}");
        }
    }

    public static class EnemyDetector
    {
        public static bool IsValidEnemy(HealthManager? healthManager)
        {
            if (healthManager == null || healthManager.gameObject == null)
            {
                return false;
            }

            GameObject gameObject = healthManager.gameObject;
            if (!gameObject.activeInHierarchy)
            {
                return false;
            }

            if (HeroController.instance != null && ReferenceEquals(gameObject, HeroController.instance.gameObject))
            {
                return false;
            }

            return healthManager.hp > 0;
        }
    }
}

namespace Vasi
{
    public static class VasiSceneExtensions
    {
        public static GameObject? GetGameObjectByName(this Scene scene, string objectName)
        {
            if (!scene.IsValid() || !scene.isLoaded || string.IsNullOrEmpty(objectName))
            {
                return null;
            }

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform? found = FindByName(root.transform, objectName);
                if (found != null)
                {
                    return found.gameObject;
                }
            }

            return null;
        }

        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return Enumerable.Empty<GameObject>();
            }

            List<GameObject> children = [];
            Transform transform = gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                children.Add(transform.GetChild(i).gameObject);
            }

            return children;
        }

        public static IEnumerable<GameObject> GetChildren(this Component component) =>
            component == null ? Enumerable.Empty<GameObject>() : component.gameObject.GetChildren();

        private static Transform? FindByName(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (string.Equals(root.name, objectName, StringComparison.Ordinal))
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                Transform? found = FindByName(child, objectName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}

namespace Satchel
{
    public sealed class TransitionInterceptor
    {
        public string fromState = string.Empty;
        public string eventName = string.Empty;
        public string toStateDefault = string.Empty;
        public string toStateCustom = string.Empty;
        public Func<bool>? shouldIntercept;
        public Action<PlayMakerFSM, FsmState>? onIntercept;
    }

    public static class PersistentBoolDataExtensions
    {
        public static void Set(this List<PersistentBoolData> items, string sceneName, string id, bool activated, bool semiPersistent = false)
        {
            if (items == null || string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(id))
            {
                return;
            }

            PersistentBoolData? existing = items.FirstOrDefault(item =>
                item != null
                && string.Equals(item.sceneName, sceneName, StringComparison.Ordinal)
                && string.Equals(item.id, id, StringComparison.Ordinal));

            if (existing != null)
            {
                existing.activated = activated;
                existing.semiPersistent = semiPersistent;
                return;
            }

            items.Add(new PersistentBoolData
            {
                sceneName = sceneName,
                id = id,
                activated = activated,
                semiPersistent = semiPersistent
            });
        }

        public static bool IsActivated(this List<PersistentBoolData> items, string sceneName, string id)
        {
            if (items == null || string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(id))
            {
                return false;
            }

            PersistentBoolData? existing = items.FirstOrDefault(item =>
                item != null
                && string.Equals(item.sceneName, sceneName, StringComparison.Ordinal)
                && string.Equals(item.id, id, StringComparison.Ordinal));

            return existing != null && existing.activated;
        }
    }
}

namespace Satchel.Futils
{
    public static class FsmExtraCompatExtensions
    {
        public static FsmState? GetState(this PlayMakerFSM fsm, string stateName) =>
            fsm?.Fsm?.GetState(stateName);

        public static FsmState GetValidState(this PlayMakerFSM fsm, string stateName)
        {
            FsmState? state = fsm?.Fsm?.GetState(stateName);
            if (state == null)
            {
                throw new InvalidOperationException($"State '{stateName}' is missing on FSM '{fsm?.FsmName}'.");
            }

            return state;
        }

        public static void AddMethod(this FsmState state, Action method)
        {
            if (state == null || method == null)
            {
                return;
            }

            List<FsmStateAction> actions = state.Actions?.ToList() ?? [];
            actions.Add(new InlineAction(method));
            state.Actions = actions.ToArray();
        }

        public static void Intercept(this PlayMakerFSM fsm, Satchel.TransitionInterceptor interceptor)
        {
            if (fsm == null
                || interceptor == null
                || string.IsNullOrEmpty(interceptor.fromState)
                || string.IsNullOrEmpty(interceptor.eventName))
            {
                return;
            }

            FsmState? fromState = fsm.Fsm.GetState(interceptor.fromState);
            if (fromState == null)
            {
                return;
            }

            fsm.InsertCustomAction(interceptor.fromState, () =>
            {
                bool useCustom = false;
                try
                {
                    useCustom = interceptor.shouldIntercept?.Invoke() == true;
                }
                catch
                {
                    useCustom = false;
                }

                string target = useCustom ? interceptor.toStateCustom : interceptor.toStateDefault;
                if (!string.IsNullOrEmpty(target))
                {
                    fromState.ChangeTransition(interceptor.eventName, target);
                }

                if (useCustom)
                {
                    interceptor.onIntercept?.Invoke(fsm, fromState);
                }
            }, 0);
        }

        private sealed class InlineAction : FsmStateAction
        {
            private readonly Action action;

            public InlineAction(Action action)
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
}

namespace Satchel.BetterMenus
{
    public static class ModToggleCompatExtensions
    {
        public static Element CreateToggle(this ModToggleDelegates toggleDelegates, string name, string description)
        {
            return Blueprints.HorizontalBoolOption(
                name,
                description,
                enabled =>
                {
                    toggleDelegates.SetModEnabled?.Invoke(enabled);
                },
                () => toggleDelegates.GetModEnabled?.Invoke() ?? false
            );
        }
    }
}
