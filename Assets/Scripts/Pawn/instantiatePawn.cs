using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instantiatePawn : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosition;

    public void PlacePawn(GameObject _pawnPrefab)
    {
        Instantiate(_pawnPrefab, _spawnPosition);
    }
}