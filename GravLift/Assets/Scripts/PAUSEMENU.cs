/* CS 426 Final Project
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages the pause menu and pausing the game
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PAUSEMENU : MonoBehaviour {
    /// <summary>
    /// tracks whether the game is paused
    /// </summary>
    private static bool is_paused;
    /// <summary> 
    /// stores the pause screen canvas as a game object
    /// </summary>
    public GameObject pause_canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start() {
        is_paused = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (is_paused) {
                Play();
            } else {
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
}