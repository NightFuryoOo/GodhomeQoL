using UnityEngine.UI;

namespace GodhomeQoL.Modules.QoL;

public sealed class BlackTransitions : Module
{
    private const string TransitionsName = "gg_battle_transitions";
    private const string TransitionsFsm = "Transitions";
    private const string HudBlankerName = "Blanker White";
    private const string HudBlankerFsm = "Blanker Control";
    private const string HudBlankerVariable = "HUD Blanker White";
    private const string DreamFlashName = "White Flash";
    private const string DreamEnterPrefix = "Dream Enter";
    private const string SceneBlankerName = "Scene Blanker";
    private const string EndWhiteSolidName = "White Solid";

    internal static bool Darkening { get; private set; }

    private protected override void Load()
    {
        Darkening = true;
        On.PlayMakerFSM.OnEnable += OnFsmEnable;
        USceneManager.activeSceneChanged += OnSceneChanged;
        AttachHudBlanker();
        AttachExistingTransitions();
        AttachSceneObjects(USceneManager.GetActiveScene());
    }

    private protected override void Unload()
    {
        Darkening = false;
        On.PlayMakerFSM.OnEnable -= OnFsmEnable;
        USceneManager.activeSceneChanged -= OnSceneChanged;
        BlackTransitionTint.RestoreAll();
    }

    private static void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
    {
        orig(self);
        try
        {
            if (self == null)
            {
                return;
            }

            GameObject go = self.gameObject;
            if (IsTransitions(self))
            {
                BlackTransitionTint.Attach(go, hideParticles: true);
            }
            else if (IsHudBlanker(self) || IsDreamFlash(go) || go.name == SceneBlankerName)
            {
                BlackTransitionTint.Attach(go, hideParticles: false);
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "BlackTransitions.cs/FsmEnable");
        }
    }

    private static void OnSceneChanged(Scene current, Scene next)
    {
        AttachHudBlanker();
        AttachSceneObjects(next);
    }

    private static bool IsTransitions(PlayMakerFSM fsm) =>
        fsm.FsmName == TransitionsFsm && fsm.gameObject.name.StartsWith(TransitionsName, StringComparison.Ordinal);

    private static bool IsHudBlanker(PlayMakerFSM fsm) =>
        fsm.FsmName == HudBlankerFsm && fsm.gameObject.name == HudBlankerName;

    private static bool IsDreamFlash(GameObject go)
    {
        Transform? parent = go.transform.parent;
        return go.name == DreamFlashName
            && parent != null
            && parent.name.StartsWith(DreamEnterPrefix, StringComparison.Ordinal);
    }

    private static void AttachHudBlanker()
    {
        try
        {
            GameObject? blanker = FsmVariables.GlobalVariables.FindFsmGameObject(HudBlankerVariable)?.Value;
            if (blanker != null)
            {
                BlackTransitionTint.Attach(blanker, hideParticles: false);
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "BlackTransitions.cs/HudBlanker");
        }
    }

    private static void AttachExistingTransitions()
    {
        try
        {
            foreach (PlayMakerFSM fsm in UObject.FindObjectsOfType<PlayMakerFSM>())
            {
                if (fsm != null && IsTransitions(fsm))
                {
                    BlackTransitionTint.Attach(fsm.gameObject, hideParticles: true);
                }
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "BlackTransitions.cs/Existing");
        }
    }

    private static void AttachSceneObjects(Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        try
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                {
                    GameObject go = child.gameObject;
                    if (go.name == EndWhiteSolidName && go.GetComponent<Image>() != null)
                    {
                        BlackTransitionTint.Attach(go, hideParticles: false);
                    }
                    else if (IsDreamFlash(go) || go.name == SceneBlankerName)
                    {
                        BlackTransitionTint.Attach(go, hideParticles: false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogSuppressed(ex, "BlackTransitions.cs/Scene");
        }
    }
}

internal sealed class BlackTransitionTint : MonoBehaviour
{
    private static readonly List<BlackTransitionTint> Instances = new();

    private SpriteRenderer[] sprites = Array.Empty<SpriteRenderer>();
    private Color[] spriteColors = Array.Empty<Color>();
    private Graphic[] graphics = Array.Empty<Graphic>();
    private Color[] graphicColors = Array.Empty<Color>();
    private ParticleSystemRenderer[] particles = Array.Empty<ParticleSystemRenderer>();
    private bool[] particleStates = Array.Empty<bool>();
    private bool collected;
    private bool tinted;

    internal static void Attach(GameObject go, bool hideParticles)
    {
        if (go == null)
        {
            return;
        }

        BlackTransitionTint tint = go.GetComponent<BlackTransitionTint>();
        if (tint == null)
        {
            tint = go.AddComponent<BlackTransitionTint>();
        }

        if (!tint.collected)
        {
            tint.Collect(hideParticles);
        }

        tint.Apply();
    }

    internal static void RestoreAll()
    {
        foreach (BlackTransitionTint tint in Instances.ToArray())
        {
            if (tint != null)
            {
                tint.Restore();
            }
        }
    }

    private void Awake() => Instances.Add(this);

    private void OnDestroy() => Instances.Remove(this);

    private void OnEnable() => Apply();

    private void LateUpdate() => Apply();

    private void Collect(bool hideParticles)
    {
        collected = true;
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        spriteColors = sprites.Select(sprite => sprite.color).ToArray();
        graphics = GetComponentsInChildren<Graphic>(true);
        graphicColors = graphics.Select(graphic => graphic.color).ToArray();
        if (hideParticles)
        {
            particles = GetComponentsInChildren<ParticleSystemRenderer>(true);
            particleStates = particles.Select(particle => particle.enabled).ToArray();
        }
    }

    private void Apply()
    {
        if (!collected)
        {
            return;
        }

        if (!BlackTransitions.Darkening)
        {
            if (tinted)
            {
                Restore();
            }

            return;
        }

        tinted = true;
        foreach (SpriteRenderer sprite in sprites)
        {
            if (sprite != null)
            {
                sprite.color = Darken(sprite.color);
            }
        }

        foreach (Graphic graphic in graphics)
        {
            if (graphic != null)
            {
                graphic.color = Darken(graphic.color);
            }
        }

        foreach (ParticleSystemRenderer particle in particles)
        {
            if (particle != null && particle.enabled)
            {
                particle.enabled = false;
            }
        }
    }

    private void Restore()
    {
        if (!tinted)
        {
            return;
        }

        tinted = false;
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                sprites[i].color = WithAlpha(spriteColors[i], sprites[i].color.a);
            }
        }

        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] != null)
            {
                graphics[i].color = WithAlpha(graphicColors[i], graphics[i].color.a);
            }
        }

        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i] != null)
            {
                particles[i].enabled = particleStates[i];
            }
        }
    }

    private static Color Darken(Color color) =>
        color.r == 0f && color.g == 0f && color.b == 0f ? color : new Color(0f, 0f, 0f, color.a);

    private static Color WithAlpha(Color color, float alpha) => new(color.r, color.g, color.b, alpha);
}
