using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BuildingsGrid : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);

    private Building[,] grid;
    public Building flyingBuilding;
    public GameObject Base;
    public Camera mainCamera;

    public int BaseX;
    public int BaseY;

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y];

        mainCamera = Camera.main;
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding.gameObject);
        }
        if (Camera.main.gameObject.GetComponent<MoneyController>()._moneyCount >= buildingPrefab.GetComponent<Building>()._price)
        {
            flyingBuilding = Instantiate(buildingPrefab);
            flyingBuilding.GetComponent<NavMeshObstacle>().enabled = false;
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
            flyingBuilding.GetComponent<BoxCollider2D>().enabled = false;

            var groundPlane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.y);

                bool available = true;

                if (x < BaseX || x > GridSize.x - flyingBuilding.Size.x) available = false;
                if (y < BaseY || y > GridSize.y - flyingBuilding.Size.y) available = false;

                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x, y, 0);
                flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    Camera.main.gameObject.GetComponent<MoneyController>()._moneyCount -= flyingBuilding.GetComponent<Building>()._price;
                    if (flyingBuilding.GetComponent<Mine>() != null)
                    {
                        flyingBuilding.GetComponent<Mine>().AddMoneyPerTime();
                    }
                    flyingBuilding.GetComponent<Building>()._building.BuildingSettings[0]._price *= flyingBuilding.GetComponent<Building>()._priceModifier;
                    Camera.main.gameObject.GetComponent<MoneyController>()._moneyCountText.text = Camera.main.gameObject.GetComponent<MoneyController>()._moneyCount.ToString();
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
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                if (grid[placeX + x, placeY + y] != null) return true;
            }
        }

        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBuilding;
            }
        }

        flyingBuilding.SetNormal();
        flyingBuilding.GetComponent<BoxCollider2D>().enabled = true;
        flyingBuilding = null;
    }
}
