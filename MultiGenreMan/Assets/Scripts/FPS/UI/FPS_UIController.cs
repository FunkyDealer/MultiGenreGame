using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS_UIController : MonoBehaviour
{
    private static FPS_UIController _instance;
    public static FPS_UIController inst { get { return _instance; } }

    [SerializeField]
    private TMP_Text _HealthNrText;
    [SerializeField]
    private TMP_Text _ArmourNrText;

    [SerializeField]
    GameObject _ammoCounter;
    [SerializeField]
    private TMP_Text _ammoNrText;

    [SerializeField]
    GameObject _promptDisplay;
    [SerializeField]
    TMP_Text _promptDisplayText;

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
        _ammoCounter.SetActive(false);
        _promptDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartAmmoCounter(int ammo)
    {
        _ammoCounter.SetActive(true);
        UpdateAmmoCounter(ammo);
    }

    public void StopAmmoCounter(int ammo)
    {
        _ammoCounter.SetActive(false);
        UpdateAmmoCounter(ammo);
    }

    public void UpdateAmmoCounter(int ammo)
    {
        _ammoNrText.text = ammo.ToString();
    }

    public void UpdateHealth(int newValue)
    {
        _HealthNrText.text = newValue.ToString();
    }

    public void UpdateArmour(int newValue)
    {
        _ArmourNrText.text = newValue.ToString();
    }

    public void UpdatePromtDisplayer(string s)
    {
        if (s == null)
        {
            _promptDisplay.SetActive(false);
        }
        else
        {
            _promptDisplay.SetActive(true);
            _promptDisplayText.text = s;
        }   
    }

    public void DeactivateHUD()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateHUD()
    {
        this.gameObject.SetActive(true);
    }
}
