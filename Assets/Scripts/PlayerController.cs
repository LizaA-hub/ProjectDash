using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10.0f, cooldown = 2f, projectileSpeed = 20f;
    [SerializeField]
    Transform projectilePrefab, map;
    [SerializeField]
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField]
    EventSystem m_EventSystem;
    private Camera cam;
    private Vector3 mousePos;
    private bool mouseDown = false, invincible = false;
    private float cooldownTimer, projectileTimer;

    private List<Transform> projectiles = new List<Transform>();
    private Transform newProjectile;
    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cooldownTimer = cooldown;
        projectileTimer = GameManager.projectileCooldown;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !mouseDown && !MouseOverUI()){
            mouseDown = true;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition) ;
            mousePos.z = transform.position.z;

            if(projectileTimer <= 0){
                FireProjectile(mousePos);
                projectileTimer = GameManager.projectileCooldown;
            }
            
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

        if((projectileTimer > 0 ) && GameManager.haveProjectile){
            projectileTimer -= Time.deltaTime;
        }
        
        UpdateProjectile(Time.deltaTime);
        
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
    #region Public Functions
        public void DisableProjectile(Transform projectile){
            projectile.gameObject.SetActive(false);
        }

    #endregion
    #region Private Functions
        private void TakeDamage(float amount){
            GameManager.ModifyHealth(-amount);
        }

        private void Heal(float amount){
            GameManager.ModifyHealth(amount);
        }

        private void FireProjectile(Vector3 target){
            
            if(projectiles.Count > 0){
                bool reuseProjectile = false;
                foreach (Transform projectile in projectiles)
                {
                    if(!projectile.gameObject.activeSelf){
                        projectile.gameObject.SetActive(true);
                        newProjectile = projectile;
                        reuseProjectile = true;
                        break;
                    }
                }
                if(!reuseProjectile){
                    //create new projectile//
                    newProjectile = Instantiate(projectilePrefab);
                    projectiles.Add(newProjectile);
                    newProjectile.gameObject.GetComponent<DashProjectile>().manager = this;      
                }

            }
            else{
                //create new projectile//
                newProjectile = Instantiate(projectilePrefab);
                projectiles.Add(newProjectile);
                newProjectile.gameObject.GetComponent<DashProjectile>().manager = this;            
            }

            //rotate projectile in target direction//
            newProjectile.position = transform.position;
            Vector3 direction = target - transform.position;
            direction = direction.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            newProjectile.rotation = Quaternion.Euler(0, 0, angle-90f);

            //set variables//
            var controller = newProjectile.gameObject.GetComponent<DashProjectile>();
            controller.direction = direction;
        }

        private void UpdateProjectile(float t){
            if(projectiles.Count > 0) foreach (Transform projectile in projectiles)
            {
                if(projectile.gameObject.activeSelf){
                    //move projectile//
                    var step = projectileSpeed * Time.deltaTime;
                    var target = projectile.gameObject.GetComponent<DashProjectile>().direction;
                    projectile.position = Vector3.MoveTowards(projectile.position, projectile.position+target, step);
                    //check if projectile is outside the map//
                    
                    if((Mathf.Abs(projectile.position.x) > map.lossyScale.x/2) || (Mathf.Abs(projectile.position.y) > map.lossyScale.y/2) ){
                        DisableProjectile(projectile);
                    }
                }
            }
        }

        private bool MouseOverUI(){
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            /*//For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
            }*/

            if(results.Count > 0){
                return true;
            }
            else{
                return false;
            }
        }
    #endregion
}
