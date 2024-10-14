using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform player, map;
    [SerializeField, Range(0f,1f)]
    float smoothTime = 0.5f;
    bool followMouse = false;
    private Vector3 target, smoothPosition, velocity = Vector3.zero, lowerBound, bound, mousePos;

    private void Start() {
        //getting camera bounds//
        bound = GetComponent<Camera>().ScreenToWorldPoint(Vector3.zero);
    }

    void LateUpdate()
    {
        //checking if the mouse is a the edge of the screen//
        mousePos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        if((Mathf.Abs(mousePos.x - transform.position.x) >= Mathf.Abs(bound.x*3/4))||( Mathf.Abs(mousePos.y - transform.position.y) >= Mathf.Abs(bound.y*3/4))){
            followMouse = true;
        }
        else{
            followMouse = false;
        }
        //mouse is at the edge of the screen
        if (followMouse){
            target = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
        else{//follow the player
            target = player.position;
        }
        
        smoothPosition = Vector3.SmoothDamp(transform.position,ClampTarget(target),ref velocity,smoothTime);
        transform.position = smoothPosition;
    }

    private Vector3 ClampTarget(Vector3 target){ // the camera don't show the outside of the map//
        Vector3 clampedTarget = target;
        clampedTarget.z = transform.position.z;

        float maxY =map.position.y + map.localScale.y/2 - Mathf.Abs(bound.y);
        float minY =map.position.y - map.localScale.y/2 + Mathf.Abs(bound.y);
        float maxX =map.position.x + map.localScale.x/2 - Mathf.Abs(bound.x);
        float minX =map.position.x - map.localScale.x/2 + Mathf.Abs(bound.x);

        clampedTarget.x = Mathf.Clamp(clampedTarget.x,minX,maxX);
        clampedTarget.y = Mathf.Clamp(clampedTarget.y,minY,maxY);
        return clampedTarget;
    }
}
