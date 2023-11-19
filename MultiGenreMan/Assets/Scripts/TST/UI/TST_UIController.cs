using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TST_UIController : MonoBehaviour
{
    private static TST_UIController _instance;
    public static TST_UIController inst { get { return _instance; } }

    [SerializeField]
    private GameObject _unitTextBox;
    [SerializeField]
    private TMP_Text _UnitNameText;
    [SerializeField]
    private TMP_Text _UnitHPText;
    [SerializeField]
    private TMP_Text _UnitMovesText;


    [SerializeField]
    private GameObject _turnIndicatorBox;
    [SerializeField]
    private TMP_Text _turnIndicatorText;

    [SerializeField]
    private GameObject _turnInfo;
    [SerializeField]
    private TMP_Text _turnInfoText;

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


        _unitTextBox.SetActive(false);
        _turnIndicatorBox.SetActive(false);

        _turnInfo.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUnitTextBox(TST_Unit unit)
    {
        _unitTextBox.SetActive(true);
        _UnitNameText.text = unit.gameObject.name;

        _UnitHPText.text = new string($"HP: {unit.GetHealth}");
        _UnitMovesText.text = new string($"Moves: {unit.MovesLeft}");
    }

    public void UpdateUnitTextBox(TST_Unit unit)
    {
        if (unit != null)
        {
            _UnitHPText.text = new string($"HP: {unit.GetHealth}");
            _UnitMovesText.text = new string($"Moves: {unit.MovesLeft}");
        }
    }

    public void SetTurnIndicator(int player)
    {
        IndicateTurnChange(player);

        _turnIndicatorBox.SetActive(true);
        if (player == 1) _turnIndicatorText.text = new string($"Player {player} (You)");
        else _turnIndicatorText.text = new string($"Player {player} (Enemy)");
    }

    private void IndicateTurnChange(int player)
    {
        _turnInfo.SetActive(true);
        if (player == 1) _turnInfoText.text = new string($"Your Turn");
        else _turnInfoText.text = new string($"Enemy Turn");


        StartCoroutine(DisableTurnChangeIndicator());
    }


    private IEnumerator DisableTurnChangeIndicator()
    {
        yield return new WaitForSeconds(1.3f);


        _turnInfo.SetActive(false);

    }

    public void DisableUI()
    {
        this.gameObject.SetActive(false);
    }

    public void EnableUI()
    {
        this.gameObject.SetActive(true);
    }
}
