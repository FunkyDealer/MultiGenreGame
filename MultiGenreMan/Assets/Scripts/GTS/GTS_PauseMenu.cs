using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GTS_PauseMenu : MonoBehaviour
{
    private static GTS_PauseMenu _instance;
    public static GTS_PauseMenu inst { get { return _instance; } }


    private GTS_Player _player;

    [SerializeField]
    private GameObject _agregator;

    bool _canUnpause = false;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_canUnpause && Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonUnpause();
        }
    }

    public void Pause(GTS_Player p)
    {
        this._player = p;

        _agregator.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        _canUnpause = false;

        StartCoroutine(pauseTimer());
    }

    public void ButtonUnpause()
    {
        _player.Unpause();

      
    }

    public void UnPause()
    {
        _agregator.SetActive(false);
        _canUnpause = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator pauseTimer()
    {
        yield return new WaitForSeconds(0.5f);

        _canUnpause = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
