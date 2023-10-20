using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTexTManager : MonoBehaviour
{
    private static FloatingTexTManager _instance;
    public static FloatingTexTManager inst { get { return _instance; } }

    [SerializeField]
    GameObject _textPrefab;

    Dictionary<int, FloatingText> _textList = new Dictionary<int, FloatingText>();

    float _maxScale = 1;
    float _minScale = 0.4f;
    float _minDistance = 50;
    float _maxDistance = 1;

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
        
    }

    public void CreateText(Vector3 position, string text, float delay)
    {

        StartCoroutine(MakeText(position, text, delay));



    }

    private IEnumerator MakeText(Vector3 position, string text, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 pos = Camera.main.WorldToScreenPoint(position);

        //change size with distance
        float dist = Vector3.Distance(position, Camera.main.transform.position);

        float scale = 1;
        scale = Mathf.Lerp(_minScale, _maxScale, Mathf.InverseLerp(_minDistance, _maxDistance, dist));

        GameObject g = Instantiate(_textPrefab, pos, Quaternion.identity, this.gameObject.transform);

        FloatingText t = g.GetComponent<FloatingText>();

        t.SetText(this, text, position, scale);

        _textList.Add(t.gameObject.GetInstanceID(), t);
    }

    public void DestroyText(FloatingText t)
    {
        _textList.Remove(t.gameObject.GetInstanceID());
        Destroy(t.gameObject);

    }
}
