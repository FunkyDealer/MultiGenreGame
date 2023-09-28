using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_Player : MonoBehaviour
{
    private RTS_MainCamera _camera;

    [SerializeField]
    private int _team = 1;

    public int Team { get; private set; }
    private int _unitCount = 0;
    private int _buildingCount = 0;

    [SerializeField]
    private int _cameraPanSpeed = 20;

    private Vector2 _movementInput = Vector2.zero;

    private RTS_ClickableEntity _currentlySelectedEntity = null;

    [SerializeField]
    GameObject _cursorIndicator;

    // Start is called before the first frame update
    void Start()
    {
        RTS_LevelManager.ChooseSpawnPoints(this);

        Team = _team;

        StartGameBySpawningBuilder();
    }

    // Update is called once per frame
    void Update()
    {


        _movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * _movementInput.x + Vector3.right * _movementInput.y;

        //transform.position += direction * _cameraPanSpeed * Time.deltaTime;

        MoveCameraHorizontally(_movementInput.x);

        MoveCameraVertically(_movementInput.y);

        if (IsSomethingSelected())
        {
            _cursorIndicator.transform.position = _currentlySelectedEntity.transform.position + Vector3.up * 3;
        }


    }

    public void GetCamera(RTS_MainCamera mainCamera) => this._camera = mainCamera;

    public void MoveCameraHorizontally(float input) => transform.position += Vector3.right * input * _cameraPanSpeed* Time.deltaTime;

    public void MoveCameraVertically(float input) => transform.position += Vector3.forward * input * _cameraPanSpeed * Time.deltaTime;

    public void SelectEntity(RTS_ClickableEntity entity)
    {
        _currentlySelectedEntity = null;
        _currentlySelectedEntity = entity;
    }

    public void DeselectEntity()
    {
        _currentlySelectedEntity = null;
    }

    public void IssueOrder(RTS_PlayerOrder order)
    {
        switch (order)
        {
            case RTS_PlayerMoveOrder:
                IssueMoveOrder(order as RTS_PlayerMoveOrder);
                break;

            default:
                break;

        }


    }

    private void IssueMoveOrder(RTS_PlayerMoveOrder order)
    {
        _currentlySelectedEntity.ReceiveMoveOrder(order);


    }

    private void StartGameBySpawningBuilder()
    {


        RTS_ClickableEntity unit = Instantiate(RTS_GameManager.GetBuilderUnityPrefab, RTS_LevelManager.GetSpawnPoint(Team), Quaternion.identity).GetComponent<RTS_ClickableEntity>();
        unit.Team = Team;

    }

    public bool IsSomethingSelected() => _currentlySelectedEntity != null;
}
