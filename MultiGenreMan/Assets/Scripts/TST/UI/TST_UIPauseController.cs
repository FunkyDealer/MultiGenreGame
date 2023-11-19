using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TST_UIPauseController : MonoBehaviour
{
    private static TST_UIPauseController _instance;
    public static TST_UIPauseController inst { get { return _instance; } }

    private bool _ready = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (_ready && Input.GetButtonDown("Escape"))
        {
            UnpauseGame();
        }

    }

    public void EnableMenu()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(SetReady());
    }

    public void UnpauseGame()
    {
        _ready = false;
        DisableMenu();

        TST_GameManager.UnpauseGame();
    }

    private void DisableMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        TST_GameManager.UnpauseGame();
        TST_GameManager.EndGame();
        RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        TST_GameManager.UnpauseGame();
        TST_GameManager.EndGame();
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator SetReady()
    {
        yield return 0;

        _ready = true;
    }

}
