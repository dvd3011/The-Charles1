using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject menuMusic;
    public GameObject OptionsMenu;

    public GameObject painelTutorial;

    private AudioZoneManager audioZoneManager;

    // Estados salvos ao pausar
    private bool bgWasActive;
    private bool musicWasActive;
    private bool wallaWasActive;

    void Start()
    {
        pauseMenu.SetActive(false);
        audioZoneManager = FindObjectOfType<AudioZoneManager>();
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        menuMusic.SetActive(true);

        Time.timeScale = 0f;

        // Salva estado atual dos sons para restaurar depois
        bgWasActive = audioZoneManager.objBG.activeSelf;
        musicWasActive = audioZoneManager.objMusic.activeSelf;
        wallaWasActive = audioZoneManager.objWalla.activeSelf;

        // Desliga todos os sons no PAUSE
        audioZoneManager.objBG.SetActive(false);
        audioZoneManager.objMusic.SetActive(false);
        audioZoneManager.objWalla.SetActive(false);

        AudioManager.instance.PauseAll();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        menuMusic.SetActive(false);

        Time.timeScale = 1f;
        AudioManager.instance.ResumeAll();

        // Restaura o estado de som EXATAMENTE como estava antes do pause
        audioZoneManager.objBG.SetActive(bgWasActive);
        audioZoneManager.objMusic.SetActive(musicWasActive);
        audioZoneManager.objWalla.SetActive(wallaWasActive);
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
    public void Tutorial()
    {
        painelTutorial.SetActive(true);
    }
    public void OkTutorial()
    {
        painelTutorial.SetActive(false);

    }
    public void Jogar()
    {
        SceneManager.LoadScene("Diario");
    }
}
