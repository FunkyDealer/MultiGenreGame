using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Interactible : MonoBehaviour
{

    [SerializeField]
    protected string _prompt;
    public string Prompt => _prompt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact(FPSPlayer p)
    {


    }


  

}
