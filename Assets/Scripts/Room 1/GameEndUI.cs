using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    [Header("Root panel")]
    public GameObject root;          

    [Header("Texts")]
    public TMP_Text titleText;     
    public TMP_Text bodyText;        

    [Header("Buttons")]
    public Button primaryButton;     
    public TMP_Text primaryLabel;

    public Button secondaryButton;   
    public TMP_Text secondaryLabel;

    void Awake()
    {
        if (root) root.SetActive(false);
    }

    public void ShowLose()
    {
        if (!root) return;
        root.SetActive(true);
        Time.timeScale = 1f; 

        if (titleText) titleText.text = "Time's up!";
        if (bodyText)
            bodyText.text =
                "You ran out of time.\n\n" +
                "You can restart from the very beginning,\n" +
                "or just stay here and look around (no interaction).";

        if (primaryLabel) primaryLabel.text = "Restart";
        if (secondaryLabel) secondaryLabel.text = "Continue";

        primaryButton.onClick.RemoveAllListeners();
        secondaryButton.onClick.RemoveAllListeners();

        primaryButton.onClick.AddListener(RestartGame);
        secondaryButton.onClick.AddListener(SpectateOnly);
    }

    public void ShowWin()
    {
        if (!root) return;
        root.SetActive(true);
        Time.timeScale = 1f;

        if (titleText) titleText.text = "You escaped!";
        if (bodyText)
            bodyText.text =
                "Congratulations, you solved all puzzles in time.\n\n" +
                "You can leave the game now, or continue to collect\n" +
                "your final prize.";

        if (primaryLabel) primaryLabel.text = "Exit game";
        if (secondaryLabel) secondaryLabel.text = "Continue to prize";

        primaryButton.onClick.RemoveAllListeners();
        secondaryButton.onClick.RemoveAllListeners();

        primaryButton.onClick.AddListener(ExitGame);
        secondaryButton.onClick.AddListener(ClosePanelOnly);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex);
    }

    void SpectateOnly()
    {
        if (root) root.SetActive(false);

        Time.timeScale = 0f; 

    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ClosePanelOnly()
    {
        if (root) root.SetActive(false);
    }
}
