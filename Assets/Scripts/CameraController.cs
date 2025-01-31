using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform player, map;
    [SerializeField, Range(0f,1f)]
    float smoothTime = 0.5f;
    [SerializeField]
    bool focusPlayer = false;
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
        if((Mathf.Abs(mousePos.x - transform.position.x) >= Mathf.Abs(bound.x*5/6))||( Mathf.Abs(mousePos.y - transform.position.y) >= Mathf.Abs(bound.y* 5/6))){ //trying from 3/4 to 5/6
            followMouse = true;
        }
        else{
            followMouse = false;
        }
        //mouse is at the edge of the screen
        if (followMouse && !focusPlayer){
            target = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        }
        else{//follow the player
            target = player.position;
        }
        
        smoothPosition = Vector3.SmoothDamp(transform.position,ClampTarget(target),ref velocity,smoothTime);
        transform.position = smoothPosition;

        //for debugging shake
        /*if (Input.GetKeyUp(KeyCode.S))
        {
            StartCoroutine(Shake(0.1f,0.1f));
        }*/
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

    public IEnumerator Shake(float duration, float amplitude)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            Vector3 translation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * amplitude;
            transform.position += translation;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
