/* CS 426 Final Project (Grav Lift)
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages the pause menu and pausing the game
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PAUSEMENU : MonoBehaviour {
    /// <summary>
    /// tracks whether the game is paused
    /// </summary>
    private static bool is_paused;
    /// <summary> 
    /// stores the pause screen canvas as a game object
    /// </summary>
    public GameObject pause_canvas;
    public GameObject options_menu;
    public GameObject controls_menu;
    public GameObject loading_menu;
    public GameObject death_screen;
    public Slider volume_slider;
    public TextMeshProUGUI volume_text;
    public Slider sens_slider;
    public TextMeshProUGUI sens_text;
    public PlayerMovement player;

    public void Start() {
        is_paused = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float saved_volume = PlayerPrefs.GetFloat("GlobalVolume", 1.0f);
        AudioListener.volume = saved_volume;
        if (volume_slider != null) {
            volume_slider.value = saved_volume;
            UpdateVolumeText(saved_volume);
            volume_slider.onValueChanged.AddListener(SetVolume);
        }

        float saved_sens = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f);
        if (player != null) {
            player.SetSens(saved_sens);
        }
        if (sens_slider != null) {
            sens_slider.value = saved_sens;
            UpdateSensText(saved_sens);
            sens_slider.onValueChanged.AddListener(SetSens);
        }
    }

    // Update is called once per frame
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (is_paused) {
                if ((options_menu != null && options_menu.activeSelf)
                 || (controls_menu != null && controls_menu.activeSelf)) {
                    Back();
                } else {
                    Play();
                }
            } else if (!(death_screen != null && death_screen.activeSelf)
                   || (loading_menu != null && loading_menu.activeSelf)) {
                Pause();
            }
        }
    }

    /// <summary>
    /// resumes the game and stops displaying the pause screen
    /// </summary>
    public void Play() {
        pause_canvas.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        is_paused = false;

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioS in allAudioSources) {
            audioS.UnPause();
        }
    }

    /// <summary>
    /// pauses the game and displays the pause screen
    /// </summary>
    public void Pause() {
        pause_canvas.SetActive(true);
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        is_paused = true;

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioS in allAudioSources) {
            audioS.Pause();
        }
    }

    public void Back()
    {
        if (options_menu != null) {
            options_menu.SetActive(false);
        }
        if (controls_menu != null) {
            controls_menu.SetActive(false);
        }
        if (pause_canvas != null) {
            pause_canvas.SetActive(true);
        }
    }

    /// <summary>
    /// stops the game and returns to the main menu
    /// </summary>
    public void MainMenu() {
        SceneManager.LoadScene("MAIN MENU");
    }

    /// <summary>
    /// getter function of bool is_paused
    /// </summary>
    /// <returns> whether the game is paused </returns>
    public bool IsPaused() {
        return is_paused;
    }

    public void SetVolume(float volume) {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GlobalVolume", volume);
        UpdateVolumeText(volume);
    }

    private void UpdateVolumeText(float volume) {
        if (volume_text != null) {
            volume_text.text = "Volume: " + Mathf.RoundToInt(volume * 100) + "%";
        }
    }

    public void SetSens(float sens) {
        if (player != null) {
            player.SetSens(sens);
        }
        PlayerPrefs.SetFloat("MouseSensitivity", sens);
        UpdateSensText(sens);
    }

    private void UpdateSensText(float sens) {
        if (sens_text != null) {
            sens_text.text = "Sensitivity: " + Mathf.RoundToInt(sens * 100) + "%";
        }
    }
}