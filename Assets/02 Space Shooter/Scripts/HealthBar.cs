using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Reference the healthbar slider to set the maximum and current HP value
    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        SetHealth(health);
    }
    public void SetHealth(int health)
    {
        slider.value = health;
    }

}
