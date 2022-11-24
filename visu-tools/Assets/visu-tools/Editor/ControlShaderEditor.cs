using UnityEditor;


/* This scripts purpose is to make the inspector for the ControlShader script look nice and easier to use. */
[CustomEditor(typeof(ControlShader))]
public class ControlShaderEditor : Editor
{
    #region properties

    // getting acces to the serialized properties
    SerializedProperty cam;
    SerializedProperty motionFieldMaterial;
    SerializedProperty imageFilterMaterial;

    SerializedProperty threshold;

    SerializedProperty kernelSize;
    SerializedProperty radialBlurOriginX;
    SerializedProperty radialBlurOriginY;
    SerializedProperty scale;

    SerializedProperty shaderActive;

    // bools to toggle specific properties within inspector
    bool setUpGroup, additionalSettings = true;
    #endregion

    private void OnEnable()
    {
        cam = serializedObject.FindProperty("cam");
        motionFieldMaterial = serializedObject.FindProperty("motionFieldMaterial");
        imageFilterMaterial = serializedObject.FindProperty("imageFilterMaterial");

        threshold = serializedObject.FindProperty("threshold");

        kernelSize = serializedObject.FindProperty("kernelSize");
        radialBlurOriginX = serializedObject.FindProperty("radialBlurOriginX");
        radialBlurOriginY = serializedObject.FindProperty("radialBlurOriginY");
        scale = serializedObject.FindProperty("scale");

        shaderActive = serializedObject.FindProperty("shaderActive");
    }

    public override void OnInspectorGUI()
    {
        // getting acces to COntrolShader script
        ControlShader controlShader = (ControlShader)target;

        serializedObject.Update();

        // make a section for all the set-up elements
        setUpGroup = EditorGUILayout.BeginFoldoutHeaderGroup(setUpGroup, "Set-up");
        if (setUpGroup)
        {
            EditorGUILayout.PropertyField(cam);
            EditorGUILayout.PropertyField(motionFieldMaterial);
            EditorGUILayout.PropertyField(imageFilterMaterial);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);

        // make a section for the shader selection for all the set-up elements
        EditorGUILayout.LabelField("Shader selection");
        EditorGUILayout.PropertyField(shaderActive);

        // make a section for the shader settings
        if (controlShader.shaderActive != ControlShader.ShaderActive.None && controlShader.shaderActive != ControlShader.ShaderActive.Motion)
        {
            additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
            if (additionalSettings)
            {

                EditorGUILayout.PropertyField(kernelSize);

                if (controlShader.shaderActive == ControlShader.ShaderActive.RadialBlur || controlShader.shaderActive == ControlShader.ShaderActive.RadialBlurDesat)
                {
                    EditorGUILayout.PropertyField(radialBlurOriginX);
                    EditorGUILayout.PropertyField(radialBlurOriginY);
                    EditorGUILayout.PropertyField(scale);
                }

                if (controlShader.shaderActive == ControlShader.ShaderActive.MotionOnHighPass)
                {
                    EditorGUILayout.PropertyField(threshold);
                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }


}
