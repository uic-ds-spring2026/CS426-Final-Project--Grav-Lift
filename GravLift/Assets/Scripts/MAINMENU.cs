/* CS 426 Final Project (Grav Lift)
 * Group members: Rafael Maatouk, Fernando Lopez, Andrew Yoe
 * Description: Script that manages the main menu and starting the game
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MAINMENU : MonoBehaviour {
    /// <summary>
    /// starts the game by loading the game scene
    /// </summary>
    public void Play() {
        SceneManager.LoadScene("GAME");
    }

    /// <summary>
    /// exits the game
    /// </summary>
    public void Quit() {
        Application.Quit();
        Debug.Log("Player has quit the game");
    }
}