using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class PriceTip : MonoBehaviour
{
    [SerializeField] private Text _tip;
    [SerializeField] private GameObject _prefab;

    public void mouseEnterEvent()
    {
        if (_prefab.layer == 3)
        {
            foreach (Buildings i in NetworkClient.localPlayer.gameObject.GetComponent<MoneyController>()._buildings)
            {
                if (i.BuildingSettings[0]._name == _prefab.name)
                {
                    _tip.text = i.BuildingSettings[0]._price.ToString();
                }
            }
        }
        else
        {
            _tip.text = _prefab.GetComponent<Pawn>()._price.ToString();
        }
    }

    public void mouseExitEvent()
    {
        _tip.text = _prefab.name;
    }
}
