using UnityEngine;

public class XPOrb : MonoBehaviour
{
    [HideInInspector]
    public float XPAmount = 1f;
    [SerializeField]
    float speed = 20f;
    private Vector3 target;
    bool move = false;
    Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        if(player == null)
        {
            Debug.Log("xp orb can't find player transform");
        }
    }

    private void Update() {
        if (move){
            var step = speed * Time.deltaTime;
            target = player.position;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("PlayerField")){
            //target = other.transform.position;
            move = true;
        }
        if(other.gameObject.CompareTag("Player")){
            GameManager.ModifyExperience(XPAmount);
            //Destroy(gameObject);
            move = false;
            gameObject.SetActive(false);
        }
    }

}
