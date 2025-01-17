using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public abstract class ABaseHealthBar : MonoBehaviour, IDamage
{

    public ABaseCharacter characterInfo;
    public ABaseCharacterData characterMaxHealth;
    //public float changeSpeed;
    //public float lerpTime;

    public Slider healthbar;
    public Image healthImage;
    public TextMeshProUGUI healthText;
    public float damage;
    public float health;
    public bool wasCritic;
    //public float maximumHealth = 1000;

    public event EventHandler OnDamaged;
    DamageDisplayEventArgs damageDisplayEventArgs;

    public bool takedDamage = false;

    private void Start()
    {
        damageDisplayEventArgs = new DamageDisplayEventArgs(damage);
    }
    /// <summary>
    /// Verilerin belli aral�ktan ba�ka aral��� ta��nmas�nda kullan�lan metottur
    /// </summary>
    /// <param name="oldmin">Eski aral���n minimum de�eri</param>
    /// <param name="oldmax">Eski aral���n maksimum de�eri</param>
    /// <param name="newmin">Yeni aral���n minimum de�eri</param>
    /// <param name="newmax">Yeni aral���n maksimum de�eri</param>
    /// <param name="value">Yeni aral��a ta��nacak de�er</param>
    /// <returns></returns>
    public float NormalizeData(float oldmin, float oldmax, float newmin, float newmax, float value)
    {
        return ((value - oldmin) / oldmax - oldmin) * (newmax - newmin) + newmin;
    }

    public void SetHealth(float value,Slider healthbarValue)
    {
        healthbarValue.value = value;
    }
    public void SetMaxHealth(float value, Slider healthbarMaxValue)
    {
        healthbarMaxValue.maxValue = value;
    }
    public void UpdateMaxHealth(float oldmaximumHealth, float newMaxHealth, float newMinHealth=0.0f)
    {
        health = healthbar.value;
        health = NormalizeData(0, oldmaximumHealth, newMinHealth, newMaxHealth, health);
        healthbar.maxValue = newMaxHealth;
        healthbar.value = health;
        healthText.text = health.ToString();

    }


    /// <summary>
    /// Hasar�n uyguland��� s�n�f
    /// </summary>
    /// <param name="amount">Hesaplanm�� hasar miktar�. Bu hasar miktar� kadar can azalacak</param>
    public void DamageApply(float amount, bool isCritic=false)
    {
        if (characterInfo.currentHealth >0)
        {


            //Debug.Log("damage");
            healthbar.value -= amount;
            wasCritic = isCritic;
            takedDamage = true;
            if (health < 0)
            {
                health = 0;
            }

            if (OnDamaged != null)
            {
                damage = amount;
                //damageDisplayEventArgs.DamageAmount = amount;
                OnDamaged(this, System.EventArgs.Empty);

            }

            takedDamage = false;


        }

    }
    /*
    public void Heal(float amount)
    {
        health += amount;
        if (health > maximumHealth)
        {
            health = maximumHealth;
        }
        //OnHealed?.Invoke(this, EventArgs.Empty);
    }
    */

    /// <summary>
    /// Can�n integer'a d�n��t�r�l� string olarak geri d�nd�r�r
    /// </summary>
    /// <param name="healthbar"></param>
    /// <returns></returns>
    public string BarValueAsIntegerDisplay(Slider healthbar)
    {
        return Mathf.CeilToInt(healthbar.value).ToString();
    }

    public string BarValueAsIntegerDisplay(float value)
    {
        return Mathf.CeilToInt(value).ToString();
    }

    public void UpdateCurrentHealthWithBarText(TextMeshProUGUI healthbar, ABaseCharacter characterHealth)
    {
        int.TryParse(healthbar.text, out characterHealth.currentHealth);
        //int.TryParse(healthText.text, out characterInfo.currentHealth);
    }

    public void UpdateCurrentHealthWithBarText(string healthbar, ABaseCharacter characterHealth)
    {
        int.TryParse(healthbar, out characterHealth.currentHealth);
    }

    public int BarValueAsIntegerDisplay(Slider healthbar, TextMeshProUGUI healthText)
    {
        int a = Mathf.CeilToInt(healthbar.value);
        healthText.SetText(a.ToString());
        return a;
    }
    public void UpdateCurrentHealthAndBarText(TextMeshProUGUI healthText, Slider healthbar, ABaseCharacter characterHealth)
    {
        healthText.text = BarValueAsIntegerDisplay(healthbar);
        UpdateCurrentHealthWithBarText(healthText.text, characterHealth);
    }

    //TODO bunu kald�rabilirim  Abasecharacter'da bir tane daha var

    /// <summary>
    ///     �yile�me miktar�n� hesaplamak i�in kullan�lan metottur.
    /// </summary>
    /// <param name="rawHealing">Ham iyile�me miktar� bunun �zerinden hesplama yap�lmaktad�r. �rne�in d��mana verilen hasar ham iyile�me miktar�d�r.
    /// Bunun �zerinden i�lem yap�lmal�, mesela istenirse %50'si al�nabilir.
    /// </param>
    /// <param name="targetGameObject">�yile�menin uygulanaca�� hedef</param>
    /// <param name="healingRatio">�yile�me oran�d�r. Ham iyile�me miktar�n�n ne kadar�n�n iyile�me olaca��n� belirler</param>
    /// <param name="healigMultiplier">�yile�meyi oransal olarak azalt�r veya artt�rabilir, -100 ile 100 aras�nda bir say� olmal�d�r. Varsay�lan 0'd�r.</param>
    public float Healing(float rawHealing, GameObject targetGameObject, float healingRatio, float healigMultiplier=0.0f)
    {
        Slider targetHealImage = targetGameObject.GetComponent<Slider>();
        float healthfill = targetHealImage.value;
        return healthfill + (rawHealing * (1 + healingRatio / 100) * (1 - healigMultiplier / 100));
    }

}