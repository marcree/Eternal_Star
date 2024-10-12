using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatBar : MonoBehaviour
{
    private Character currentCharacter;
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;

    private bool isRecovering;

    private void Update()
    {
        if(healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime*2;
        }
        if (isRecovering)
        {
            float percentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = percentage;

            if(percentage>=1)
            {
                isRecovering = false;
                return;
            }
        }

    }
    public void OnhealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;

    }
    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}
