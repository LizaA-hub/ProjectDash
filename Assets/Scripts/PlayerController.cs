using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10.0f, cooldown = 2f;
    private Camera cam;
    private Vector3 mousePos;
    private bool mouseDown = false, invincible = false;
    private float cooldownTimer;
    
    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cooldownTimer = cooldown;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !mouseDown){
            mouseDown = true;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition) ;
            mousePos.z = transform.position.z;
            
        }
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mousePos, step);

        if(Input.GetMouseButtonUp(0)){
            mouseDown = false;
        }

        if(invincible){
            cooldownTimer -= Time.deltaTime;
            if(cooldownTimer <= 0){
                cooldownTimer = cooldown;
                invincible = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Enemy")){
            if(!invincible){
                var strength = other.gameObject.GetComponent<EnemyController>().strength;
                TakeDamage(strength);
                invincible = true;
            }
        }
    }
    #endregion
    #region Private Functions
        private void TakeDamage(float amount){
            GameManager.ModifyHealth(-amount);
        }

        private void Heal(float amount){
            GameManager.ModifyHealth(amount);
        }
    #endregion
}
