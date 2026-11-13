namespace GodhomeQoL.Modules.Tools;

internal sealed class FreezeHitboxesRender : MonoBehaviour
{
    private const int CleanupIntervalFrames = 30;

    private struct HitboxType : IComparable<HitboxType>
    {
        public static readonly HitboxType KnightHurtbox = new(Color.white, -1);
        public static readonly HitboxType Knight = new(Color.yellow, 0);
        public static readonly HitboxType Enemy = new(new Color(0.8f, 0f, 0f), 1);
        public static readonly HitboxType Attack = new(Color.cyan, 2);
        public static readonly HitboxType Terrain = new(new Color(0f, 0.8f, 0f), 3);
        public static readonly HitboxType Trigger = new(new Color(0.5f, 0.5f, 1f), 4);
        public static readonly HitboxType Breakable = new(new Color(1f, 0.75f, 0.8f), 5);
        public static readonly HitboxType Gate = new(new Color(0f, 0f, 0.5f), 6);
        public static readonly HitboxType HazardRespawn = new(new Color(0.5f, 0f, 0.5f), 7);
        public static readonly HitboxType Other = new(new Color(0.9f, 0.6f, 0.4f), 8);

        public readonly Color Color;
        public readonly int Depth;

        private HitboxType(Color color, int depth)
        {
            Color = color;
            Depth = depth;
        }

        public int CompareTo(HitboxType other)
        {
            return other.Depth.CompareTo(Depth);
        }
    }

    private readonly SortedDictionary<HitboxType, HashSet<Collider2D>> colliders = new()
    {
        { HitboxType.KnightHurtbox, new HashSet<Collider2D>() },
        { HitboxType.Knight, new HashSet<Collider2D>() },
        { HitboxType.Enemy, new HashSet<Collider2D>() },
        { HitboxType.Attack, new HashSet<Collider2D>() },
        { HitboxType.Terrain, new HashSet<Collider2D>() },
        { HitboxType.Trigger, new HashSet<Collider2D>() },
        { HitboxType.Breakable, new HashSet<Collider2D>() },
        { HitboxType.Gate, new HashSet<Collider2D>() },
        { HitboxType.HazardRespawn, new HashSet<Collider2D>() },
        { HitboxType.Other, new HashSet<Collider2D>() }
    };
    private int lastCleanupFrame = -1;

    public static float LineWidth => Math.Max(0.7f, Screen.width / 960f * GameCameras.instance.tk2dCam.ZoomFactor);

    private void Start()
    {
        foreach (Collider2D col in Resources.FindObjectsOfTypeAll<Collider2D>())
        {
            TryAddHitboxes(col);
        }
    }

    public void UpdateHitbox(GameObject go)
    {
        foreach (Collider2D col in go.GetComponentsInChildren<Collider2D>(true))
        {
            TryAddHitboxes(col);
        }
    }

    private static Vector2 LocalToScreenPoint(Camera camera, Collider2D collider2D, Vector2 point)
    {
        Vector2 result = camera.WorldToScreenPoint((Vector2)collider2D.transform.TransformPoint(point + collider2D.offset));
        return new Vector2((int)Math.Round(result.x), (int)Math.Round(Screen.height - result.y));
    }

