using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Helix Icon")]
public sealed class GameIconGraphic : Graphic
{
    public enum IconType
    {
        None,
        Play,
        Replay,
        Home,
        Menu,
        Bolt
    }

    [SerializeField] private IconType iconType;

    public void SetIcon(IconType icon, Color tint)
    {
        iconType = icon;
        color = tint;
        raycastTarget = false;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper)
    {
        vertexHelper.Clear();
        if (iconType == IconType.None)
            return;

        Rect rect = GetPixelAdjustedRect();
        float scale = Mathf.Min(rect.width, rect.height) * 0.48f;
        Vector2 center = rect.center;
        switch (iconType)
        {
            case IconType.Play:
                AddTriangle(vertexHelper, Point(center, scale, -0.32f, -0.52f),
                    Point(center, scale, -0.32f, 0.52f), Point(center, scale, 0.5f, 0f));
                break;
            case IconType.Home:
                AddQuad(vertexHelper, Point(center, scale, -0.43f, -0.38f),
                    Point(center, scale, 0.43f, 0.12f));
                AddTriangle(vertexHelper, Point(center, scale, -0.58f, 0.02f),
                    Point(center, scale, 0f, 0.58f), Point(center, scale, 0.58f, 0.02f));
                AddQuad(vertexHelper, Point(center, scale, -0.1f, -0.38f),
                    Point(center, scale, 0.1f, -0.02f));
                break;
            case IconType.Menu:
                AddQuad(vertexHelper, Point(center, scale, -0.48f, 0.32f), Point(center, scale, 0.48f, 0.44f));
                AddQuad(vertexHelper, Point(center, scale, -0.48f, -0.06f), Point(center, scale, 0.48f, 0.06f));
                AddQuad(vertexHelper, Point(center, scale, -0.48f, -0.44f), Point(center, scale, 0.48f, -0.32f));
                break;
            case IconType.Replay:
                AddReplayIcon(vertexHelper, center, scale);
                break;
            case IconType.Bolt:
                AddTriangle(vertexHelper, Point(center, scale, 0.12f, 0.58f),
                    Point(center, scale, -0.4f, -0.02f), Point(center, scale, 0.06f, 0.02f));
                AddTriangle(vertexHelper, Point(center, scale, 0.06f, 0.02f),
                    Point(center, scale, -0.08f, -0.58f), Point(center, scale, 0.42f, 0.08f));
                break;
        }
    }

    private void AddReplayIcon(VertexHelper vertexHelper, Vector2 center, float scale)
    {
        const int segments = 12;
        const float radius = 0.38f;
        const float thickness = 0.12f;
        float startAngle = 48f;
        float endAngle = 330f;
        Vector2 previous = PointOnCircle(center, scale, radius, startAngle);
        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, i / (float)segments);
            Vector2 next = PointOnCircle(center, scale, radius, angle);
            AddLine(vertexHelper, previous, next, scale * thickness);
            previous = next;
        }

        Vector2 arrowTip = PointOnCircle(center, scale, 0.66f, startAngle - 15f);
        Vector2 arrowLeft = PointOnCircle(center, scale, 0.2f, startAngle - 50f);
        Vector2 arrowRight = PointOnCircle(center, scale, 0.2f, startAngle + 2f);
        AddTriangle(vertexHelper, arrowLeft, arrowTip, arrowRight);
    }

    private void AddLine(VertexHelper vertexHelper, Vector2 from, Vector2 to, float thickness)
    {
        Vector2 direction = (to - from).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);
        AddQuad(vertexHelper, from - normal, to + normal, to - normal, from + normal);
    }

    private void AddQuad(VertexHelper vertexHelper, Vector2 min, Vector2 max)
    {
        AddQuad(vertexHelper, new Vector2(min.x, min.y), new Vector2(max.x, min.y),
            new Vector2(max.x, max.y), new Vector2(min.x, max.y));
    }

    private void AddQuad(VertexHelper vertexHelper, Vector2 bottomLeft, Vector2 bottomRight,
        Vector2 topRight, Vector2 topLeft)
    {
        int start = vertexHelper.currentVertCount;
        AddVertex(vertexHelper, bottomLeft);
        AddVertex(vertexHelper, bottomRight);
        AddVertex(vertexHelper, topRight);
        AddVertex(vertexHelper, topLeft);
        vertexHelper.AddTriangle(start, start + 1, start + 2);
        vertexHelper.AddTriangle(start, start + 2, start + 3);
    }

    private void AddTriangle(VertexHelper vertexHelper, Vector2 a, Vector2 b, Vector2 c)
    {
        int start = vertexHelper.currentVertCount;
        AddVertex(vertexHelper, a);
        AddVertex(vertexHelper, b);
        AddVertex(vertexHelper, c);
        vertexHelper.AddTriangle(start, start + 1, start + 2);
    }

    private void AddVertex(VertexHelper vertexHelper, Vector2 position)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.position = position;
        vertex.color = color;
        vertexHelper.AddVert(vertex);
    }

    private static Vector2 Point(Vector2 center, float scale, float x, float y) =>
        center + new Vector2(x, y) * scale;

    private static Vector2 PointOnCircle(Vector2 center, float scale, float radius, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return center + new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * (scale * radius);
    }
}
