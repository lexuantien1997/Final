using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {
	public static Vector2 Rotate(this Vector2 v , float degree) {
		float sin = Mathf.Sin (degree * Mathf.Deg2Rad);
		float cos = Mathf.Cos (degree * Mathf.Deg2Rad);

		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) - (cos * ty);
		return v;
	}


    public static Bounds InverseTransformBounds(this Transform transform,Bounds worldBounds)
    {
        var center = transform.InverseTransformPoint(worldBounds.center);

        var extents = worldBounds.extents;
        var axisx = transform.InverseTransformVector(extents.x, 0, 0);
        var axisy = transform.InverseTransformVector(0, extents.y, 0);
        var axisz = transform.InverseTransformVector(0, 0, extents.z);

        extents.x = Mathf.Abs(axisx.x) + Mathf.Abs(axisy.x) + Mathf.Abs(axisz.x);
        extents.y = Mathf.Abs(axisx.y) + Mathf.Abs(axisy.y) + Mathf.Abs(axisz.y);
        extents.z = Mathf.Abs(axisx.z) + Mathf.Abs(axisy.z) + Mathf.Abs(axisz.z);

        return new Bounds { center = center, extents = extents };

    }

}
