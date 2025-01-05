using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private KeyCode cameraUp = KeyCode.W;
    [SerializeField] private KeyCode cameraLeft = KeyCode.A;
    [SerializeField] private KeyCode cameraDown = KeyCode.S;
    [SerializeField] private KeyCode cameraRight = KeyCode.D;
    [SerializeField] private float _cameraMoveSpeed = 10f;
    [SerializeField] private float _minX, _maxX;
    [SerializeField] private float _minY, _maxY;
    [SerializeField] private GameObject _map;

    private void Start()
    {
        _map = GameObject.FindGameObjectWithTag("Ground");

        _minX = _map.transform.position.x - _map.GetComponent<Renderer>().bounds.size.x / 2f;
        _maxX = _map.transform.position.x + _map.GetComponent<Renderer>().bounds.size.x / 2f;

        _minY = _map.transform.position.y - _map.GetComponent<Renderer>().bounds.size.y / 2f;
        _maxY = _map.transform.position.y + _map.GetComponent<Renderer>().bounds.size.y / 2f;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKey(cameraUp)) 
        {
            gameObject.transform.Translate(0, _cameraMoveSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(cameraLeft))
        {
            gameObject.transform.Translate(-_cameraMoveSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(cameraDown))
        {
            gameObject.transform.Translate(0, -_cameraMoveSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(cameraRight))
        {
            gameObject.transform.Translate(_cameraMoveSpeed * Time.deltaTime, 0, 0);
        }

        gameObject.transform.position = ClampCamera(transform.position);

        Transform cameraTransform = Camera.main.transform;
        cameraTransform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, cameraTransform.position.z);
    }

    private Vector3 ClampCamera(Vector3 targetPosition) {
        float camHeight = Camera.main.orthographicSize;
        float camWidth = Camera.main.orthographicSize * Camera.main.aspect;

        float absoluteMinX = _minX + camWidth;
        float absoluteMaxX = _maxX - camWidth;
        float absoluteMinY = _minY + camHeight;
        float absoluteMaxY = _maxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, absoluteMinX, absoluteMaxX);
        float newY = Mathf.Clamp(targetPosition.y, absoluteMinY, absoluteMaxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
