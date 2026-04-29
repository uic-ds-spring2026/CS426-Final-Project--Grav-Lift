using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveObjects.PressurePlates; // needed to access the script
// This file is specifically for the last room of the game and is detecting if all buttons are pressed
public class RoomButtonManager : MonoBehaviour
{
    public PressurePlateScript[] buttons; 
    public DoorController[] linkedDoors; 

    private bool[] buttonStates; 
    private bool allButtonsPressed = false; 

    void Start()
    {
        buttonStates = new bool[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonStates[i] = false; 
        }
    }

    public void SetButtonState(int buttonIndex, bool isPressed)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonStates.Length)
        {
            return;
        }

        buttonStates[buttonIndex] = isPressed;
        CheckButtonStates();
    }

    private void CheckButtonStates()
    {

        allButtonsPressed = true;
        foreach (bool state in buttonStates)
        {
            if (!state)
            {
                allButtonsPressed = false;
                break;
            }
        }

        if (allButtonsPressed)
        {
            OpenDoors();
        }
        else
        {
            CloseDoors();
        }
    }

    private void OpenDoors()
    {
        foreach (DoorController door in linkedDoors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }

    private void CloseDoors()
    {
        foreach (DoorController door in linkedDoors)
        {
            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }
}