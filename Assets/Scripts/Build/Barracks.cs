using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Barracks : NetworkBehaviour
{
    public List<GameObject> _trainQueue;
    [SerializeField] private List<TimerTip> _timerTipList;
    public bool _canCheckAmount = false;
    [SerializeField] private LayerMask _buildingMask;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Image _timerImage;
    [HideInInspector] public float _trainTimer;
    private GameObject _camera;

    private void Start()
    {
        if (!isLocalPlayer) return;

        _camera = Camera.main.gameObject;
        gameObject.tag = _camera.name;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D _hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, _buildingMask);

            if (_hit && _hit.collider.gameObject == gameObject)
            {
                List<GameObject> list = new List<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == _canvas.name && obj != _canvas));
                foreach (GameObject canvas in list)
                {
                    canvas.SetActive(false);
                }
                _canvas.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _canvas.SetActive(false);
        }

        if (_trainQueue.Count > 0 && _trainTimer < _trainQueue[0].gameObject.GetComponent<Pawn>()._trainTime)
        {
            _trainTimer += Time.deltaTime;
        }
        if (_trainQueue.Count > 0 && _trainTimer > _trainQueue[0].gameObject.GetComponent<Pawn>()._trainTime)
        {
            InstantiatePawn(_trainQueue[0].gameObject);
            _trainTimer = 0;
            foreach (TimerTip tip in _timerTipList)
            {
                if (tip._prefab == _trainQueue[0].gameObject) 
                {
                    tip.ResetTimerTip();
                }
            }
            _trainQueue.RemoveAt(0);
        }
    }

    public void TrainPawn(GameObject _pawnPrefab)
    {
        if (_camera.GetComponent<MoneyController>()._moneyCount >= _pawnPrefab.GetComponent<Pawn>()._price)
        {
            _camera.GetComponent<MoneyController>()._moneyCount -= _pawnPrefab.GetComponent<Pawn>()._price;
            _camera.GetComponent<MoneyController>().UpdateText();
            _trainQueue.Add(_pawnPrefab);
        }
    }

    private void InstantiatePawn(GameObject _pawnPrefab)
    {
        GameObject pawn = Instantiate(_pawnPrefab, transform.position, Quaternion.identity);
        pawn.tag = gameObject.tag;
    }
}