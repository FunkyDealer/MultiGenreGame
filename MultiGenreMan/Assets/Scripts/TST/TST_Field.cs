using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Field : MonoBehaviour
{
    private static TST_Field _instance;
    public static TST_Field inst { get { return _instance; } }


    [SerializeField]
    private int _WIDTH;
    [SerializeField]
    private int _LENGTH;
    [SerializeField]
    private float _SIZE;
    [SerializeField]
    private GameObject _SPACEPREFAB;


    public static int _width { get; private set; }
    public static int _length { get; private set; }
    private static float _spaceSize;
    private static GameObject _spacePrefab;

    private static TST_Space[,] _grid;





    private void Awake()
    {
        _width = _WIDTH;
        _length = _LENGTH;
        _spaceSize = _SIZE;
        _spacePrefab = _SPACEPREFAB;


    }

    // Start is called before the first frame update
    void Start()
    {
        _grid = new TST_Space[_width, _length];


        for (int w = 0; w < _width; w++) //create grid
        {
            for (int l = 0; l < _length; l++)
            {
                _grid[w, l] = CreateSpace(w, l);


            }
        }

        TST_GameManager.StartUnits(); //tell game manager to start units and put them in the correct place
    }

    // Update is called once per frame
    void Update()
    {
        


    }

    private TST_Space CreateSpace(int width, int length)
    {
        Vector3 pos = new Vector3(transform.position.x + (width * _spaceSize) - (_width / 2 * _spaceSize), 0.02f, transform.position.z + (length * _spaceSize) - (_length / 2 * _spaceSize));

        TST_Space space = Instantiate(_spacePrefab, pos, Quaternion.identity, transform).GetComponent<TST_Space>();

        TST_SpaceInfo info = new TST_SpaceInfo(pos, width, length, _spaceSize);

        space.SetSpaceInfo(info);



        return space;
    }

    public static TST_SpaceInfo GetSpaceInfo(Vector2Int space)
    {
        return _grid[space.x, space.y].GetSpaceInfo();
    }

}
