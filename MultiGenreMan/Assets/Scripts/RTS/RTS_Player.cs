using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<int,RTS_ClickableEntity> _currentlySelectedEntities; //item.GetInstanceID() , item
    private int _SelectedEntitiesAmmount = 0;

    [SerializeField]
    GameObject _cursorIndicator;

    // Start is called before the first frame update
    void Start()
    {
        RTS_LevelManager.ChooseSpawnPoints(this);

        Team = _team;

        _currentlySelectedEntities = new Dictionary<int, RTS_ClickableEntity>();

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
            _cursorIndicator.transform.position = _currentlySelectedEntities.First().Value.transform.position + Vector3.up * 3;
        }


    }

    public void GetCamera(RTS_MainCamera mainCamera) => this._camera = mainCamera;

    public void MoveCameraHorizontally(float input) => transform.position += Vector3.right * input * _cameraPanSpeed* Time.deltaTime;

    public void MoveCameraVertically(float input) => transform.position += Vector3.forward * input * _cameraPanSpeed * Time.deltaTime;

    //return true if sucessful in selecting something, false if otherwise
    public bool TryToSelectEntities(List<GameObject> objects, bool adding)
    {
        if (objects.Count == 0)
        {
            //Debug.Log("List was Empty, couldn't select anything");
            DeselectAllEntities();
            return false;
        }

        List<RTS_ClickableEntity> entities = new List<RTS_ClickableEntity>();

        foreach (var o in objects)
        {
            RTS_ClickableEntity entity = o.GetComponent<RTS_ClickableEntity>();
           if (entity != null)
            {
                entities.Add(entity);
            }
        }

        int entitesSize = entities.Count;
        
        if (entitesSize == 0)
        {
            //deselect all units
            DeselectAllEntities();
            return false;
        }
        else if (entitesSize == 1)
        {
            if (!adding) SelectSingleEntity(entities[0]);

            return true;
        }
        else if (entitesSize > 1)
        {
            if (!adding) SelectMultipleEntities(entities);

            return true;
        }


        return false;
    }


    public void SelectSingleEntity(RTS_ClickableEntity entity)
    {
        DeselectAllEntities();
        _currentlySelectedEntities.Add(entity.GetInstanceID() ,entity);
        _SelectedEntitiesAmmount++;
    }

    public void SelectMultipleEntities(List<RTS_ClickableEntity> entities)
    {
        DeselectAllEntities();

        foreach (var e in entities)
        {
            _currentlySelectedEntities.Add(e.GetInstanceID(), e);

        }

        _SelectedEntitiesAmmount = _currentlySelectedEntities.Count;
    }

    public void DeselectAllEntities()
    {
        _currentlySelectedEntities.Clear();
        _SelectedEntitiesAmmount = 0;

        _cursorIndicator.transform.position = new Vector3(0, -5, 0);
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
        foreach (var unit in _currentlySelectedEntities)
        {
            unit.Value.ReceiveMoveOrder(order);
        }

        


    }

    private void StartGameBySpawningBuilder()
    {


        RTS_ClickableEntity unit = Instantiate(RTS_GameManager.GetBuilderUnityPrefab, RTS_LevelManager.GetSpawnPoint(Team), Quaternion.identity).GetComponent<RTS_ClickableEntity>();
        unit.Team = Team;

    }

    public bool IsSomethingSelected() => _SelectedEntitiesAmmount > 0;
}
