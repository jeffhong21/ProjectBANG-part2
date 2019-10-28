using UnityEngine;
using UnityEditor;

public static class GizmosUtils
{
    static GUIStyle boldText = new GUIStyle()
    {
        fontStyle = FontStyle.Bold
    };


    public static void DrawText(GUISkin guiSkin, string text, Vector3 position, Color? color = null, int fontSize = 12, float yOffset = 16)
    {
#if UNITY_EDITOR
        var prevSkin = GUI.skin;
        if (guiSkin == null)
            Debug.LogWarning("editor warning: guiSkin parameter is null");
        else
            GUI.skin = guiSkin;

        GUIContent textContent = new GUIContent(text);

        GUIStyle style = (guiSkin != null) ? new GUIStyle(guiSkin.GetStyle("Label")) : new GUIStyle();
        if (color != null)
            style.normal.textColor = (Color)color;
        if (fontSize > 0)
            style.fontSize = fontSize;

        Vector2 textSize = style.CalcSize(textContent);
        Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);

        if (screenPoint.z > 0) // checks necessary to the text is not visible when the camera is pointed in the opposite direction relative to the object
        {
            var worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f, screenPoint.y + textSize.y * 0.5f + yOffset, screenPoint.z));
            UnityEditor.Handles.Label(worldPosition, textContent, style);
        }
        GUI.skin = prevSkin;
