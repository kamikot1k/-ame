using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PriceTip : MonoBehaviour
{
    [SerializeField] private Text _tip;
    [SerializeField] private GameObject _buildingPrefab;

    public void mouseEnterEvent()
    {
        foreach(Buildings i in Camera.main.gameObject.GetComponent<MoneyController>()._buildings)
        {
            if (i.BuildingSettings[0]._name == _buildingPrefab.name)
            {
                _tip.text = i.BuildingSettings[0]._price.ToString();
            }
        }
    }

    public void mouseExitEvent()
    {
        _tip.text = _buildingPrefab.name;
    }
}
