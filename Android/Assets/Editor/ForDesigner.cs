using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(PropOption))]
public class ForDesigner : PropertyDrawer {

    // Draw the property inside the given rect
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //    // Using BeginProperty / EndProperty on the parent property means that
    //    // prefab override logic works on the entire property.
    //    EditorGUI.BeginProperty(position, label, property);
    //    // Draw label
    //    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //    // Don't make child fields be indented
    //    var indent = EditorGUI.indentLevel;
    //    EditorGUI.indentLevel = 0;

    //    // Calculate rects
    //    var Guard = new Rect(position.x, position.y, 30, position.height);
    //    var Pickup = new Rect(position.x + 35, position.y, 30, position.height);
    //    var BallPillarFire = new Rect(position.x, position.y - position.height, 30, position.height);
    //    var SpikeTrap = new Rect(position.x + 35, position.y - position.height, 30, position.height);
    //    var FireTrap = new Rect(position.x + 70, position.y - position.height * 2, 30, position.height);
    //    var GasTrap = new Rect(position.x, position.y - position.height * 2, 30, position.height);
    //    var CrushingWall = new Rect(position.x + 35, position.y - position.height * 2, 30, position.height);
    //    var Catapult = new Rect(position.x + 70, position.y - position.height * 2, 30, position.height);

    //    // Draw fields - passs GUIContent.none to each so they are drawn without labels
    //    EditorGUI.PropertyField(Guard, property.FindPropertyRelative("Guard"), GUIContent.none);
    //    EditorGUI.PropertyField(Pickup, property.FindPropertyRelative("Pickup"), GUIContent.none);
    //    EditorGUI.PropertyField(BallPillarFire, property.FindPropertyRelative("BallPillarFire"), GUIContent.none);
    //    EditorGUI.PropertyField(SpikeTrap, property.FindPropertyRelative("SpikeTrap"), GUIContent.none);
    //    EditorGUI.PropertyField(FireTrap, property.FindPropertyRelative("FireTrap"), GUIContent.none);
    //    EditorGUI.PropertyField(GasTrap, property.FindPropertyRelative("GasTrap"), GUIContent.none);
    //    EditorGUI.PropertyField(CrushingWall, property.FindPropertyRelative("CrushingWall"), GUIContent.none);
    //    EditorGUI.PropertyField(Catapult, property.FindPropertyRelative("Catapult"), GUIContent.none);

    //    // Set indent back to what it was
    //    EditorGUI.indentLevel = indent;

    //    EditorGUI.EndProperty();
    //}
}
