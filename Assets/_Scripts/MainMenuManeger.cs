using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panelleri")]
    [SerializeField] private GameObject creditsPanel;


    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }


    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkıldı!");
        Application.Quit();
    }



    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }


    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
}