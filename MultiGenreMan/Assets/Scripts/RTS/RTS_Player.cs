using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_Player : MonoBehaviour
{
    private RTS_MainCamera _camera;

    private int _unitCount;
    private int _buildingCount;



    // Start is called before the first frame update
    void Start()
    {
        RTS_LevelManager.ChooseSpawnPoints(this);



    }

    // Update is called once per frame
    void Update()
    {
        




    }

    public void GetCamera(RTS_MainCamera mainCamera) => this._camera = mainCamera;
}
