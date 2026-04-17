using System.IO;
using TMPro;
using UnityEngine;

public class UserSave : MonoBehaviour
{
    public TextMeshProUGUI savePrompt;
    
    void Start()
    {
        if (UserSaveData.Instance.userName == null)
            UserSaveData.Instance.userName = "wodeshijie";
        savePrompt.gameObject.SetActive(false);
    }

    public void SaveData()
    {
        SaveFileCreateAndWrite();
        FadeOut();
    }
    
    void SaveFileCreateAndWrite()
    {
        FileStream fileStream = new FileStream(Application.persistentDataPath + "/playerInfo.txt", FileMode.Create);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.WriteLine("UserName:" + UserSaveData.Instance.userName);
        streamWriter.WriteLine("UserID:" + UserSaveData.Instance.userID);
        streamWriter.WriteLine("Score:" + UserSaveData.Instance.score);
        streamWriter.Close();
        fileStream.Close();
    }
 
    void FadeOut()
    {
        savePrompt.gameObject.SetActive(true);
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", 1,
            "to", 0,
            "onupdate", "UpdateAlpha",
            "time", 0.5f,
            "oncomplete", "DestroySelf",
            "ignoretimescale", true
        ));
    }

    void UpdateAlpha(float alphaValue)
    {
        Color tempColor = savePrompt.color;
        tempColor.a = alphaValue;
        savePrompt.color = tempColor;
    }

    void DestroySelf()
    {
        savePrompt.gameObject.SetActive(false);
    }
}
