using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    public bool smoothFill = true;
    public float animationTime = 0.4f;
    Image slider;
    
    [SerializeField] float maxValue;
    [SerializeField] float currentValue;

    void Start() {
        slider = gameObject.transform.GetChild(1).GetComponent<Image>();
    }

    public void SetMaxValue(float value) {
        maxValue = value;
    }

    public bool SetValue(float value) {
        if (value <= maxValue && value > 0) {
            currentValue = value;
            StartCoroutine(UpdateSlider(value));
            
            return true;
        }

        return false;
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

    IEnumerator UpdateSlider(float newAmount) {
        if (slider != null) {
            if (smoothFill) {
            float fromAmount = slider.fillAmount;
            float endAmount = (1/maxValue)*currentValue;
            
            for (float t = 0f; t < 1; t += Time.deltaTime/animationTime) {
                slider.fillAmount = Mathf.Lerp(fromAmount, endAmount, t);
                yield return null;
            }
            } else {
                    slider.fillAmount = (1/maxValue)*currentValue;
            }
        }
        else yield return null;
    }
}