    private void TryAddHitboxes(Collider2D collider2D)
    {
        if (collider2D == null)
        {
            return;
        }

        if (collider2D is BoxCollider2D or PolygonCollider2D or EdgeCollider2D or CircleCollider2D)
        {
            GameObject go = collider2D.gameObject;
            if (collider2D.GetComponent<global::HeroBox>() != null)
            {
                colliders[HitboxType.KnightHurtbox].Add(collider2D);
            }
            else if (collider2D.GetComponent<DamageHero>() || collider2D.gameObject.LocateMyFSM("damages_hero"))
            {
                colliders[HitboxType.Enemy].Add(collider2D);
            }
            else if (go.GetComponent<HealthManager>() || go.LocateMyFSM("health_manager_enemy") || go.LocateMyFSM("health_manager"))
            {
                colliders[HitboxType.Other].Add(collider2D);
            }
            else if (go.layer == (int)PhysLayers.TERRAIN)
            {
                if (go.name.Contains("Breakable") || go.name.Contains("Collapse") || go.GetComponent<Breakable>() != null)
                {
                    colliders[HitboxType.Breakable].Add(collider2D);
                }
                else
                {
                    colliders[HitboxType.Terrain].Add(collider2D);
                }
            }
            else if (go == HeroController.instance?.gameObject && !collider2D.isTrigger)
            {
                colliders[HitboxType.Knight].Add(collider2D);
            }
            else if (go.GetComponent<DamageEnemies>() || go.LocateMyFSM("damages_enemy") || go.name == "Damager" && go.LocateMyFSM("Damage"))
            {
                colliders[HitboxType.Attack].Add(collider2D);
            }
            else if (collider2D.isTrigger && collider2D.GetComponent<HazardRespawnTrigger>())
            {
                colliders[HitboxType.HazardRespawn].Add(collider2D);
            }
            else if (collider2D.isTrigger && collider2D.GetComponent<TransitionPoint>())
            {
                colliders[HitboxType.Gate].Add(collider2D);
            }
            else if (collider2D.GetComponent<Breakable>())
            {
                NonBouncer? bounce = collider2D.GetComponent<NonBouncer>();
                if (bounce == null || !bounce.active)
                {
                    colliders[HitboxType.Trigger].Add(collider2D);
                }
            }
        }
    }

    private void OnGUI()
    {
        if (Event.current?.type != EventType.Repaint || Camera.main == null || GameManager.instance == null || GameManager.instance.isPaused)
        {
            return;
        }

        CleanupDestroyedCollidersIfNeeded();

        GUI.depth = int.MaxValue;
        Camera camera = Camera.main;
        float lineWidth = LineWidth;
        foreach (KeyValuePair<HitboxType, HashSet<Collider2D>> pair in colliders)
        {
            foreach (Collider2D collider2D in pair.Value)
            {
                DrawHitbox(camera, collider2D, pair.Key, lineWidth);
            }
        }

        DrawHitSource(camera, lineWidth);
        DrawUnfreezeHint();
    }

    private static readonly Color HitSourceColor = new(1f, 0.15f, 0.85f);

    private static void DrawHitSource(Camera camera, float lineWidth)
    {
        IReadOnlyList<FreezeHitSource.Shape> shapes = FreezeHitSource.Shapes;
        if (shapes.Count == 0)
        {
            return;
        }

        int origDepth = GUI.depth;
        GUI.depth = -2;
        foreach (FreezeHitSource.Shape shape in shapes)
        {
            DrawWorldShape(camera, shape, Color.black, lineWidth * 4f);
        }

        foreach (FreezeHitSource.Shape shape in shapes)
        {
            DrawWorldShape(camera, shape, HitSourceColor, lineWidth * 2.5f);
        }

        GUI.depth = origDepth;
    }

    private static void DrawWorldShape(Camera camera, FreezeHitSource.Shape shape, Color color, float width)
    {
        Vector2[] points = shape.Points;
        int count = shape.Closed ? points.Length : points.Length - 1;
        for (int i = 0; i < count; i++)
        {
            FreezeHitboxesDrawing.DrawLine(WorldToGui(camera, points[i]), WorldToGui(camera, points[(i + 1) % points.Length]), color, width, true);
        }
    }

    private static Vector2 WorldToGui(Camera camera, Vector2 world)
    {
        Vector3 screen = camera.WorldToScreenPoint(world);
        return new Vector2((int)Math.Round(screen.x), (int)Math.Round(Screen.height - screen.y));
    }

