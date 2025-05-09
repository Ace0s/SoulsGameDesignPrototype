using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    public GameObject root; // The whole UI panel to enable/disable
    public Slider healthBar;
    public TextMeshProUGUI bossNameText;

    void Start()
    {
        root.SetActive(false);
    }

    public void UpdateHealth(int current, int max)
    {
        healthBar.value = (float)current / max;
    }

    public void ShowBossName(string bossName)
    {
        bossNameText.text = bossName;
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
