using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartHealthUI : MonoBehaviour
{
    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    
    [Header("Heart Images (Drag ALL 5 here)")]
    [SerializeField] private List<Image> heartImages = new List<Image>();

    private Health playerHealth;
    private int lastHealth = -1;

    private void Start()
    {
        // YOUR EXISTING Health.cs compatible!
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("HeartHealthUI: No Player with Health found!");
        }
        else
        {
            UpdateHearts();
        }
    }

    private void Update()
    {
        if (playerHealth == null || playerHealth.CurrentHealth == lastHealth) return;
        
        UpdateHearts();
        lastHealth = playerHealth.CurrentHealth;
    }

    private void UpdateHearts()
    {
        if (playerHealth == null) return;

        int current = playerHealth.CurrentHealth;
        int max = playerHealth.MaxHealth;

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < max) // Show up to max health
            {
                heartImages[i].sprite = (i < current) ? fullHeartSprite : emptyHeartSprite;
                heartImages[i].enabled = true;
            }
            else
            {
                heartImages[i].enabled = false; // Hide extras
            }
        }
    }
}