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

    SerializedProperty scaleMotionField;
    SerializedProperty threshold;

    SerializedProperty kernelSize;
    SerializedProperty radialBlurOriginX;
    SerializedProperty radialBlurOriginY;
    SerializedProperty radialBlurOriginXLeftEye;
    SerializedProperty radialBlurOriginYLeftEye;
    SerializedProperty radialBlurOriginXRightEye;
    SerializedProperty radialBlurOriginYRightEye;
    SerializedProperty scaleRadial;
    SerializedProperty scaleMotionBlur;

    SerializedProperty colorNear;
    SerializedProperty colorFar;

    SerializedProperty shaderActive;

    // bools to toggle specific properties within inspector
    bool setUpGroup, additionalSettings = true;
    #endregion

    /* Method that gets called when object becomes enabled and active */
    private void OnEnable()
    {
        cam = serializedObject.FindProperty("cam");
        motionFieldMaterial = serializedObject.FindProperty("motionFieldMaterial");
        imageFilterMaterial = serializedObject.FindProperty("imageFilterMaterial");
        depthMaterial = serializedObject.FindProperty("depthMaterial");

        scaleMotionField = serializedObject.FindProperty("scaleMotionField");
        threshold = serializedObject.FindProperty("threshold");

        kernelSize = serializedObject.FindProperty("kernelSize");
        radialBlurOriginX = serializedObject.FindProperty("radialBlurOriginX");
        radialBlurOriginY = serializedObject.FindProperty("radialBlurOriginY");
        radialBlurOriginXLeftEye = serializedObject.FindProperty("radialBlurOriginXLeftEye");
        radialBlurOriginYLeftEye = serializedObject.FindProperty("radialBlurOriginYLeftEye");
        radialBlurOriginXRightEye = serializedObject.FindProperty("radialBlurOriginXRightEye");
        radialBlurOriginYRightEye = serializedObject.FindProperty("radialBlurOriginYRightEye");
        scaleRadial = serializedObject.FindProperty("scaleRadial");
        scaleMotionBlur = serializedObject.FindProperty("scaleMotionBlur");

        colorNear = serializedObject.FindProperty("colorNear");
        colorFar = serializedObject.FindProperty("colorFar");

        shaderActive = serializedObject.FindProperty("shaderActive");
    }

    /* Method that makes it possible to customize inspector */
    public override void OnInspectorGUI()
    {
        // getting acces to C0ntrolShader script
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

        // make a section for the shader settings depending on which shader is currently active
        switch (controlShader.shaderActive)
        {
            case ControlShader.ShaderActive.RadialBlur:
            case ControlShader.ShaderActive.RadialBlurDesat:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
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
                break;
            case ControlShader.ShaderActive.GaussianBlur:
            case ControlShader.ShaderActive.Sharpening:
            case ControlShader.ShaderActive.HighPass:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.MotionBlur:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
                EditorGUILayout.PropertyField(scaleMotionBlur);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.MotionField:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(scaleMotionField);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.HighPassOnMotionField:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
                EditorGUILayout.PropertyField(scaleMotionField);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.MotionFieldOnHighPass:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
                EditorGUILayout.PropertyField(scaleMotionField);
                EditorGUILayout.PropertyField(threshold);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.Depth:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(colorNear);
                EditorGUILayout.PropertyField(colorFar);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.HighPassOnDepth:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
                EditorGUILayout.PropertyField(colorNear);
                EditorGUILayout.PropertyField(colorFar);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
            case ControlShader.ShaderActive.MotionFieldOnHighPassOnDepth:
                additionalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(additionalSettings, "Additional settings for active shader");
                EditorGUILayout.PropertyField(kernelSize);
                EditorGUILayout.PropertyField(scaleMotionField);
                EditorGUILayout.PropertyField(threshold);
                EditorGUILayout.PropertyField(colorNear);
                EditorGUILayout.PropertyField(colorFar);
                EditorGUILayout.EndFoldoutHeaderGroup();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
