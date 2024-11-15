using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10.0f, cooldown = 2f, projectileSpeed = 15f, waveSpeed = 10f;
    [SerializeField]
    Transform projectilePrefab, map, shockWavePrefab;
    [SerializeField]
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField]
    EventSystem m_EventSystem;
    private Camera cam;
    private Vector3 mousePos;
    private bool mouseDown = false, invincible = false, isMoving = false, canFireWave = false;
    private float cooldownTimer, projectileTimer, shockWaveTimer;

    private List<Transform> projectiles = new List<Transform>(), shockWaves = new List<Transform>();
    private Transform newProjectile, newWave;
    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        cooldownTimer = cooldown;
        projectileTimer = GameManager.projectileCooldown;
        shockWaveTimer = GameManager.shockWaveCooldown;
    }


    // Update is called once per frame
    void Update()
    {
        //on mouse click//
        if(Input.GetMouseButtonDown(0) && !mouseDown && !MouseOverUI()){
            mouseDown = true;
            canFireWave = true;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition) ;
            mousePos.z = transform.position.z;

            //dash projectile//
            if(projectileTimer <= 0f){
                FireProjectile(mousePos);
                for(int i = 1; i < GameManager.projectileNb;i++){//number of projectile fired dependent of the upgrade level
                    Vector2 rotatedDir = Vector2.zero;
                    Vector2 relativTarget = mousePos-transform.position;
                    float angle = 90f-15f;
                    rotatedDir.x = relativTarget.x*Mathf.Cos(angle*i)-relativTarget.y*Mathf.Sin(angle*i) + transform.position.x;
                    rotatedDir.y = relativTarget.x*Mathf.Sin(angle*i)+relativTarget.y*Mathf.Cos(angle*i) + transform.position.y;
                    FireProjectile(rotatedDir);
                    rotatedDir.x = relativTarget.x*Mathf.Cos(-angle*i)-relativTarget.y*Mathf.Sin(-angle*i) + transform.position.x;
                    rotatedDir.y = relativTarget.x*Mathf.Sin(-angle*i)+relativTarget.y*Mathf.Cos(-angle*i) + transform.position.y;
                    FireProjectile(rotatedDir);

                }
                projectileTimer = GameManager.projectileCooldown;
            }

            //dash shock wave//
            if((shockWaveTimer <= 0f) && isMoving){
                FireShockWave();
                shockWaveTimer = GameManager.shockWaveCooldown;
            }

            if(!isMoving){
                isMoving = true;
            }
            
        }

        //moving player//
        var step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, mousePos, step);
        if(Vector3.Distance(transform.position,mousePos) < 0.1f){
            isMoving = false;
            if(canFireWave){
                if(shockWaveTimer <= 0f){
                    FireShockWave();
                    shockWaveTimer = GameManager.shockWaveCooldown;
                }
                canFireWave = false;
            }
            
        }

        //reset mouse input//
        if(Input.GetMouseButtonUp(0)){
            mouseDown = false;
        }

        //update timers//
        UpdateTimers(Time.deltaTime);

        //update instantiated dash attacks//
        UpdateProjectile(Time.deltaTime);
        UpdateWaves(Time.deltaTime);
        
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
        public void DisableTransform(Transform _transform){
            _transform.gameObject.SetActive(false);
        }

    #endregion
    #region Private Functions
        private void UpdateTimers(float t){
            //timer befor next damage//
            if(invincible){
                cooldownTimer -= t;
                if(cooldownTimer <= 0){
                    cooldownTimer = cooldown;
                    invincible = false;
                }
            }
            //dash projectile timer//
            if((projectileTimer > 0 ) && GameManager.haveProjectile){
                projectileTimer -= t;
            }
            //shock wave timer//
            if((shockWaveTimer > 0 ) && GameManager.haveShockWave){
                shockWaveTimer -= t;
            }
        }

        private void TakeDamage(float amount){
            GameManager.ModifyHealth(-amount);
        }

        private void Heal(float amount){
            GameManager.ModifyHealth(amount);
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

    #region Projectile Functions

        private void FireProjectile(Vector3 target){
            
            if(projectiles.Count > 0){// check if there's a projectile object available
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
                        DisableTransform(projectile);
                    }
                }
            }
        }

        #endregion

        #region Wave Functions
        private void FireShockWave(){
            //check if there's a wave object avalaible//
            if(shockWaves.Count > 0){
                bool reuseWave = false;
                foreach (Transform wave in shockWaves)
                {
                    if(!wave.gameObject.activeSelf){
                        wave.gameObject.SetActive(true);
                        newWave = wave;
                        reuseWave = true;
                        break;
                    }
                }
                if(!reuseWave){
                    //create a new object//
                    newWave = Instantiate(shockWavePrefab);
                    shockWaves.Add(newWave);
                }
            }
            else{//create a new object//
                newWave = Instantiate(shockWavePrefab);
                shockWaves.Add(newWave);
            }

            newWave.position = transform.position;
            ShockWave controller = newWave.gameObject.GetComponent<ShockWave>();
            controller.SetRing(1f,25);
        }

        private void UpdateWaves(float t){
            foreach (Transform wave in shockWaves)
            {
                if(wave.gameObject.activeSelf){
                    //increase wave 
                    ShockWave controller = wave.gameObject.GetComponent<ShockWave>();
                    float step = waveSpeed*t;
                    float radius = Mathf.MoveTowards(controller.currentRadius, GameManager.shockWaveMaxRadius, step);
                    float segment = 25f/7f*radius+150f/7f;
                    controller.SetRing(radius,(int)segment);
                    //check if radius > max radius

                    if(radius >= GameManager.shockWaveMaxRadius){
                        DisableTransform(wave);
                    }
                }
            }
        }
    #endregion
}
