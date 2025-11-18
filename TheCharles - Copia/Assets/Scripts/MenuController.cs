using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject menuMusic;
    public GameObject OptionsMenu;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        menuMusic.SetActive(true);

        Time.timeScale = 0f;
        AudioManager.instance.PauseAll();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        menuMusic.SetActive(false);

        Time.timeScale = 1f;
        AudioManager.instance.ResumeAll();
    }

    public void GoToStart()
    {
        Time.timeScale = 1f;
        AudioManager.instance.StopAll();
        SceneManager.LoadScene("Diario");
    }

    public void Options()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(false);
        OptionsMenu.SetActive(true);

        AudioManager.instance.PauseAll();
    }

    public void noOptions()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
    public void Jogar()
    {
        SceneManager.LoadScene("Diario");
    }

}
