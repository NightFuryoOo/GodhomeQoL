using System.Reflection;

namespace GodhomeQoL.Modules.Tools;

internal static class FreezeHitboxesDrawing
{
    private static Texture2D? aaLineTex;
    private static Texture2D? lineTex;
    private static Material? blitMaterial;
    private static Material? blendMaterial;
    private static readonly Rect lineRect = new(0, 0, 1, 1);

    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
    {
#if UNITY_EDITOR
        if (lineTex == null)
        {
            Initialize();
        }
#endif

        float dx = pointB.x - pointA.x;
        float dy = pointB.y - pointA.y;
        float len = Mathf.Sqrt(dx * dx + dy * dy);
        if (len < 0.001f)
        {
            return;
        }

        Texture2D tex;
        Material mat;
        if (antiAlias)
        {
            width *= 3.0f;
            tex = aaLineTex!;
            mat = blendMaterial!;
        }
        else
        {
            tex = lineTex!;
            mat = blitMaterial!;
        }

        float wdx = width * dy / len;
        float wdy = width * dx / len;

        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = dx;
        matrix.m01 = -wdx;
        matrix.m03 = pointA.x + 0.5f * wdx;
        matrix.m10 = dy;
        matrix.m11 = wdy;
        matrix.m13 = pointA.y - 0.5f * wdy;

        GL.PushMatrix();
        GL.MultMatrix(matrix);

        Graphics.DrawTexture(lineRect, tex, lineRect, 0, 0, 0, 0, color, mat);
        if (antiAlias)
        {
            Graphics.DrawTexture(lineRect, tex, lineRect, 0, 0, 0, 0, color, mat);
        }

        GL.PopMatrix();
    }

    public static void DrawCircle(Vector2 center, int radius, Color color, float width, bool antiAlias, int segmentsPerQuarter)
    {
        float rh = radius * 0.551915024494f;

        Vector2 p1 = new(center.x, center.y - radius);
        Vector2 p1TanA = new(center.x - rh, center.y - radius);
        Vector2 p1TanB = new(center.x + rh, center.y - radius);

        Vector2 p2 = new(center.x + radius, center.y);
        Vector2 p2TanA = new(center.x + radius, center.y - rh);
        Vector2 p2TanB = new(center.x + radius, center.y + rh);

        Vector2 p3 = new(center.x, center.y + radius);
        Vector2 p3TanA = new(center.x - rh, center.y + radius);
        Vector2 p3TanB = new(center.x + rh, center.y + radius);

        Vector2 p4 = new(center.x - radius, center.y);
        Vector2 p4TanA = new(center.x - radius, center.y - rh);
        Vector2 p4TanB = new(center.x - radius, center.y + rh);

        DrawBezierLine(p1, p1TanB, p2, p2TanA, color, width, antiAlias, segmentsPerQuarter);
        DrawBezierLine(p2, p2TanB, p3, p3TanB, color, width, antiAlias, segmentsPerQuarter);
        DrawBezierLine(p3, p3TanA, p4, p4TanB, color, width, antiAlias, segmentsPerQuarter);
        DrawBezierLine(p4, p4TanA, p1, p1TanA, color, width, antiAlias, segmentsPerQuarter);
    }

    public static void DrawBezierLine(
        Vector2 start,
        Vector2 startTangent,
        Vector2 end,
        Vector2 endTangent,
        Color color,
        float width,
        bool antiAlias,
        int segments)
    {
        Vector2 lastV = CubeBezier(start, startTangent, end, endTangent, 0);
        for (int i = 1; i < segments + 1; ++i)
        {
            Vector2 v = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
            DrawLine(lastV, v, color, width, antiAlias);
            lastV = v;
        }
    }

    private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
    {
        float rt = 1 - t;
        return rt * rt * rt * s + 3 * rt * rt * t * st + 3 * rt * t * t * et + t * t * t * e;
    }

    static FreezeHitboxesDrawing()
    {
        Initialize();
    }

    private static void Initialize()
    {
        if (lineTex == null)
        {
            lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            lineTex.SetPixel(0, 1, Color.white);
            lineTex.Apply();
        }

        if (aaLineTex == null)
        {
            aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, false);
            aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
            aaLineTex.SetPixel(0, 1, Color.white);
            aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
            aaLineTex.Apply();
        }

        blitMaterial = (Material)typeof(GUI)
            .GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, null);
        blendMaterial = (Material)typeof(GUI)
            .GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, null);
    }
}
