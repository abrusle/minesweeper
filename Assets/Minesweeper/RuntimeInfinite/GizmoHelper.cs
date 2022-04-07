using UnityEngine;

public static class GizmoHelper
{
    public static void DrawRect(Rect rect)
    {
        Vector3 bl = new(rect.xMin, rect.yMin, 0);
        Vector3 tl = new(bl.x, rect.yMax, 0);
        Vector3 br = new(rect.xMax, bl.y, 0);
        Vector3 tr = new(br.x, tl.y, 0);
        Gizmos.DrawLine(bl, tl);
        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
    }
}