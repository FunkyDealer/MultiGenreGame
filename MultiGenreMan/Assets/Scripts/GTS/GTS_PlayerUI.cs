using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GTS_PlayerUI : MonoBehaviour
{
    private static GTS_PlayerUI _instance;
    public static GTS_PlayerUI inst { get { return _instance; } }

    [SerializeField]
    private TMP_Text _healthTextDisplay;
    [SerializeField] private TMP_Text _primaryAmmoTextDisplay;
    [SerializeField] private TMP_Text _secondaryAmmoTextDisplay;

    [SerializeField] private GameObject _primaryAmmoObject;
    [SerializeField] private GameObject _secondaryAmmoObject;

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

        _primaryAmmoObject.SetActive(false);
        _secondaryAmmoObject.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateHealthTextDisplay(int value)
    {
        if (value < 0) value = 0;

        _healthTextDisplay.text = value.ToString();


    }

    public void UpdatePrimaryAmmoDisplay(int value)
    {
        if (value < 0) value = 0;

        if (!_primaryAmmoObject.active) _primaryAmmoObject.SetActive(true);

        _primaryAmmoTextDisplay.text = value.ToString();


    }

    public void UpdateSecondaryAmmoDisplay(int value)
    {
        if (value < 0) value = 0;

        if (!_secondaryAmmoObject.active) _secondaryAmmoObject.SetActive(true);

        _secondaryAmmoTextDisplay.text = value.ToString();


    }

}
