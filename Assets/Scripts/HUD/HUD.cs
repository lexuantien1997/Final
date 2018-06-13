using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Image HealthUI;

    public HealthSystem playerHealth;

    public Text playerLife;

    public Text playerBullet;

    public Text playerBullet2;

    void Update()
    {
        GameObject gameObject = GameObject.Find("Main Character");

        MainCharacterScript mainCharacterScript = gameObject.GetComponent<MainCharacterScript>();

        playerLife.text = mainCharacterScript.life.ToString();

        playerBullet.text = mainCharacterScript.bullet.ToString();

        playerBullet2.text = mainCharacterScript.bullet2.ToString();
    }

    public void UpdateGUI()
    {

        if (playerHealth.currentHealth < 0)
        {
            playerHealth.currentHealth = 0;
        }
        HealthUI.rectTransform.localScale = new Vector2(10 * playerHealth.currentHealth / 100, HealthUI.rectTransform.localScale.y);

    }
}