using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventoryPanelUI inventoryPanelUI;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private GameObject[] alwaysHideWhenNotPaused;

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
        inventoryPanel.SetActive(false);

        foreach (var decor in alwaysHideWhenNotPaused)
        if (decor != null)
            decor.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        inventoryPanel.SetActive(false);

        // This is the important part — hide/show every time we toggle
        if (alwaysHideWhenNotPaused != null)
        {
            foreach (var decor in alwaysHideWhenNotPaused)
            {
                if (decor != null) // safety check
                    decor.SetActive(isPaused);
            }
        }

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        if (isPaused) pausePanel.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
    }

    public void Resume() { PlaySound(); TogglePause(); }

    public void OpenInventory()
    {
        PlaySound();
        inventoryPanel.SetActive(true);
        pausePanel.SetActive(false);

        var invUI = inventoryPanel.GetComponent<InventoryPanelUI>();
        if (invUI != null)
            invUI.RefreshInventory();
        else
            Debug.LogError("No InventoryUI component found on InventoryPanel!");
    }

    public void SaveGame()
    {
        PlaySound();
        PlayerPrefs.SetInt("Coins", PlayerInventory.Instance.GetCoinCount());
        PlayerPrefs.SetInt("Saved", 1);
        Debug.Log("<color=green>Game Saved!</color>");
    }

    public void QuitToMenu()
    {
        PlaySound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void PlaySound()
    {
        if (buttonSound) AudioSource.PlayClipAtPoint(buttonSound, Camera.main.transform.position);
    }
}

