using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : MonoBehaviour
{
    public List<GameObject> _trainQueue;
    [SerializeField] private LayerMask _buildingMask;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Image _timerImage;
    [HideInInspector] public float _trainTimer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D _hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity);

            if (_hit && _hit.collider.gameObject == gameObject)
            {
                _canvas.SetActive(true);
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            _canvas.SetActive(false);
        }

        if (_trainQueue.Count > 0 && _trainTimer < _trainQueue[0].gameObject.GetComponent<Pawn>()._trainTime)
        {
            _trainTimer += Time.deltaTime;
        }
        if (_trainQueue.Count > 0 && _trainTimer >= _trainQueue[0].gameObject.GetComponent<Pawn>()._trainTime)
        {
            InstantiatePawn(_trainQueue[0].gameObject);
            _trainQueue.RemoveAt(0);
            _trainTimer = 0;
        }
    }

    public void TrainPawn(GameObject _pawnPrefab)
    {
        _trainQueue.Add(_pawnPrefab);
    }

    private void InstantiatePawn(GameObject _pawnPrefab)
    {
        GameObject pawn = Instantiate(_pawnPrefab, transform.position, Quaternion.identity);
        pawn.tag = gameObject.tag;
    }
}