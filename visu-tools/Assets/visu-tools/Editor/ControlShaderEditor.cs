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
    SerializedProperty depthMaterial;

    SerializedProperty scaleMotion;
    SerializedProperty threshold;

    SerializedProperty kernelSize;
    SerializedProperty radialBlurOriginX;
    SerializedProperty radialBlurOriginY;
    SerializedProperty radialBlurOriginXLeftEye;
    SerializedProperty radialBlurOriginYLeftEye;
    SerializedProperty radialBlurOriginXRightEye;
    SerializedProperty radialBlurOriginYRightEye;
    SerializedProperty scaleRadial;

    SerializedProperty colorNear;
    SerializedProperty colorFar;

    SerializedProperty shaderActive;

    // bools to toggle specific properties within inspector
    bool setUpGroup, additionalSettings = true;
    #endregion

    private void OnEnable()
    {
        cam = serializedObject.FindProperty("cam");
        motionFieldMaterial = serializedObject.FindProperty("motionFieldMaterial");
        imageFilterMaterial = serializedObject.FindProperty("imageFilterMaterial");
        depthMaterial = serializedObject.FindProperty("depthMaterial");

        scaleMotion = serializedObject.FindProperty("scaleMotion");
        threshold = serializedObject.FindProperty("threshold");

        kernelSize = serializedObject.FindProperty("kernelSize");
        radialBlurOriginX = serializedObject.FindProperty("radialBlurOriginX");
        radialBlurOriginY = serializedObject.FindProperty("radialBlurOriginY");
        radialBlurOriginXLeftEye = serializedObject.FindProperty("radialBlurOriginXLeftEye");
        radialBlurOriginYLeftEye = serializedObject.FindProperty("radialBlurOriginYLeftEye");
        radialBlurOriginXRightEye = serializedObject.FindProperty("radialBlurOriginXRightEye");
        radialBlurOriginYRightEye = serializedObject.FindProperty("radialBlurOriginYRightEye");
        scaleRadial = serializedObject.FindProperty("scaleRadial");

        colorNear = serializedObject.FindProperty("colorNear");
        colorFar = serializedObject.FindProperty("colorFar");

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
            EditorGUILayout.PropertyField(depthMaterial);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space(10);

        // make a section for the shader selection for all the set-up elements
        EditorGUILayout.LabelField("Shader selection");
        EditorGUILayout.PropertyField(shaderActive);

        // make a section for the shader settings
        if (controlShader.shaderActive == ControlShader.ShaderActive.Depth)
        {
            additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
            EditorGUILayout.PropertyField(colorNear);
            EditorGUILayout.PropertyField(colorFar);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        else if (controlShader.shaderActive != ControlShader.ShaderActive.None && controlShader.shaderActive != ControlShader.ShaderActive.Motion && controlShader.shaderActive != ControlShader.ShaderActive.Depth)
        {
            additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
            if (additionalSettings)
            {

                EditorGUILayout.PropertyField(kernelSize);

                if (controlShader.shaderActive == ControlShader.ShaderActive.RadialBlur || controlShader.shaderActive == ControlShader.ShaderActive.RadialBlurDesat)
                {
                    if (controlShader.XrActive)
                    {
                        EditorGUILayout.PropertyField(radialBlurOriginXLeftEye);
                        EditorGUILayout.PropertyField(radialBlurOriginYLeftEye);
                        EditorGUILayout.PropertyField(radialBlurOriginXRightEye);
                        EditorGUILayout.PropertyField(radialBlurOriginYRightEye);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(radialBlurOriginX);
                        EditorGUILayout.PropertyField(radialBlurOriginY);
                    }

                    EditorGUILayout.PropertyField(scaleRadial);
                }

                if (controlShader.shaderActive == ControlShader.ShaderActive.MotionOnHighPass || controlShader.shaderActive == ControlShader.ShaderActive.MotionOnHighPassOnDepth)
                {
                    EditorGUILayout.PropertyField(scaleMotion);
                    EditorGUILayout.PropertyField(threshold);
                }

                if (controlShader.shaderActive == ControlShader.ShaderActive.MotionOnHighPassOnDepth || controlShader.shaderActive == ControlShader.ShaderActive.HighPassOnDepth)
                {
                    EditorGUILayout.PropertyField(colorNear);
                    EditorGUILayout.PropertyField(colorFar);
                }

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (controlShader.shaderActive == ControlShader.ShaderActive.Motion || controlShader.shaderActive == ControlShader.ShaderActive.HighPassOnMotion)
        {
            EditorGUILayout.PropertyField(scaleMotion);
        }

        serializedObject.ApplyModifiedProperties();
    }


}
