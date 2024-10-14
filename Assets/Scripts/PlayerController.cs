using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1.0f;
    private Camera cam;
    private Vector3 mousePos;
    private bool mouseDown = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !mouseDown){
            mouseDown = true;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition) ;
            mousePos.z = 0f;
        }
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mousePos, step);

        if(Input.GetMouseButtonUp(0)){
            mouseDown = false;
        }
    }
}
