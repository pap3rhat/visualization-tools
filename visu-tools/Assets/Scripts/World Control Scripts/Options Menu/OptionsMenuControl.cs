using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenuControl : MonoBehaviour
{
    [SerializeField] private GameObject objectMenuUI;
    [SerializeField] private GameObject optionsBtn;

    [SerializeField] private GameObject selectionDropdown;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera cam;

    [SerializeField] private GameObject addOptSliderPrefab;
    [SerializeField] private Transform scrollViewContent;

    private ControlShader controlShaderScript;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- CONTROL METHODS ---

    #region Control methods

    public void Start()
    {
        controlShaderScript = cam.GetComponent<ControlShader>(); // getting acces to script that controls which effect is applied to camera

        TMP_Dropdown selection = selectionDropdown.GetComponent<TMP_Dropdown>(); // getting dropdown menu
        selection.options.Clear(); // clearing dropdown, just to be sure

        List<string> availableEffects = Enum.GetValues(typeof(ControlShader.ShaderActive)).Cast<ControlShader.ShaderActive>().Select(v => v.ToString()).ToList();
        // changing option "None" to be first instead of last
        string last = availableEffects[availableEffects.Count - 1];
        availableEffects.RemoveAt(availableEffects.Count - 1);
        availableEffects.Insert(0, last);

        // adding options to dropdown selection
        foreach (string effect in availableEffects)
        {
            selection.options.Add(new TMP_Dropdown.OptionData() { text = effect });
        }

        // refreshing appereance
        selection.RefreshShownValue();
        PopulateAdditionalSettingsList();

        selection.onValueChanged.AddListener(delegate { SelectEffect(selection); });  // adding listener that changes selected effect
    }

    /* Sets the selected image effect */
    private void SelectEffect(TMP_Dropdown selection)
    {
        controlShaderScript.shaderActive = (ControlShader.ShaderActive)(selection.value - 1);
        PopulateAdditionalSettingsList();
    }

    /* Fills the additional settings section with the correct available settings*/
    private void PopulateAdditionalSettingsList()
    {

        // clearning additional settings
        ClearContent();

        // putting available setting in viewContent
        switch (controlShaderScript.shaderActive)
        {
            case ControlShader.ShaderActive.None:
                SetUpEmtpy();
                break;
            case ControlShader.ShaderActive.RadialBlur:
            case ControlShader.ShaderActive.RadialBlurDesat:
                SetUpRadial();
                break;
            case ControlShader.ShaderActive.GaussianBlur:
            case ControlShader.ShaderActive.Sharpening:
            case ControlShader.ShaderActive.HighPass:
                SetUpJustKernel();
                break;
            case ControlShader.ShaderActive.MotionBlur:
                SetUpMotionBlur();
                break;
            case ControlShader.ShaderActive.MotionField:
                SetUpJustMotionField();
                break;
            case ControlShader.ShaderActive.HighPassOnMotionField:
                SetUpHighPassMotion();
                break;
            case ControlShader.ShaderActive.MotionFieldOnHighPass:
                SetUpHighPassMotion();
                SetUpThreshold();
                break;
            case ControlShader.ShaderActive.Depth: //TODO: maybe check how colors could work
                SetUpEmtpy();
                break;
            case ControlShader.ShaderActive.HighPassOnDepth:
                SetUpJustKernel();
                break;
            case ControlShader.ShaderActive.MotionFieldOnHighPassOnDepth:
                SetUpHighPassMotion();
                SetUpThreshold();
                break;
        }
    }

    /* Returns a entry for the addtional settings*/
    private GameObject GetNewSettingObject()
    {
        return Instantiate(addOptSliderPrefab, scrollViewContent);
    }

    /* Clear all additional settings children in scrollViewContent*/
    private void ClearContent()
    {
        foreach (Transform child in scrollViewContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- ON CLICK METHODS ---

    #region On click

    /* Opens up option menu and stops time */
    public void OpenOptions()
    {
        Time.timeScale = 0f; // stopping time
        // disabeling movement
        player.GetComponent<PlayerMovement>().enabled = false;
        cam.GetComponent<MouseLook>().enabled = false;
        // adjusting interface
        objectMenuUI.SetActive(true);
        optionsBtn.SetActive(false);
    }

    /* Closes options menu and continues time */
    public void CloseOptions()
    {
        Time.timeScale = 1f; // continuing time
        // disabeling movement
        player.GetComponent<PlayerMovement>().enabled = true;
        cam.GetComponent<MouseLook>().enabled = true;
        // adjusting interface
        objectMenuUI.SetActive(false);
        optionsBtn.SetActive(true);
    }

    #endregion

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- HELPER ---

    #region Helper methods

    /* Sets up additional setting for threshold that is used for motion on high pass effects */
    private void SetUpThreshold()
    {
        Slider tmp = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Threshold", 0, 0.25f, 0.0075f, false);
        tmp.onValueChanged.AddListener(delegate { controlShaderScript.threshold = tmp.value; });  // adding listener that changes value if slider changes;
    }

    private void SetUpHighPassMotion()
    {
        Slider tmp = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, true);
        tmp.onValueChanged.AddListener(delegate { controlShaderScript.kernelSize = (int)tmp.value; });  // adding listener that changes value if slider changes;
        Slider tmp2 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Field", 1, 10, 1f, false);
        tmp2.onValueChanged.AddListener(delegate { controlShaderScript.scaleMotionField = tmp2.value; });  // adding listener that changes value if slider changes;
    }

    /* Sets up additional setting for motion field scale*/
    private void SetUpJustMotionField()
    {
        Slider tmp = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Field", 1, 10, 1f, false);
        tmp.onValueChanged.AddListener(delegate { controlShaderScript.scaleMotionField = tmp.value; });  // adding listener that changes value if slider changes;
    }

    /* Sets up motion blur additional settings */
    private void SetUpMotionBlur()
    {
        Slider tmp = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, true);
        tmp.onValueChanged.AddListener(delegate { controlShaderScript.kernelSize = (int)tmp.value; });  // adding listener that changes value if slider changes;
        Slider tmp2 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Blur", 0, 10, 5f, false);
        tmp2.onValueChanged.AddListener(delegate { controlShaderScript.scaleMotionBlur = tmp2.value; });  // adding listener that changes value if slider changes;
    }

    /* Sets up additional kernel setting */
    private void SetUpJustKernel()
    {
        Slider tmp = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, true);
        tmp.onValueChanged.AddListener(delegate { controlShaderScript.kernelSize = (int)tmp.value; });  // adding listener that changes value if slider changes;
    }

    /* Sets up additional setting that there are no additional settings */
    private void SetUpEmtpy()
    {
        GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("No additional settings available", -1, -1, -1, true, false);
    }

    /* Sets up additional settings for radial blur effects */
    private void SetUpRadial()
    {
        Slider tmp = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, true);
        tmp.onValueChanged.AddListener(delegate { controlShaderScript.kernelSize = (int)tmp.value; });  // adding listener that changes value if slider changes;
        if (controlShaderScript.XrActive) // TODO: does this matter? Example not intended for XR?
        {
            Slider tmp2 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin X Left Eye", 0, 1, 0.5f, false);
            tmp2.onValueChanged.AddListener(delegate { controlShaderScript.radialBlurOriginXLeftEye = tmp2.value; });  // adding listener that changes value if slider changes;
            Slider tmp3 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin Y Left Eye", 0, 1, 0.5f, false);
            tmp3.onValueChanged.AddListener(delegate { controlShaderScript.radialBlurOriginYLeftEye = tmp3.value; });  // adding listener that changes value if slider changes;
            Slider tmp4 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin X Right Eye", 0, 1, 0.5f, false);
            tmp4.onValueChanged.AddListener(delegate { controlShaderScript.radialBlurOriginXRightEye = tmp4.value; });  // adding listener that changes value if slider changes;
            Slider tmp5 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin Y Right Eye", 0, 1, 0.5f, false);
            tmp5.onValueChanged.AddListener(delegate { controlShaderScript.radialBlurOriginYRightEye = tmp5.value; });  // adding listener that changes value if slider changes;
        }
        else
        {
            Slider tmp6 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin X", 0, 1, 0.5f, false);
            tmp6.onValueChanged.AddListener(delegate { controlShaderScript.radialBlurOriginX = tmp6.value; });  // adding listener that changes value if slider changes;
            Slider tmp7 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin Y", 0, 1, 0.5f, false);
            tmp7.onValueChanged.AddListener(delegate { controlShaderScript.radialBlurOriginY = tmp7.value; });  // adding listener that changes value if slider changes;
        }
        Slider tmp8 = GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Radial", 0, 10, 5f, false);
        tmp8.onValueChanged.AddListener(delegate { controlShaderScript.scaleRadial = tmp8.value; });  // adding listener that changes value if slider changes;
    }

    #endregion
}
