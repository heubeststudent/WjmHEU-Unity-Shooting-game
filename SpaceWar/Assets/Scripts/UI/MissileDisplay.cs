using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MissileDisplay : MonoBehaviour
{
    static Text amountText;
    static Image cooldownImage;

    void Awake()
    {
        amountText = transform.Find("Amount Text").GetComponent<Text>();
        cooldownImage = transform.Find("Cooldown Image").GetComponent<Image>();
    }

    public static void UpdateAmountText(int amount) => amountText.text = amount.ToString();

    public static void UpdateCooldownImage(float fillAmount) => cooldownImage.fillAmount = fillAmount;
}
