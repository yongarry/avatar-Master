using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CurvedPlane))]
[CanEditMultipleObjects]
public class CurvedPlaneEditor : Editor {

	SerializedProperty UseFixedUpdate;
	SerializedProperty Poligons;
	SerializedProperty DefaultCenter;
	SerializedProperty Center;
	SerializedProperty useCurving;
	SerializedProperty CurveCoefX;
	SerializedProperty CurveCoefY;
	SerializedProperty CustomMaterial;
	SerializedProperty Target;
	SerializedProperty TexturesAspectRatio;
	SerializedProperty Size;
	SerializedProperty Width;
	SerializedProperty Height;

	void OnEnable()
	{
		UseFixedUpdate = serializedObject.FindProperty ("UseFixedUpdate");
		useCurving = serializedObject.FindProperty("useCurving");
		Poligons = serializedObject.FindProperty ("quality");
		DefaultCenter = serializedObject.FindProperty ("defaultCenter");
		Center = serializedObject.FindProperty ("m_CustomCenter");
		useCurving = serializedObject.FindProperty ("useCurving");
		CurveCoefX = serializedObject.FindProperty ("m_CurveCoeficientX");
		CurveCoefY = serializedObject.FindProperty ("m_CurveCoeficientY");
		CustomMaterial = serializedObject.FindProperty ("CustomMaterial");
		Target = serializedObject.FindProperty ("m_Target");
		TexturesAspectRatio = serializedObject.FindProperty ("TexturesSize");
		Size = serializedObject.FindProperty ("Scale");
		Width = serializedObject.FindProperty ("width");
		Height = serializedObject.FindProperty ("height");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField (UseFixedUpdate);

		EditorGUILayout.PropertyField (useCurving);
		if (useCurving.boolValue) {
			EditorGUILayout.PropertyField (CurveCoefX);
			EditorGUILayout.PropertyField (CurveCoefY);
			EditorGUILayout.PropertyField (Poligons);
			EditorGUILayout.PropertyField (DefaultCenter);
			if (!DefaultCenter.boolValue) {
				EditorGUILayout.PropertyField (Center);
			}

		}
		EditorGUILayout.PropertyField (CustomMaterial);
		if (CustomMaterial.objectReferenceValue == null) {
			EditorGUILayout.HelpBox ("Will create standard material instead", MessageType.Info);
		}
		EditorGUILayout.PropertyField (Target);
		if (Target.objectReferenceValue == null) {
			EditorGUILayout.HelpBox ("Will use Camera.main instead", MessageType.Info);
		}

		EditorGUILayout.PropertyField (TexturesAspectRatio);
		if (TexturesAspectRatio.boolValue) {
			EditorGUILayout.PropertyField (Size);
		} else {
			EditorGUILayout.PropertyField (Width);
			EditorGUILayout.PropertyField (Height);
		}





		serializedObject.ApplyModifiedProperties();
	}
}
