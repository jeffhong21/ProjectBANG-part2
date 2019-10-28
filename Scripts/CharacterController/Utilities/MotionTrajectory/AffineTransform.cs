using UnityEngine;


[System.Serializable]
public struct AffineTransform
{
    //public Vector3 translation
    //{
    //    get {
    //        var ws = transform.TransformPoint(p);
    //        var dif = transform.localPosition + p - ws;
    //        return ws + dif;
    //    }
    //    set {
    //        p = value;
    //    }
    //}
    //public Quaternion rotation
    //{
    //    get { return r; }
    //    set { r = value; }
    //}

    public Vector3 translation;
    public Quaternion rotation;

    public Transform transform { get; }

    public AffineTransform(Transform transform)
    {
        this.transform = transform;
        this.translation = Vector3.zero;
        this.rotation = Quaternion.identity;
    }

    public void Set(Vector3 t, Quaternion r)
    {
        translation = t;
        rotation = r;
    }

    //private Vector3 p;
}