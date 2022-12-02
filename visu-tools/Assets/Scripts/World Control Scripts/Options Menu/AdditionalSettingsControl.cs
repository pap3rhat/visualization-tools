using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AdditionalSettingsControl : MonoBehaviour
{
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text minVal;
    [SerializeField] private TMP_Text maxVal;


    [SerializeField] private Slider slider;

    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /* Sets up additional settings element
     * descriptionString: Name of the setting
     * minVal: minimal values setting can have
     * maxVal: maximal value setting can have
     * defaultVal: default value of setting
     * wholeNumbers: Can only whole numbers be used on slider?
     * showSlider: Should the slider even be shown or is description enough? (Used from "No settings" setting)
     * returns: the slider, so listenre can be added in other script that knows which values in controlshaderscipt should be changed
     */
    public Slider SetContent(string descriptionString, float minVal, float maxVal, float defaultVal, bool wholeNumbers, bool showSlider = true)
    {
        // setting gui text
        description.text = descriptionString;

        // setting gui slider
        if (showSlider)
        {
            slider.minValue = minVal;
            slider.maxValue = maxVal;
            slider.value = defaultVal;
            slider.wholeNumbers = wholeNumbers;
            this.minVal.text = minVal.ToString();
            this.maxVal.text = maxVal.ToString();
        }
        else // not showing slider if wanted 
        {
            slider.gameObject.SetActive(false);
            this.minVal.enabled = false;
            this.maxVal.enabled = false;
        }

        return slider;
    }
}
