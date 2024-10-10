using UnityEngine;
using System.Collections;

enum Directions{LEFT,RIGHT,UP,DOWN,LEFT_UP,LEFT_DOWN,RIGHT_UP,RIGHT_DOWN,NONE};
public class PlayerMovement : MonoBehaviour
{
   [SerializeField]
   float distance = 1.0f, speed = 1.0f, frequence = 1.0f;
   [SerializeField]
   bool useMouse = true;
   private float count = 0.0f;
   private Camera cam;

   private Vector3 mousePos, direction, targetPos;
   private Directions nextMove = Directions.NONE;

   private void Start() {
    cam = Camera.main;
   }

   private void Update() {
        if (useMouse) {
            if (count < frequence){
                count += Time.deltaTime;
            }
            else{
                count = 0f;
                mousePos = cam.ScreenToWorldPoint(Input.mousePosition) ;
                mousePos.z = 0f;
                direction = mousePos - transform.position;
                direction = direction.normalized;
                targetPos = transform.position + direction*distance;
            }

            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            
        }
        else{
            if ((transform.position-targetPos).sqrMagnitude > 0f )
            {
                var step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
               return;
            }
            nextMove = GetNextDirection();
            switch (nextMove)
            {
                case Directions.DOWN:
                    direction = new Vector3(0f,-1f,0f);
                    break;
                case Directions.LEFT:
                    direction = new Vector3(-1f,0f,0f);
                    break;
                case Directions.UP:
                    direction = new Vector3(0f,1f,0f);
                    break;
                case Directions.RIGHT:
                    direction = new Vector3(1f,0f,0f);
                    break;
                case Directions.LEFT_DOWN:
                    direction = new Vector3(-1f,-1f,0f);
                    break;
                case Directions.LEFT_UP:
                    direction = new Vector3(-1f,1f,0f);
                    break;
                case Directions.RIGHT_UP:
                    direction = new Vector3(1f,1f,0f);
                    break;
                case Directions.RIGHT_DOWN:
                    direction = new Vector3(1f,-1f,0f);
                    break;
                default:
                    direction = Vector3.zero;
                    break;
            }
            direction = direction.normalized;
            targetPos = transform.position + direction*distance;


        }
   }

   private Directions GetNextDirection(){
        direction = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0f);
        if (direction.x != 0f){
            if (direction.x > 0f){
                if(direction.y != 0f){
                    if(direction.y > 0f){
                        return Directions.RIGHT_UP;
                    }
                    else{
                        return Directions.RIGHT_DOWN;
                    }
                }
                else{
                    return Directions.RIGHT;
                }
            }
            else{
                if(direction.y != 0f){
                    if(direction.y > 0f){
                        return Directions.LEFT_UP;
                    }
                    else{
                        return Directions.LEFT_DOWN;
                    }
                }
                else{
                    return Directions.LEFT;
                }
            }
        }
        else{
            if(direction.y > 0f){
                return Directions.UP;
            }
            else if (direction.y < 0f){
                return Directions.DOWN;
            }
        }
        return Directions.NONE;
   }
}