    private static readonly Color HintPlateColor = new(0.02f, 0.015f, 0.01f, 0.86f);
    private static readonly Color HintLineColor = new(0.902f, 0.749f, 0.384f, 0.55f);
    private static readonly Color HintTextColor = new(0.93f, 0.9f, 0.84f, 1f);
    private static readonly Color HintLabelColor = new(0.66f, 0.64f, 0.6f, 1f);
    private const string HitSourceHex = "FF26D9";

    private GUIStyle? hintPlateStyle;
    private GUIStyle? hintTitleStyle;
    private GUIStyle? hintBodyStyle;
    private GUIStyle? hintSourceStyle;

    private void EnsureHintStyles()
    {
        if (hintPlateStyle == null)
        {
            Sprite plate = QuickMenu.HkUi.SoftPlate;
            hintPlateStyle = new GUIStyle
            {
                border = new RectOffset((int)plate.border.x, (int)plate.border.z, (int)plate.border.w, (int)plate.border.y)
            };
            hintPlateStyle.normal.background = plate.texture;
        }

        hintTitleStyle ??= new GUIStyle
        {
            alignment = TextAnchor.UpperCenter,
            wordWrap = false,
            richText = false
        };
        hintBodyStyle ??= new GUIStyle
        {
            alignment = TextAnchor.UpperCenter,
            wordWrap = false,
            richText = false
        };
        hintSourceStyle ??= new GUIStyle
        {
            alignment = TextAnchor.UpperCenter,
            wordWrap = false,
            richText = true
        };

        hintTitleStyle.font = QuickMenu.HkUi.TitleFont;
        hintTitleStyle.normal.textColor = Color.white;
        hintBodyStyle.font = QuickMenu.HkUi.BodyFont;
        hintBodyStyle.normal.textColor = HintTextColor;
        hintSourceStyle.font = QuickMenu.HkUi.BodyFont;
        hintSourceStyle.normal.textColor = HintLabelColor;
    }

    private void DrawUnfreezeHint()
    {
        EnsureHintStyles();
        float scale = Mathf.Max(0.5f, Screen.height / 1080f) * HudTextScales.Clamp(HudTextScales.FreezeHint);
        hintTitleStyle!.fontSize = Mathf.RoundToInt(32f * scale);
        hintBodyStyle!.fontSize = Mathf.RoundToInt(22f * scale);
        hintSourceStyle!.fontSize = Mathf.RoundToInt(22f * scale);

        KeyCode key = FreezeHitboxes.GetUnfreezeKey();
        string title = "FreezeHitboxes/Title".Localize();
        string instruction = key != KeyCode.None
            ? string.Format("FreezeHitboxes/HintKey".Localize(), key)
            : string.Format("FreezeHitboxes/HintNoKey".Localize(), QuickMenu.GetQuickMenuHotkeyLabel());
        string source = FreezeHitSource.Name;
        string? hitText = string.IsNullOrEmpty(source)
            ? null
            : string.Format("FreezeHitboxes/HitBy".Localize(), $"<color=#{HitSourceHex}>{source}</color>");

        Vector2 titleSize = hintTitleStyle.CalcSize(new GUIContent(title));
        Vector2 instructionSize = hintBodyStyle.CalcSize(new GUIContent(instruction));
        Vector2 hitSize = hitText != null ? hintSourceStyle.CalcSize(new GUIContent(hitText)) : Vector2.zero;

        float padding = 26f * scale;
        float contentWidth = Mathf.Max(titleSize.x, instructionSize.x, hitSize.x);
        float width = Mathf.Min(contentWidth + (padding * 2f), Screen.width * 0.94f);
        float lineGap = 10f * scale;
        float height = padding + titleSize.y + lineGap + (2f * scale) + lineGap + instructionSize.y
            + (hitText != null ? (4f * scale) + hitSize.y : 0f) + padding;
        float x = (Screen.width - width) / 2f;
        float y = Screen.height * 0.06f;

        Color previous = GUI.color;
        GUI.color = HintPlateColor;
        GUI.Box(new Rect(x, y, width, height), GUIContent.none, hintPlateStyle);
        GUI.color = Color.white;

        float cursor = y + padding;
        GUI.Label(new Rect(x, cursor, width, titleSize.y), title, hintTitleStyle);
        cursor += titleSize.y + lineGap;

        GUI.color = HintLineColor;
        GUI.DrawTexture(new Rect(x + (width * 0.2f), cursor, width * 0.6f, Mathf.Max(1f, 2f * scale)), Texture2D.whiteTexture);
        GUI.color = Color.white;
        cursor += (2f * scale) + lineGap;

        GUI.Label(new Rect(x, cursor, width, instructionSize.y), instruction, hintBodyStyle);
        cursor += instructionSize.y;

        if (hitText != null)
        {
            cursor += 4f * scale;
            GUI.Label(new Rect(x, cursor, width, hitSize.y), hitText, hintSourceStyle);
        }

        GUI.color = previous;
    }

