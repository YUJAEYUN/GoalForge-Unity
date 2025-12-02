using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonHandler : MonoBehaviour
{
    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}