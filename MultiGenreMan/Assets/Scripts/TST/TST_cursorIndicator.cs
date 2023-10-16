using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_cursorIndicator : MonoBehaviour
{

    private float _size;

    // Start is called before the first frame update
    void Start()
    {
        _size = TST_Field.GetSpaceSize();
        transform.localScale = new Vector3(_size, _size, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetGridInfo(int size)
    {
        _size = size;

        transform.localScale = new Vector3(_size, _size, transform.localScale.z);

    }

    public void GoToSpace(Vector3 space)
    {
        transform.position = new Vector3(space.x, space.y + 0.1f, space.z);
    }

}
