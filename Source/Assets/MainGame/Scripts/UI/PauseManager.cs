using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] Volume globalVolume;

    
    private PlayerMovement2D player;
    private bool gameIsPaused = false;
    DepthOfField depthOfField;
    VolumeProfile volumeProfile;
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement2D>();
    }
    private void Start()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        volumeProfile = globalVolume.sharedProfile;
        if (!volumeProfile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(!gameIsPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }

        }
    }

    private void PauseGame()
    {
        depthOfField.active = true;
        player.DisableMovement();
        gameIsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }
    private void ResumeGame()
    {
        depthOfField.active = false;
        player.EnableMovement();
        gameIsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OnResumeButtonClicked()
    {
        ResumeGame();
    }
    public void OnOptionsButtonClicked()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
    public void OnControlsButtonClicked()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void OnBackButtonClicked()
    {
        pausePanel.SetActive(true);
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }
}
