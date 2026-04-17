using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSystem : MonoBehaviour
{
    public RectTransform startBackgroundImage;
    Button startButton;
    Button continueButton;
    Button exitButton;

    void Start()
    {
        startButton = startBackgroundImage.Find("B_Start").GetComponent<Button>();
        continueButton = startBackgroundImage.Find("B_Continue").GetComponent<Button>();
        exitButton = startBackgroundImage.Find("B_Exit").GetComponent<Button>();
        startButton.onClick.AddListener(Event_B_Start);
        continueButton.onClick.AddListener(Event_B_Continue);
        exitButton.onClick.AddListener(Event_B_Exit);
    }

    void Event_B_Start()
    {
        SceneManager.LoadScene(1);
    }

    void Event_B_Continue()
    {
        FileStream fileStream = new FileStream(Application.persistentDataPath + "/playerInfo.txt", FileMode.Open);
        StreamReader streamReader = new StreamReader(fileStream);
        string[] data = streamReader.ReadLine().Split(':');
        UserSaveData.Instance.userName = data[1];
        data = streamReader.ReadLine().Split(':');
        UserSaveData.Instance.userID = int.Parse(data[1]);
        data = streamReader.ReadLine().Split(':');
        UserSaveData.Instance.score = float.Parse(data[1]);
        Debug.Log(UserSaveData.Instance.score);
        fileStream.Close();
        streamReader.Close();
        SceneManager.LoadScene(1);
    }

    void Event_B_Exit()
    {
        Application.Quit();
    }
}
