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
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("No additional settings available", -1, -1, -1, -1, true, false);
                break;
            case ControlShader.ShaderActive.RadialBlur:
            case ControlShader.ShaderActive.RadialBlurDesat:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                if (controlShaderScript.XrActive) // TODO: does this matter? Example not intended for XR?
                {
                    GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin X Left Eye", 0, 1, 0.5f, controlShaderScript.radialBlurOriginXLeftEye, false);
                    GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin Y Left Eye", 0, 1, 0.5f, controlShaderScript.radialBlurOriginYLeftEye, false);
                    GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin X Right Eye", 0, 1, 0.5f, controlShaderScript.radialBlurOriginXRightEye, false);
                    GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin Y Right Eye", 0, 1, 0.5f, controlShaderScript.radialBlurOriginYRightEye, false);
                }
                else
                {
                    GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin X", 0, 1, 0.5f, controlShaderScript.radialBlurOriginX, false);
                    GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Radial Blur Origin Y", 0, 1, 0.5f, controlShaderScript.radialBlurOriginY, false);
                }
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Radial", 0, 10, 5f,controlShaderScript.scaleRadial, false);
                break;
            case ControlShader.ShaderActive.GaussianBlur:
            case ControlShader.ShaderActive.Sharpening:
            case ControlShader.ShaderActive.HighPass:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                break;
            case ControlShader.ShaderActive.MotionBlur:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Blur", 0, 10, 5f, controlShaderScript.scaleMotionBlur, false);
                break;
            case ControlShader.ShaderActive.MotionField:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Field", 1, 10, 1f,controlShaderScript.scaleMotionField, false);
                break;
            case ControlShader.ShaderActive.HighPassOnMotionField:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Field", 1, 10, 1f, controlShaderScript.scaleMotionField, false);
                break;
            case ControlShader.ShaderActive.MotionFieldOnHighPass:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Field", 1, 10, 1f, controlShaderScript.scaleMotionField, false);
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Threshold", 0, 0.25f, 0.0075f, controlShaderScript.threshold, false);
                break;
            case ControlShader.ShaderActive.Depth: //TODO: maybe check how colors could work
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("No additional settings available", -1, -1, -1, -1, true, false);
                break;
            case ControlShader.ShaderActive.HighPassOnDepth:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                break;
            case ControlShader.ShaderActive.MotionFieldOnHighPassOnDepth:
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Kernel size", 5, 127, 9, controlShaderScript.kernelSize, true);
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Scale Motion Field", 1, 10, 1f, controlShaderScript.scaleMotionField, false);
                GetNewSettingObject().GetComponent<AdditionalSettingsControl>().SetContent("Threshold", 0, 0.25f, 0.0075f, controlShaderScript.threshold, false);
                break;
        }
    }

    /* Returns a new slider for the addtional settings*/
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

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // --- ON CLICK METHODS ---
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
}
