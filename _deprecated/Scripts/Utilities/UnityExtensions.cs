using UnityEngine;
using System.Collections;

public static class UnityExtensions
{

    #region Transform

    /// <summary>
    /// Resets transform values.
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetTransform(this Transform transform)
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Resets transform local values.
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetLocalTransform(this Transform transform)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = new Quaternion(0, 0, 0, 1);
        transform.localScale = new Vector3(1, 1, 1);
    }


    /// <summary>
    /// Equivalent to transform.position + Vector3.up * value.
    /// </summary>
    /// <param name="transform">Transform to affect.</param>
    /// <param name="value">Y value.</param>
    /// <returns>Transform position plus Vector3.up * value</returns>
    public static Vector3 WithY(this Transform transform, float value)
    {
        return transform.position + new Vector3(0, value, 0);
    }

    /// <summary>
    /// Equivalent to transform.position + Vector3.forward * value.
    /// </summary>
    /// <param name="transform">Transform to affect.</param>
    /// <param name="value">Z value.</param>
    /// <returns>Transform position plus Vector3.forward * value</returns>
    public static Vector3 WithZ(this Transform transform, float value)
    {
        return transform.position + new Vector3(0, 0, value);
    }

    /// <summary>
    /// Returns Vector with X and Z values to transform position.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>Returns Vector with X and Z values to transform position.</returns>
    public static Vector3 WithXZ(this Transform transform, float x, float z)
    {
        return transform.position + new Vector3(x, 0, z);
    }


    /// <summary>
    /// Gets angle around Y axis from a world space direction.
    /// </summary>
    /// <param name="transform">Transform to affect</param>
    /// <param name="worldDirection">World space direction.</param>
    /// <returns>Angle between transform forward and world direction.</returns>
    public static float AngleFromForward(this Transform transform, Vector3 worldDirection)
    {
        Vector3 local = transform.InverseTransformDirection(worldDirection);
        return Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
    }


    #endregion


    #region GameObjects

    /// <summary>
    /// Returns the component of Type type. If one doesn't already exist on the GameObject it will be added.
    /// </summary>
    /// <typeparam name="T">The type of Component to return.</typeparam>
    /// <param name="gameObject">The GameObject this Component is attached to.</param>
    /// <returns>Component</returns>
    static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }




    #endregion


    #region Colliders

    public static Vector3 ClosestPointExt(this Collider c, Vector3 p)
    {
        if (c is SphereCollider) {
            var csc = c as SphereCollider;

            var scale = csc.transform.localScale;
            return c.transform.position + (p - c.transform.position).normalized * csc.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        }
        else if (c is BoxCollider) {
            var cbc = c as BoxCollider;
            var local_p = cbc.transform.InverseTransformPoint(p);

            local_p -= cbc.center;

            var minsize = -0.5f * cbc.size;
            var maxsize = 0.5f * cbc.size;

            local_p.x = Mathf.Clamp(local_p.x, minsize.x, maxsize.x);
            local_p.y = Mathf.Clamp(local_p.y, minsize.y, maxsize.y);
            local_p.z = Mathf.Clamp(local_p.z, minsize.z, maxsize.z);

            local_p += cbc.center;

            return cbc.transform.TransformPoint(local_p);
        }
        else if (c is CapsuleCollider) {
            // TODO: Only supports Y axis based capsules now
            var ccc = c as CapsuleCollider;
            var local_p = ccc.transform.InverseTransformPoint(p);
            local_p -= ccc.center;

            // Clamp inside outer cylinder top/bot
            local_p.y = Mathf.Clamp(local_p.y, -ccc.height * 0.5f, ccc.height * 0.5f);

            // Clamp to cylinder edge
            Vector2 h = new Vector2(local_p.x, local_p.z);
            h = h.normalized * ccc.radius;
            local_p.x = h.x;
            local_p.z = h.y;

            // Capsule ends
            float dist_to_top = ccc.height * 0.5f - Mathf.Abs(local_p.y);
            if (dist_to_top < ccc.radius) {
                float f = (ccc.radius - dist_to_top) / ccc.radius;
                float scaledown = Mathf.Sqrt(1.0f - f * f);
                local_p.x *= scaledown;
                local_p.z *= scaledown;
            }

            local_p += ccc.center;
            return ccc.transform.TransformPoint(local_p);
        }
        else {
            return c.ClosestPointOnBounds(p);
        }
    }


    #endregion



    #region LayerMask

    public static bool Contains(this LayerMask layers, GameObject gameObject)
    {
        return 0 != (layers.value & 1 << gameObject.layer);
    }

    #endregion


    #region Mis

    static public T GetOrAddComponent<T>(this MonoBehaviour mono) where T : Component
    {
        return mono.GetComponent<T>() ?? mono.gameObject.AddComponent<T>();
    }

    static public T GetOrAddComponent<T>(this Component component) where T : Component
    {
        return component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
    }

    #endregion


    #region Color

    public static Color Darker(this Color color, bool includeAlpha = false, float steps = 1, float interval = 0.1f)
    {
        return GetShade(color, false, includeAlpha, steps, interval);
    }

    public static Color Brighter(this Color color, bool includeAlpha = false, float steps = 1, float interval = 0.1f)
    {
        return GetShade(color, true, includeAlpha, steps, interval);
    }

    public static Color GetShade(this Color color, bool lighten,  bool includeAlpha = false, float steps = 1, float interval = 0.1f)
    {
        var sign = lighten ? 1 : -1;
        var r = Mathf.Clamp01(color.r + sign * (steps * interval));
        var g = Mathf.Clamp01(color.g + sign * (steps * interval));
        var b = Mathf.Clamp01(color.b + sign * (steps * interval));
        var a = includeAlpha ? Mathf.Clamp(color.a + sign * (steps * interval), 0.2f, 1f) : color.a;
        return new Color(r, g, b, a);
    }

    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }


    //public static string HexConverter(this Color color)
    //{
    //    return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    //}

    //public static string RGBConverter(this Color color)
    //{
    //    return "RGB(" + color.r.ToString() + "," + color.g.ToString() + "," + color.b.ToString() + ")";
    //}



    #endregion


}
