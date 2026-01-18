using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening; // FREE: Window → Package Manager → Search "DOTween" → Install

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioSource musicSource;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        FadeIn();
    }

    private void FadeIn()
    {
        mainMenuPanel.GetComponent<CanvasGroup>().alpha = 0;
        mainMenuPanel.GetComponent<CanvasGroup>().DOFade(1, 1f);
    }

    public void PlayButtonSound()
    {
        if (buttonSound) AudioSource.PlayClipAtPoint(buttonSound, Camera.main.transform.position);
    }

    public void NewGame()
    {
        PlayButtonSound();
        mainMenuPanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
            SceneManager.LoadScene("Game"));
    }

    public void LoadGame()
    {
        PlayButtonSound();
        PlayerPrefs.SetInt("LoadGame", 1);
        NewGame();
    }

    public void QuitGame()
    {
        PlayButtonSound();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}