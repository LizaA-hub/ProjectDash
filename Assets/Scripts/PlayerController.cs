using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 10.0f, cooldown = 2f, projectileSpeed = 15f, waveSpeed = 10f;
    [SerializeField]
    Transform projectilePrefab, shockWavePrefab, swordTransform;
    [SerializeField]
    SpriteRenderer shieldSprite;
    [SerializeField]
    CircleCollider2D orbMagnet;
    [SerializeField]
    LayerMask enemyLayer;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    private Camera cam;
    private Vector3 mousePos;
    private Vector2 swordPos;
    private bool mouseDown = false, invincible = false, isMoving = false, canFireWave = false, canMoveSword = false, movingSword = false;
    private float cooldownTimer, projectileTimer, shockWaveTimer, angle = 0f, swordRadius = 0.4f, swordSpeed = 30f, swordTimer, angleLimit = 0f;
    private int dashShield = 0;
    private List<Transform> projectiles = new List<Transform>(), shockWaves = new List<Transform>();
    private Transform newProjectile, newWave, map;
    private TrailRenderer swordTrail;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()//setting all variables 
    {
        cam = Camera.main;
        cooldownTimer = cooldown;
        projectileTimer = shockWaveTimer = swordTimer = GameManager.dashCooldown;
        GameManager.magnetIncrease.AddListener(IncreaseOrbMagnet);

        map = GameObject.Find("Ground").GetComponent<Transform>();
        if(map == null)
        {
            Debug.Log("ground object not found");
        }

        m_Raycaster = GameObject.Find("HUD").GetComponent<GraphicRaycaster>();
        if(m_Raycaster == null)
        {
            Debug.Log("HUD object not found");
        }

        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        if(m_EventSystem == null)
        {
            Debug.Log("event system object not found");
        }

        swordTrail = swordTransform.gameObject.GetComponent<TrailRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        //on mouse click//
        if(Input.GetMouseButtonDown(0) && !mouseDown && !MouseOverUI()){
            canFireWave = true;
            canMoveSword = true;

            if (movingSword)
            {
                ToggleSword(false);
            }

            mouseDown = true;
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition) ;
            mousePos.z = transform.position.z;

            //dash projectile//
            if(projectileTimer <= 0f){
                FireProjectile(mousePos);
                for(int i = 1; i < GameManager.projectileNb;i++){//number of projectile fired dependent of the upgrade level
                    float angle = (90f-15f)*i;
                    FireProjectile(LocalRotation(transform.position,mousePos,angle));
                    FireProjectile(LocalRotation(transform.position, mousePos, -angle));

                }
                projectileTimer = GameManager.dashCooldown;
            }

            //dash shock wave//
            if((shockWaveTimer <= 0f) && isMoving){
                FireShockWave();
                shockWaveTimer = GameManager.dashCooldown;
            }

            if(!isMoving){
                isMoving = true;
                dashShield = GameManager.dashShieldLevel;
                shieldSprite.enabled = (dashShield>0)? true: false;
                
            }
            
        }

        //moving player//
        MovePlayer(Time.deltaTime);

        //reset mouse input//
        if(Input.GetMouseButtonUp(0)){
            mouseDown = false;
        }

        //update timers//
        UpdateTimers(Time.deltaTime);

        //update instantiated dash attacks//
        UpdateProjectile(Time.deltaTime);
        UpdateWaves(Time.deltaTime);
        MoveSword(Time.deltaTime);
        
        



    }


    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Enemy")){
            if(!invincible){
                if (dashShield > 0)
                {
                    dashShield -= 1;
                    shieldSprite.enabled = (dashShield > 0) ? true : false;
                    return;
                }
                //Debug.Log("player take damages");
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
            //sword timer//
            if (swordTimer > 0 && GameManager.haveSword) { 
                swordTimer -= t;
            }
        }

        private void MovePlayer(float t)
        {
            var step = moveSpeed * t;
            transform.position = Vector3.MoveTowards(transform.position, mousePos, step);

            if (Vector3.Distance(transform.position, mousePos) < 0.1f)//player stop moving
            {
                isMoving = false;
                //fire wave attack//
                if (canFireWave)
                {
                    if (shockWaveTimer <= 0f)
                    {
                        FireShockWave();
                        shockWaveTimer = GameManager.dashCooldown;
                    }
                    canFireWave = false;
                }
                //hide dash shield//
                dashShield = 0;
                shieldSprite.enabled = false;

                //start moving sword
                if (canMoveSword)
                {
                    if (swordTimer <= 0f)
                        {
                            ToggleSword(true);
                        }
                    canMoveSword = false;
                }
                

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

            /*// For every result returned, output the name of the GameObject on the Canvas hit by the Ray
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

        private void IncreaseOrbMagnet()
        {
            float radius = orbMagnet.radius;
            radius += radius * 0.1f;
            orbMagnet.radius = radius;
        }

        Vector3 LocalRotation(Vector3 origin,Vector3 from, float angle)
        {
            Vector3 localFrom = from - origin;
            Vector3 to = Vector3.zero;
            to.x = localFrom.x * Mathf.Cos(angle) - localFrom.y * Mathf.Sin(angle) + transform.position.x;
            to.y = localFrom.x * Mathf.Sin(angle) + localFrom.y * Mathf.Cos(angle) + transform.position.y;
            return to;
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
                        var mapMaxSize = Mathf.Max(map.lossyScale.x/2,map.lossyScale.y/2);
                        var maxRadius = Mathf.Min(mapMaxSize,GameManager.shockWaveMaxRadius);
                        if(radius >= maxRadius){
                            DisableTransform(wave);
                        }
                    }
                }
            }

        #endregion
        #region Sword Functions
            private void MoveSword(float t)
            {
                if (!movingSword) return;
                if (angle > angleLimit)
                {
                    var swordStep = t * swordSpeed;
                    angle -= swordStep;
                    float x = swordPos.x + Mathf.Cos(angle) * swordRadius;
                    float y = swordPos.y + Mathf.Sin(angle) * swordRadius;

                    swordTransform.position = new Vector3(x, y, 0);
                }
                else
                {
                ToggleSword(false);
                }

        }

            private void ToggleSword(bool toggle)
            {
                if (toggle)
                {
                    //reset variable//

                    var degAngle = Vector2.SignedAngle(new Vector2(0f,-1f), mousePos - transform.position);
                    angle = degAngle*Mathf.Deg2Rad;
                    angleLimit = angle - 3f / 2f * Mathf.PI;

                    float coord = Mathf.Sqrt((swordRadius*swordRadius) / 2);

                    float rotAngle = 90f - 15f;
                    Vector3 swordLocal = LocalRotation(transform.position, mousePos, rotAngle);
                    swordPos = (swordLocal-transform.position).normalized * coord + transform.position;
                    movingSword = true;
                    swordTrail.emitting = true;
                    //detect enemy in range//
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordPos, swordRadius+(swordTrail.widthMultiplier/2), enemyLayer);
          
                    foreach (var enemy in hitEnemies)
                    {
                        //Debug.Log(enemy.name + "touch with sword");
                        var controller = enemy.GetComponent<EnemyController>();
                        controller.TakeDamage(GameManager.swordStrength);
                    }
                }
                else
                {
                    swordTimer = GameManager.dashCooldown;
                    movingSword = false;
                    swordTrail.emitting = false;
                }
            }

            #endregion
        }
