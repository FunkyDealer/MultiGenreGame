using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GTS_DebugUi : MonoBehaviour
{
    private static GTS_DebugUi _instance;
    public static GTS_DebugUi inst { get { return _instance; } }


    Dictionary<string, TMP_Text> _debugLines;

    [SerializeField]
    private GameObject _linePrebab;
    [SerializeField]
    private GameObject _debugMaster;

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


        _debugLines = new Dictionary<string, TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DebugLine(string key, string text)
    {
        if (!AppManager.inst.Debug) return;
        if (!_debugLines.ContainsKey(key)) //create key
        { 
            TMP_Text newT = CreateLine(text);

            _debugLines.Add(key, newT);
        }
        else
        {
            _debugLines[key].text = text;

        }
    }

    private TMP_Text CreateLine(string text)
    {
        int ammount = _debugLines.Count;

        TMP_Text newLine;

        GameObject go = Instantiate(_linePrebab,Vector3.zero,Quaternion.identity, _debugMaster.transform);

        RectTransform r = go.GetComponent<RectTransform>();
        r.localPosition = new Vector3(0,-20 * ammount,0);

        newLine = go.GetComponent<TMP_Text>();
        newLine.text = text;

        return newLine;
    }


}
