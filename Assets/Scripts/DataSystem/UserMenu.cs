using System;
using UnityEngine;
using UnityEngine.UI;

public class UserMenu : MonoBehaviour
{
    public RectTransform savePanel;
    UserSave userSave;
    Button saveButton;
    Button continueButton;
    Button exitButton;
    public Button menuButton;


    void Start()
    {
        savePanel.gameObject.SetActive(false);
        userSave = GetComponent<UserSave>();
        saveButton = savePanel.Find("B_Save").GetComponent<Button>();
        continueButton = savePanel.Find("B_Continue").GetComponent<Button>();
        exitButton = savePanel.Find("B_Exit").GetComponent<Button>();
        continueButton.onClick.AddListener(Event_B_Continue);
        saveButton.onClick.AddListener(Event_B_Save);
        exitButton.onClick.AddListener(Event_B_Exit);
        menuButton.onClick.AddListener(Event_B_Menu);
    }

    void Event_B_Save()
    {
        userSave.SaveData();
    }

    void Event_B_Continue()
    {
        GameplayPauseController.Resume(GameplayPauseReason.Menu);
        savePanel.gameObject.SetActive(false);
    }

    void Event_B_Exit()
    {
        Application.Quit();
    }
    
    void Event_B_Menu()
    {
        GameplayPauseController.Pause(GameplayPauseReason.Menu);
        savePanel.gameObject.SetActive(true);
    }
}
