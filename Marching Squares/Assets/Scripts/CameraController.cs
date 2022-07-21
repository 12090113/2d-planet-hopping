using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private Transform target;
    private float speed = 0.125f;
    private float rotspeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float scrollSpeed = 0.1f;
    private Camera cam;
    private float scroll;
    private float touchPos = -2;
    private float touchID = -1;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        scroll = cam.orthographicSize;
        player = FindObjectOfType<PlayerController>().transform;
        target = player;
    }
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotspeed);
    }

    private void Update()
    {
        for (int i = Input.touchCount - 1; i >= 0; i--)
        {
            if (Input.touches[i].rawPosition.y / Screen.height > 0.75)
            {
                Touch touch = Input.touches[i];
                if (touch.fingerId == touchID && touch.phase == TouchPhase.Moved)
                {
                    Debug.Log(touchID);
                    scroll += -cam.orthographicSize * (10 * (touch.position.x - touchPos) / Screen.width);
                    scroll = Mathf.Clamp(scroll, 1, 10000);
                }
                touchPos = touch.position.x;
                touchID = touch.fingerId;
            }
        }
        if (/*Input.GetAxis("Mouse ScrollWheel")*/ Input.mouseScrollDelta.y != 0f)
        {
            scroll += -cam.orthographicSize * Input.mouseScrollDelta.y * scrollSpeed; //Input.GetAxis("Mouse ScrollWheel");
            scroll = Mathf.Clamp(scroll, 1, 10000);
        }
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, scroll, 0.1f);
        /*if (offset.z == -10 && cam.orthographicSize > 70)
        {
            offset.z = -20;
        }
        else if (offset.z == -20 && cam.orthographicSize <= 70)
        {
            offset.z = -10;
        }*/
    }
}