using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARPG_UIController : MonoBehaviour
{
    private static ARPG_UIController _instance;
    public static ARPG_UIController inst { get { return _instance; } }

    [SerializeField]
    private TMP_Text _HealthNrText;
    [SerializeField]
    private TMP_Text _ManaNrText;

    [SerializeField]
    private GameObject _objectiveUIContainer;
    [SerializeField]
    private GameObject _objectiveTextContainer;

    [SerializeField]
    private GameObject _objectivePrefab;

    private List<GameObject> _objectives;

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

        _objectives = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(int newValue)
    {
        _HealthNrText.text = newValue.ToString();
    }

    public void UpdateMana(int newValue)
    {
        _ManaNrText.text = newValue.ToString(); 
    }

    public void AddObjective(string text)
    {
        GameObject o = Instantiate(_objectivePrefab, _objectiveTextContainer.transform);

        RectTransform rect = o.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector3(0, 0 + -30 * _objectives.Count, 0);

        TMP_Text t = o.GetComponentInChildren<TMP_Text>();

        t.text = text;

        _objectives.Add(o);
    }

}
