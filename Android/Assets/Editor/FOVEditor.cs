using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(FieldOfView))]
public class FOVEditor : Editor {

    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(fov.transform.position, fov.transform.up, fov.transform.forward, 360, fov.radius * fov.transform.lossyScale.x);
        Vector3 viewAngleA = fov.DirectionFormAngle(-fov.viewAngle / 2);
        Vector3 viewAngleB = fov.DirectionFormAngle(fov.viewAngle / 2);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.radius * fov.transform.lossyScale.x);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.radius * fov.transform.lossyScale.x);

        foreach(Transform visibleTargets in fov.visibleTargetTransform)
        {
            Handles.DrawLine(fov.transform.position, visibleTargets.position);
        }
    }
}
