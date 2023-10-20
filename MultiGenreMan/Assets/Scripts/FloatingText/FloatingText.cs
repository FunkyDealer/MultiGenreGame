using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private FloatingTexTManager _myManager;

    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private float _timeToKill;
    [SerializeField]
    private float _speed;

    RectTransform _myRect;
    float _alpha;

    Vector3 _ObjPosition; //position of object in the world

    Vector3 _offset = Vector3.zero;

    private void Awake()
    {
        _myRect = GetComponent<RectTransform>();

        _alpha = _text.color.a;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Kill());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        if (_ObjPosition != null) transform.position = Camera.main.WorldToScreenPoint(_ObjPosition);

        _offset += Vector3.up * Time.deltaTime * _speed;

        _myRect.position += _offset;

        _alpha -= (Time.deltaTime / _timeToKill);

        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _alpha);
    }

    public void SetText(FloatingTexTManager m, string t, Vector3 pos, float scale)
    {
        _ObjPosition = pos;
        _myManager = m;
        _text.text = t;

        _myRect.localScale = new Vector3(scale, scale, 1);
    }

    private IEnumerator Kill()
    {
        yield return new WaitForSeconds(_timeToKill);

        _myManager.DestroyText(this);
    }


}
