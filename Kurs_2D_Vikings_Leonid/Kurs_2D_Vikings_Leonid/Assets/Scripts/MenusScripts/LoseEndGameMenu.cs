using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseEndGameMenu : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _restartButton;

    private void Start()
    {
        _closeButton.onClick.AddListener(Exit);
        _restartButton.onClick.AddListener(Restart);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
