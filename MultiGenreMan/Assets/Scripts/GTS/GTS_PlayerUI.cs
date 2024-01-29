using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GTS_PlayerUI : MonoBehaviour
{
    private static GTS_PlayerUI _instance;
    public static GTS_PlayerUI inst { get { return _instance; } }

    [SerializeField]
    private TMP_Text _healthTextDisplay;
    [SerializeField] private TMP_Text _primaryAmmoTextDisplay;
    [SerializeField] private TMP_Text _secondaryAmmoTextDisplay;

    [SerializeField] private GameObject _primaryAmmoObject;
    [SerializeField] private GameObject _secondaryAmmoObject;

    [SerializeField] private GameObject _frontDamageIndicator;
    [SerializeField] private GameObject _backDamageIndicator;
    [SerializeField] private GameObject _rightDamageIndicator;
    [SerializeField] private GameObject _leftDamageIndicator;

    [SerializeField] RectTransform myLockOnCollider;

    [SerializeField] private GameObject EnemyRecParent;
    [SerializeField] private GameObject EnemyRecObj;

    [SerializeField] private RectTransform _myCrossHair;

    [SerializeField] private Color _lockedOnColor;
    [SerializeField] private Color _notLockedOnColor;
    [SerializeField] private Color _lockingOnColor;


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

        _primaryAmmoObject.SetActive(false);
        _secondaryAmmoObject.SetActive(false);

        _frontDamageIndicator.SetActive(false);
        _backDamageIndicator.SetActive(false);
        _rightDamageIndicator.SetActive(false);
        _leftDamageIndicator.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void UpdateHealthTextDisplay(int value)
    {
        if (value < 0) value = 0;

        _healthTextDisplay.text = value.ToString();


    }

    public void UpdatePrimaryAmmoDisplay(int value)
    {
        if (value < 0) value = 0;

        if (!_primaryAmmoObject.active) _primaryAmmoObject.SetActive(true);

        _primaryAmmoTextDisplay.text = value.ToString();


    }

    public void UpdateSecondaryAmmoDisplay(int value)
    {
        if (value < 0) value = 0;

        if (!_secondaryAmmoObject.active) _secondaryAmmoObject.SetActive(true);

        _secondaryAmmoTextDisplay.text = value.ToString();
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        enemies.Clear();
        this.gameObject.SetActive(false);
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public void DamageIndicator(Transform me, Vector3 damageDirection)
    {
        float desiredAngle = 45;


        float angle = Vector3.Angle(me.forward, damageDirection);

        if (angle <= desiredAngle)
        {
            StopCoroutine(TurnOffFrontDamageIndicator());
            _frontDamageIndicator.SetActive(true);
            StartCoroutine(TurnOffFrontDamageIndicator());
        }
        else
        {
            angle = Vector3.Angle(me.right, damageDirection);

            if (angle <= desiredAngle)
            {
                StopCoroutine(TurnOffRightDamageIndicator());
                _rightDamageIndicator.SetActive(true);
                StartCoroutine(TurnOffRightDamageIndicator());
            }
            else
            {
                angle = Vector3.Angle(-me.right, damageDirection);

                if (angle <= desiredAngle)
                {
                    StopCoroutine(TurnOffLeftDamageIndicator());
                    _leftDamageIndicator.SetActive(true);
                    StartCoroutine(TurnOffLeftDamageIndicator());
                }
                else
                {
                    angle = Vector3.Angle(-me.forward, damageDirection);

                    if (angle <= desiredAngle)
                    {
                        StopCoroutine(TurnOffBackDamageIndicator());
                        _backDamageIndicator.SetActive(true);
                        StartCoroutine(TurnOffBackDamageIndicator());
                    }
                    else
                    {
                        Debug.Log("The angle wasn't in any direction?");
                    }
                }
            }
        }
    }

    private IEnumerator TurnOffFrontDamageIndicator()
    {
        yield return new WaitForSeconds(2f);

        _frontDamageIndicator.SetActive(false);
    }

    private IEnumerator TurnOffBackDamageIndicator()
    {
        yield return new WaitForSeconds(2f);

        _backDamageIndicator.SetActive(false);
    }

    private IEnumerator TurnOffRightDamageIndicator()
    {
        yield return new WaitForSeconds(2f);

        _rightDamageIndicator.SetActive(false);
    }

    private IEnumerator TurnOffLeftDamageIndicator()
    {
        yield return new WaitForSeconds(2f);

        _leftDamageIndicator.SetActive(false);
    }

    Dictionary<int, Tuple<RectTransform, RawImage>> enemies = new Dictionary<int, Tuple<RectTransform, RawImage>>();

    public bool IsInLockBounds(Vector3 pos)
    {
        Vector3 myBounds = new Vector3(myLockOnCollider.sizeDelta.x / 2, myLockOnCollider.sizeDelta.y / 2, 0);

        Vector3 point = myLockOnCollider.transform.InverseTransformPoint(pos);
        point = new Vector3(Mathf.Abs(point.x), Mathf.Abs(point.y), 0);

        return point.x < myBounds.x && point.y < myBounds.y;
    }

    private Tuple<RectTransform, RawImage> AddEnemyOnScreen(Vector3 pos, int id)
    {
        GameObject o = Instantiate(EnemyRecObj, EnemyRecParent.transform);

        RectTransform r = o.GetComponent<RectTransform>();
        r.localPosition = pos;
        RawImage i = o.GetComponent<RawImage>();

        Tuple<RectTransform, RawImage> t = new Tuple<RectTransform, RawImage>(r, i);

        enemies.Add(id, t);
        return t;
    }

    public void UpdateScreenEnemy(Vector3 pos, int id)
    {
        Tuple<RectTransform, RawImage> o;

        if (!enemies.ContainsKey(id))
        {
            o = AddEnemyOnScreen(pos, id);
            return;
        }
        else
        {
            o = enemies[id];
        }

        o.Item1.localPosition = EnemyRecParent.transform.InverseTransformPoint(pos);
    }

    public void RemoveEnemyFromScreen(int id)
    {
        Tuple<RectTransform, RawImage> o;

        if (enemies.ContainsKey(id))
        {
            o = enemies[id];
            enemies.Remove(id);

            Destroy(o.Item1.gameObject);
        }
    }

    public void StartLockingOnEnemy(int id)
    {
        foreach (var i in enemies)
        {
            if (i.Key == id) i.Value.Item2.color = _lockingOnColor;

            else
            {
                i.Value.Item2.color = _notLockedOnColor;
            }
        }
    }

    public void LockOnToEnemy(int id)
    { 
        foreach (var i in enemies)
        {
            if (i.Key == id) i.Value.Item2.color = _lockedOnColor;

            else
            {
                i.Value.Item2.color = _notLockedOnColor;
            }
        }
    }

    public void UnLockEveryone()
    {
        if (enemies.Count > 0)
        {
            foreach (var i in enemies)
            {
                i.Value.Item2.color = _notLockedOnColor;
            }
        }
    }

    public void ClearAllLockOns()
    {
        if (enemies.Count > 0)
        {
            foreach (var i in enemies)
            {
                Destroy(i.Value.Item1.gameObject);
            }
            enemies.Clear();
        }
    }

    public void FPSMode()
    {
        ClearAllLockOns();

        myLockOnCollider.gameObject.SetActive(false);
        EnemyRecParent.gameObject.SetActive(false);
        _myCrossHair.gameObject.SetActive(true);

    }


    public void LockOnModeSwitch(bool LockOn)
    {
        if (LockOn)
        {
            myLockOnCollider.gameObject.SetActive(true);
            EnemyRecParent.gameObject.SetActive(true);
            _myCrossHair.gameObject.SetActive(false);
        }
        else
        {
            myLockOnCollider.gameObject.SetActive(false);
            EnemyRecParent.gameObject.SetActive(false);
            _myCrossHair.gameObject.SetActive(true);
            ClearAllLockOns();
        }
    }


}
