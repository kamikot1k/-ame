using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSelectionManager : MonoBehaviour
{
    public static PawnSelectionManager Instance { get; set; }

    public List<GameObject> allPawns = new List<GameObject>();
    public List<GameObject> selectedPawns = new List<GameObject>();

    [SerializeField] private LayerMask _pawnMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private GameObject _destinationMarker;
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

            if (_hit)
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

        if (Input.GetMouseButton(1) && selectedPawns.Count > 0)
        {
            Vector3 ray = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D _hit = Physics2D.Raycast(ray, Vector2.zero, Mathf.Infinity, _groundMask);

            if (_hit)
            {
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
            TriggerSelectSprite(pawn, true);
            EnablePawnMovement(pawn, true);
        }
        else
        {
            EnablePawnMovement(pawn, false);
            TriggerSelectSprite(pawn, false);
            selectedPawns.Remove(pawn);
        }
    }

    private void ClickSelectPawn(GameObject pawn)
    {
        DeselectAll();

        selectedPawns.Add(pawn);

        TriggerSelectSprite(pawn, true);
        EnablePawnMovement(pawn, true);
    }

    private void EnablePawnMovement(GameObject pawn, bool trigger)
    {
        pawn.GetComponent<PawnMovement>().enabled = trigger;
    }

    private void DeselectAll()
    {
        foreach (var pawn in selectedPawns)
        {
            EnablePawnMovement(pawn, false);
            TriggerSelectSprite(pawn, false);
        }
        _destinationMarker.SetActive(false);
        selectedPawns.Clear();
    }

    private void TriggerSelectSprite(GameObject pawn, bool isVisible)
    {
        pawn.transform.GetChild(0).gameObject.SetActive(isVisible);
    }
}
