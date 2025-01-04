using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Renderer MainRenderer;
    public Vector2Int Size = Vector2Int.one;
    public float _price;
    public float _priceModifier;
    public GameObject _originalPrefab;
    public Buildings _building;

    private void Start()
    {
        foreach (Buildings i in Camera.main.gameObject.GetComponent<MoneyController>()._buildings)
        {
            if (i.BuildingSettings[0]._name == gameObject.name.Remove(i.BuildingSettings[0]._name.Length, gameObject.name.Length - i.BuildingSettings[0]._name.Length))
            {
                this._price = i.BuildingSettings[0]._price;
                this._priceModifier = i.BuildingSettings[0]._priceModifier;
                _building = i;
            }
        }
    }

    public void SetTransparent(bool available)
    {
        if (available)
        {
            MainRenderer.material.color = Color.green;
            MainRenderer.sortingOrder = 1;
        }
        else
        {
            MainRenderer.material.color = Color.red;
            MainRenderer.sortingOrder = -1;
        }
    }

    public void SetNormal()
    {
        MainRenderer.material.color = Color.white;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                if ((x + y) % 2 == 0) Gizmos.color = new Color(0.88f, 0f, 1f, 0.3f);
                else Gizmos.color = new Color(1f, 0.68f, 0f, 0.3f);

                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
    }
}
