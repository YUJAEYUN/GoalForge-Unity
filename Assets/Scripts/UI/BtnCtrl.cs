using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnCtrl : MonoBehaviour
{
    // 버튼 클릭 시 호출할 함수들
    public void OnClickNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
     public void OnClickTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
    public void OnClickManual()
    {
        SceneManager.LoadScene("ManualScene");
    }
        public void OnClickMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void OnClickExit()
    {
        Debug.Log("Exit Game");

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
