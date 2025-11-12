using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private void ChangeScene(string sceneName)
    {
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }

    public void OnClickTitleButton()
    {
        ChangeScene("MainScene");
    }
}
