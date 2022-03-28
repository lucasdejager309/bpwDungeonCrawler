using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public bool smoothFill = true;
    public float animationTime = 0.4f;
    Image background;
    Image slider;
    Image border;
    
    [SerializeField] float maxValue;
    [SerializeField] float currentValue;

    void Start() {
        background = gameObject.transform.GetChild(0).GetComponent<Image>();
        slider = gameObject.transform.GetChild(1).GetComponent<Image>();
        border = gameObject.transform.GetChild(2).GetComponent<Image>();
    }

    public void SetActive(bool state) {
        if (background == null) background = gameObject.transform.GetChild(0).GetComponent<Image>();
        if (slider == null) slider = gameObject.transform.GetChild(1).GetComponent<Image>();
        if (border == null) border = gameObject.transform.GetChild(2).GetComponent<Image>();
        
        background.enabled = slider.enabled = border.enabled = state;
    }

    public void SetMaxValue(float value) {
        maxValue = value;
    }

    public bool SetValue(float value) {
        if (value <= maxValue && value > 0) {
            currentValue = value;
            
            if (smoothFill) {
                StartCoroutine(UpdateSlider(currentValue));
            }  else {
                slider.fillAmount = (1/maxValue)*currentValue;
            }
            
            
            return true;
        }

        return false;
    }

    public float GetValue() {
        return currentValue;
    }

    public void ModifyValue(float amount) {
        if (currentValue + amount > 0 && currentValue + amount < maxValue) {
            SetValue(currentValue + amount);
        } else if (currentValue + amount < 0) {
            SetValue(0);
        } else if (currentValue + amount > maxValue) {
            SetValue(maxValue);
        }
    }

    IEnumerator UpdateSlider(float newValue) {
        if (slider != null) {
            float fromAmount = slider.fillAmount;
            float endAmount = (1/maxValue)*newValue;
                
            for (float t = 0f; t < 1; t += Time.deltaTime/animationTime) {
                slider.fillAmount = Mathf.Lerp(fromAmount, endAmount, t);
                yield return null;
            }
        }
        else yield return null;
    }
}