#endif
    }


    public static void DrawString(string text, Vector3 worldPos, Color? color = null)
    {
        //Handles.BeginGUI();
        ////if (color.HasValue) GUI.color = color.Value;
        //if (color == null)
        //    GUI.color = Color.black;
        //else
        //    GUI.color = color.Value;
        
        //var view = SceneView.currentDrawingSceneView;
        //Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        //Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));

        ////GUI.Label(new Rect(screenPos.x, -screenPos.y + 4, size.x, size.y), text);
        //GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text, boldText);

        //Handles.EndGUI();
    }

    public static void DrawArrow(Vector3 pos, Vector3 direction, float length = 1f, float tipSize = 0.25f, float width = 0.5f)
    {
        direction.Normalize();

        var sideLen = length - length * tipSize;
        var widthOffset = Vector3.Cross(direction, Vector3.up) * width;

        var baseLeft = pos + widthOffset * 0.3f;
        var baseRight = pos - widthOffset * 0.3f;
        var tip = pos + direction * length;
        var upCornerInRight = pos - widthOffset * 0.3f + direction * sideLen;
        var upCornerInLeft = pos + widthOffset * 0.3f + direction * sideLen;
        var upCornerOutRight = pos - widthOffset * 0.5f + direction * sideLen;
        var upCornerOutLeft = pos + widthOffset * 0.5f + direction * sideLen;


        Gizmos.DrawLine(baseLeft, baseRight);
        Gizmos.DrawLine(baseRight, upCornerInRight);
        Gizmos.DrawLine(upCornerInRight, upCornerOutRight);
        Gizmos.DrawLine(upCornerOutRight, tip);
        Gizmos.DrawLine(tip, upCornerOutLeft);
        Gizmos.DrawLine(upCornerOutLeft, upCornerInLeft);
        Gizmos.DrawLine(upCornerInLeft, baseLeft);
    }


    public static void DrawMarker(Vector3 position, float size, Color color, float duration = 0, bool depthTest = true)
    {
        Vector3 line1PosA = position + Vector3.up * size * 0.5f;
        Vector3 line1PosB = position - Vector3.up * size * 0.5f;

        Vector3 line2PosA = position + Vector3.right * size * 0.5f;
        Vector3 line2PosB = position - Vector3.right * size * 0.5f;

        Vector3 line3PosA = position + Vector3.forward * size * 0.5f;
        Vector3 line3PosB = position - Vector3.forward * size * 0.5f;

        Debug.DrawLine(line1PosA, line1PosB, color, duration, depthTest);
        Debug.DrawLine(line2PosA, line2PosB, color, duration, depthTest);
        Debug.DrawLine(line3PosA, line3PosB, color, duration, depthTest);
    }







    /// <summary>
    /// Draws a wire cube with a given rotation 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="rotation"></param>
    public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation = default(Quaternion))
    {
        var old = Gizmos.matrix;
        if (rotation.Equals(default(Quaternion)))
            rotation = Quaternion.identity;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = old;
    }

    public static void DrawWireArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.DrawLine(from, to);
        var direction = to - from;
        var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawLine(to, to + right * arrowHeadLength);
        Gizmos.DrawLine(to, to + left * arrowHeadLength);
    }

    public static void DrawWireSphere(Vector3 center, float radius, Quaternion rotation = default(Quaternion))
    {
        var old = Gizmos.matrix;
        if (rotation.Equals(default(Quaternion)))
            rotation = Quaternion.identity;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        Gizmos.matrix = old;
    }


    /// <summary>
    /// Draws a flat wire circle (up)
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="segments"></param>
    /// <param name="rotation"></param>
    public static void DrawWireCircle(Vector3 center, float radius, int segments = 20, Quaternion rotation = default(Quaternion))
    {
        DrawWireArc(center, radius, 360, segments, rotation);
    }

    /// <summary>
    /// Draws an arc with a rotation around the center
    /// </summary>
    /// <param name="center">center point</param>
    /// <param name="radius">radiu</param>
    /// <param name="angle">angle in degrees</param>
    /// <param name="segments">number of segments</param>
    /// <param name="rotation">rotation around the center</param>
    public static void DrawWireArc(Vector3 center, float radius, float angle, int segments = 20, Quaternion rotation = default(Quaternion))
    {

        var old = Gizmos.matrix;

        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Vector3 from = Vector3.forward * radius;
        var step = Mathf.RoundToInt(angle / segments);
        for (int i = 0; i <= angle; i += step)
        {
            var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
            Gizmos.DrawLine(from, to);
            from = to;
        }

        Gizmos.matrix = old;
    }


    /// <summary>
    /// Draws an arc with a rotation around an arbitraty center of rotation
    /// </summary>
    /// <param name="center">the circle's center point</param>
    /// <param name="radius">radius</param>
    /// <param name="angle">angle in degrees</param>
    /// <param name="segments">number of segments</param>
    /// <param name="rotation">rotation around the centerOfRotation</param>
    /// <param name="centerOfRotation">center of rotation</param>
    public static void DrawWireArc(Vector3 center, float radius, float angle, int segments, Quaternion rotation, Vector3 centerOfRotation)
    {

        var old = Gizmos.matrix;
        if (rotation.Equals(default(Quaternion)))
            rotation = Quaternion.identity;
        Gizmos.matrix = Matrix4x4.TRS(centerOfRotation, rotation, Vector3.one);
        var deltaTranslation = centerOfRotation - center;
        Vector3 from = deltaTranslation + Vector3.forward * radius;
        var step = Mathf.RoundToInt(angle / segments);
        for (int i = 0; i <= angle; i += step)
        {
            var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad)) + deltaTranslation;
            Gizmos.DrawLine(from, to);
            from = to;
        }

        Gizmos.matrix = old;
    }

    /// <summary>
    /// Draws an arc with a rotation around an arbitraty center of rotation
    /// </summary>
    /// <param name="matrix">Gizmo matrix applied before drawing</param>
    /// <param name="radius">radius</param>
    /// <param name="angle">angle in degrees</param>
    /// <param name="segments">number of segments</param>
    public static void DrawWireArc(Matrix4x4 matrix, float radius, float angle, int segments)
    {
        var old = Gizmos.matrix;
        Gizmos.matrix = matrix;
        Vector3 from = Vector3.forward * radius;
        var step = Mathf.RoundToInt(angle / segments);
        for (int i = 0; i <= angle; i += step)
        {
            var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
            Gizmos.DrawLine(from, to);
            from = to;
        }

        Gizmos.matrix = old;
    }

    /// <summary>
    /// Draws a wire cylinder face up with a rotation around the center
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="height"></param>
    /// <param name="rotation"></param>
    public static void DrawWireCylinder(Vector3 center, float radius, float height, Quaternion rotation = default(Quaternion))
    {
        var old = Gizmos.matrix;
        if (rotation.Equals(default(Quaternion)))
            rotation = Quaternion.identity;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        var half = height / 2;

        //draw the 4 outer lines
        Gizmos.DrawLine(Vector3.right * radius - Vector3.up * half, Vector3.right * radius + Vector3.up * half);
        Gizmos.DrawLine(-Vector3.right * radius - Vector3.up * half, -Vector3.right * radius + Vector3.up * half);
        Gizmos.DrawLine(Vector3.forward * radius - Vector3.up * half, Vector3.forward * radius + Vector3.up * half);
        Gizmos.DrawLine(-Vector3.forward * radius - Vector3.up * half, -Vector3.forward * radius + Vector3.up * half);

        //draw the 2 cricles with the center of rotation being the center of the cylinder, not the center of the circle itself
        DrawWireArc(center + Vector3.up * half, radius, 360, 20, rotation, center);
        DrawWireArc(center + Vector3.down * half, radius, 360, 20, rotation, center);
        Gizmos.matrix = old;
    }

    /// <summary>
    /// Draws a wire capsule face up
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="height"></param>
    /// <param name="rotation"></param>
    public static void DrawWireCapsule(Vector3 center, float radius, float height, Quaternion rotation = default(Quaternion))
    {
        if (rotation.Equals(default(Quaternion)))
            rotation = Quaternion.identity;
        var old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        var half = height / 2 - radius;

        //draw cylinder base
        DrawWireCylinder(center, radius, height - radius * 2, rotation);

        //draw upper cap
        //do some cool stuff with orthogonal matrices
        var mat = Matrix4x4.Translate(center + rotation * Vector3.up * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.forward));
        DrawWireArc(mat, radius, 180, 20);
        mat = Matrix4x4.Translate(center + rotation * Vector3.up * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.forward));
        DrawWireArc(mat, radius, 180, 20);

        //draw lower cap
        mat = Matrix4x4.Translate(center + rotation * Vector3.down * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward));
        DrawWireArc(mat, radius, 180, 20);
        mat = Matrix4x4.Translate(center + rotation * Vector3.down * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(-90, Vector3.forward));
        DrawWireArc(mat, radius, 180, 20);

        Gizmos.matrix = old;

    }

}







