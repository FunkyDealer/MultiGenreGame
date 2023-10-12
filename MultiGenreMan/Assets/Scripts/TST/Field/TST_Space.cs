using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Space : MonoBehaviour
{
    private Vector3 _topLeft;
    private Vector3 _topRight;
    private Vector3 _bottomLeft;
    private Vector3 _bottomRight;

    private int _width;
    private int _length;

    private float _size;

    private Vector3 _pos;

    private MeshRenderer _myMesh;
    private Color _originalColor;
    [SerializeField]
    private Color _selectedColor;
    [SerializeField]
    private Color _hoverColor;

    private void Awake()
    {

        _myMesh = GetComponent<MeshRenderer>();

        _originalColor = _myMesh.material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
       


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSpaceInfo(TST_SpaceInfo info)
    {
        this._width = info._width;
        this._length = info._length;
        this._size = info._size;
        this._pos = info._pos;
        this._topLeft = info._topLeft;
        this._topRight = info._topRight;
        this._bottomLeft = info._bottomLeft;
        this._bottomRight = info._bottomRight;


        transform.Rotate(Vector3.right, 90);

        //transform.localScale = new Vector3(_size, transform.localScale.y, _size);
        transform.localScale = new Vector3(_size, _size, transform.localScale.z);



    }

    public TST_SpaceInfo GetSpaceInfo()
    {
        return new TST_SpaceInfo(_pos, _width, _length, _size);
    }

    public TST_SpaceInfo OnClick()
    {
        _myMesh.material.color = _selectedColor;

        return new TST_SpaceInfo(_pos, _width, _length, _size);
    }

    public void OnHoverEnter()
    {

    }

    public void OnHoverExit()
    {

    }

    public void Deselect()
    {
        _myMesh.material.color = _originalColor;
    }


}
