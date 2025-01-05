using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class BuildingsGrid : NetworkBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);

    public Building[,] grid;

    public GameObject flyingBuilding;

    public GameObject Base;
    public Camera mainCamera;

    private float _curBuildingPrice;

    public int BaseX;
    public int BaseY;

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y];
    }

    public void StartPlacingBuilding(GameObject buildingPrefab)
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding);
        }
        foreach (Buildings i in NetworkClient.localPlayer.gameObject.GetComponent<MoneyController>()._buildings)
        {
            if (i.BuildingSettings[0]._name == buildingPrefab.name)
            {
                _curBuildingPrice = i.BuildingSettings[0]._price;
            }
        }
        if (NetworkClient.localPlayer.gameObject.GetComponent<MoneyController>()._moneyCount >= _curBuildingPrice)
        {
            flyingBuilding = Instantiate(buildingPrefab);
            flyingBuilding.GetComponent<NavMeshObstacle>().enabled = false;
            flyingBuilding.GetComponent<BoxCollider2D>().enabled = false;
            if (flyingBuilding.GetComponent<Barracks>() != null)
            {
                flyingBuilding.GetComponent<Barracks>().enabled = false;
            }
        }
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            var groundPlane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.y);

                bool available = true;

                if (x < BaseX || x > GridSize.x - flyingBuilding.GetComponent<Building>().Size.x) available = false;
                if (y < BaseY || y > GridSize.y - flyingBuilding.GetComponent<Building>().Size.y) available = false;

                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x, y, 0);
                flyingBuilding.GetComponent<Building>().SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    NetworkClient.localPlayer.gameObject.GetComponent<MoneyController>()._moneyCount -= flyingBuilding.GetComponent<Building>()._price;
                    if (flyingBuilding.GetComponent<Mine>() != null)
                    {
                        flyingBuilding.GetComponent<Mine>().AddMoneyPerTime();
                    }
                    flyingBuilding.GetComponent<Building>()._building.BuildingSettings[0]._price *= flyingBuilding.GetComponent<Building>()._priceModifier;
                    NetworkClient.localPlayer.gameObject.GetComponent<MoneyController>()._moneyCountText.text = NetworkClient.localPlayer.gameObject.GetComponent<MoneyController>()._moneyCount.ToString();
                    flyingBuilding.GetComponent<Renderer>().sortingOrder = 0;
                    flyingBuilding.GetComponent<NavMeshObstacle>().enabled = true;
                    if (flyingBuilding.GetComponent<Barracks>() != null)
                    {
                        flyingBuilding.GetComponent<Barracks>().enabled = true;
                    }
                    PlaceFlyingBuilding(x, y);
                }
            }
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.GetComponent<Building>().Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.GetComponent<Building>().Size.y; y++)
            {
                if (grid[placeX + x, placeY + y] != null) return true;
            }
        }

        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {

        for (int x = 0; x < flyingBuilding.GetComponent<Building>().Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.GetComponent<Building>().Size.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBuilding.GetComponent<Building>();
            }
        }

        flyingBuilding.GetComponent<Building>().SetNormal();
        flyingBuilding.GetComponent<BoxCollider2D>().enabled = true;
        if (flyingBuilding.GetComponent<BuildingSpriteConnecting>() != null)
        {
            foreach (GameObject i in Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == flyingBuilding.name && obj.tag == flyingBuilding.tag))
            {
                i.GetComponent<BuildingSpriteConnecting>().UpdateTexture();
            }
        }
        flyingBuilding.GetComponent<Building>()._team = NetworkClient.localPlayer.gameObject.name;
        PlaceBuilding();
        flyingBuilding = null;
    }

    [Server]
    public void PlaceBuilding()
    {
        NetworkServer.Spawn(flyingBuilding);
    }
}