    private void CleanupDestroyedCollidersIfNeeded()
    {
        int frame = Time.frameCount;
        if (frame == lastCleanupFrame || frame % CleanupIntervalFrames != 0)
        {
            return;
        }

        lastCleanupFrame = frame;
        foreach (HashSet<Collider2D> set in colliders.Values)
        {
            set.RemoveWhere(collider => collider == null);
        }
    }

    private static void DrawHitbox(Camera camera, Collider2D collider2D, HitboxType hitboxType, float lineWidth)
    {
        if (collider2D == null || !collider2D.isActiveAndEnabled)
        {
            return;
        }

        int origDepth = GUI.depth;
        GUI.depth = hitboxType.Depth;
        if (collider2D is BoxCollider2D or EdgeCollider2D or PolygonCollider2D)
        {
            switch (collider2D)
            {
                case BoxCollider2D boxCollider2D:
                    Vector2 halfSize = boxCollider2D.size / 2f;
                    Vector2 topLeft = new(-halfSize.x, halfSize.y);
                    Vector2 topRight = halfSize;
                    Vector2 bottomRight = new(halfSize.x, -halfSize.y);
                    Vector2 bottomLeft = -halfSize;
                    List<Vector2> boxPoints = new()
                    {
                        topLeft, topRight, bottomRight, bottomLeft, topLeft
                    };
                    DrawPointSequence(boxPoints, camera, collider2D, hitboxType, lineWidth);
                    break;
                case EdgeCollider2D edgeCollider2D:
                    DrawPointSequence(new List<Vector2>(edgeCollider2D.points), camera, collider2D, hitboxType, lineWidth);
                    break;
                case PolygonCollider2D polygonCollider2D:
                    for (int i = 0; i < polygonCollider2D.pathCount; i++)
                    {
                        List<Vector2> polygonPoints = new(polygonCollider2D.GetPath(i));
                        if (polygonPoints.Count > 0)
                        {
                            polygonPoints.Add(polygonPoints[0]);
                        }
                        DrawPointSequence(polygonPoints, camera, collider2D, hitboxType, lineWidth);
                    }
                    break;
            }
        }
        else if (collider2D is CircleCollider2D circleCollider2D)
        {
            Vector2 center = LocalToScreenPoint(camera, collider2D, Vector2.zero);
            Vector2 right = LocalToScreenPoint(camera, collider2D, Vector2.right * circleCollider2D.radius);
            int radius = (int)Math.Round(Vector2.Distance(center, right));
            FreezeHitboxesDrawing.DrawCircle(center, radius, hitboxType.Color, lineWidth, true, Mathf.Clamp(radius / 16, 4, 32));
        }

        GUI.depth = origDepth;
    }

    private static void DrawPointSequence(List<Vector2> points, Camera camera, Collider2D collider2D, HitboxType hitboxType, float lineWidth)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 pointA = LocalToScreenPoint(camera, collider2D, points[i]);
            Vector2 pointB = LocalToScreenPoint(camera, collider2D, points[i + 1]);
            FreezeHitboxesDrawing.DrawLine(pointA, pointB, hitboxType.Color, lineWidth, true);
        }
    }
}
