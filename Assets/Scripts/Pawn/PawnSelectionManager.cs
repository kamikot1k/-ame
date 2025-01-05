using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class PawnSelectionManager : NetworkBehaviour
{
    public static PawnSelectionManager Instance { get; set; }

    public List<GameObject> allPawns = new List<GameObject>();
    public List<GameObject> selectedPawns = new List<GameObject>();

    [SerializeField] private LayerMask _pawnMask;
    [SerializeField] private LayerMask _groundMask;
    public GameObject _destinationMarker;
    private Camera _camera;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } 
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ray = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D _hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, _pawnMask);

            if (_hit && NetworkClient.localPlayer.gameObject.name == _hit.collider.gameObject.GetComponent<Pawn>()._team)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelectPawn(_hit.collider.gameObject);
                } 
                else
                {
                    ClickSelectPawn(_hit.collider.gameObject);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    DeselectAll();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedPawns.Count > 0)
        {
            Vector3 ray = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D _hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, _groundMask);

            if (_hit)
            {
                List<Vector3> _targetPositionList = GetPositionListAround(_hit.point, new float[] { 0.8f, 1.6f, 2.4f, 3.2f, 4f, 4.8f, 5.6f, 6.4f, 7.2f }, new int[] { 5, 10, 20, 30, 40, 50, 60, 70, 80 });

                int _targetPositionListIndex = 0;

                foreach (GameObject pawn in selectedPawns)
                {
                    if (pawn.GetComponent<PawnMovement>()._isMovementAvailable == true)
                    {
                        pawn.GetComponent<PawnMovement>()._agent.SetDestination(_targetPositionList[_targetPositionListIndex]);

                        if (Input.GetKey(KeyCode.R))
                        {
                            pawn.GetComponent<PawnMovement>()._priorityMovement = true;
                        }
                        pawn.GetComponent<PawnMovement>()._isMovement = true;
                        pawn.GetComponent<PawnMovement>()._animator.SetBool("isRun", pawn.GetComponent<PawnMovement>()._isMovement);

                        if (_hit.point.x > pawn.transform.position.x)
                        {
                            pawn.GetComponent<PawnMovement>()._sr.flipX = false;
                        }
                        else if (_hit.point.x < pawn.transform.position.x)
                        {
                            pawn.GetComponent<PawnMovement>()._sr.flipX = true;
                        }
                        _targetPositionListIndex = (_targetPositionListIndex + 1) % _targetPositionList.Count;
                    }
                }

                _destinationMarker.transform.position = _hit.point;
                _destinationMarker.SetActive(true);
            }
        }
    }

    private void MultiSelectPawn(GameObject pawn)
    {
        if (selectedPawns.Contains(pawn) == false)
        {
            selectedPawns.Add(pawn);
            SelectPawn(pawn, true);
        }
        else
        {
            SelectPawn(pawn, false);
            selectedPawns.Remove(pawn);
        }
    }

    internal void DragSelect(GameObject pawn)
    {
        if (selectedPawns.Contains (pawn) == false)
        {
            selectedPawns.Add (pawn);
            SelectPawn(pawn, true);
        }
    }

    private void ClickSelectPawn(GameObject pawn)
    {
        DeselectAll();

        selectedPawns.Add(pawn);

        SelectPawn(pawn, true);
    }

    public void SelectPawn(GameObject pawn, bool isSelect)
    {
        TriggerSelectSprite(pawn, isSelect);
        EnablePawnMovement(pawn, isSelect);
    }

    private void EnablePawnMovement(GameObject pawn, bool trigger)
    {
        pawn.GetComponent<PawnMovement>()._isMovementAvailable = trigger;
    }

    private void DeselectAll()
    {
        foreach (var pawn in selectedPawns)
        {
            if (pawn != null)
            {
                SelectPawn(pawn, false);
            }
        }
        _destinationMarker.SetActive(false);
        selectedPawns.Clear();
    }

    private void TriggerSelectSprite(GameObject pawn, bool isVisible)
    {
        pawn.transform.GetChild(0).gameObject.SetActive(isVisible);
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    {
        List<Vector3> positionList = new List<Vector3>();
        positionList.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++) 
        {
            positionList.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return positionList;
    }

    private List<Vector3> GetPositionListAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();

        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }

        return positionList;
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}
