using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GoToFPSGame()
    {
        SceneManager.LoadScene("FPSTestLevel");
    }

    public void GoToARPGGame()
    {
        SceneManager.LoadScene("ARPG_TestLevel");
    }

    public void GoToTacticsGame()
    {
        SceneManager.LoadScene("TSTTestLevel");
    }
}
