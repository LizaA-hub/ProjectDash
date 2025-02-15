using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    #region Variables Definitions
    //inspector variables//
    [Header("Need to be assigned")]

    public PlayerStatsScriptableObject initialStats;
    [SerializeField]
    Transform projectilePrefab, shockWavePrefab, swordTransform, bombPrefab;
    [SerializeField]
    SpriteRenderer shieldSprite;
    [SerializeField]
    CircleCollider2D orbMagnet;

    [Space(10f)]
    [Header("Behavior variables")]

    [SerializeField]
    float  cooldown = 2f;
    [SerializeField]
    float projectileSpeed = 15f;
    [SerializeField]
    float waveSpeed = 10f;
    [SerializeField]
    float knockBackForce = 1f;
    [SerializeField]
    private Color damageColor;

    [Space(10f)]
    [Header("Other")]

    [SerializeField]
    LayerMask enemyLayer;
    [SerializeField]
    bool debug = false;

    //hiden variables//
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    private Camera cam;
    private Vector3 mousePos;
    private Vector2 swordPos;
    private bool mouseDown = false,  isMoving = false, canFireWave = false, canMoveSword = false, movingSword = false;
    private float angle = 0f, swordRadius = 0.4f, swordSpeed = 30f, angleLimit = 0f, moveSpeed;
    public static float projectileTimer, shockWaveTimer, swordTimer, bombTimer;
    public static int dashShield = 0;

    private List<Transform> projectiles = new List<Transform>(), shockWaves = new List<Transform>(), bombs = new List<Transform>(), damageTransforms = new List<Transform>();
    private List<float> damageTimers = new List<float>();

    private Transform newProjectile, newWave, map, newBomb;
    private TrailRenderer swordTrail;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    void Start()//setting all variables 
    {
        cam = Camera.main;
        projectileTimer = shockWaveTimer = swordTimer = bombTimer = PowerUpManager.upgradableDatas.dashCooldown;
        PowerUpManager.magnetIncrease.AddListener(IncreaseOrbMagnet);

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

        moveSpeed = GameManagerV2.instance.skills.dashSpeed;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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
                for(int i = 1; i < PowerUpManager.upgradableDatas.projectileNb;i++){//number of projectile fired dependent of the upgrade level
                    float angle = (90f-15f)*i;
                    FireProjectile(LocalRotation(transform.position,mousePos,angle));
                    FireProjectile(LocalRotation(transform.position, mousePos, -angle));

                }
                projectileTimer = PowerUpManager.upgradableDatas.dashCooldown;
            }

            //dash shock wave//
            if((shockWaveTimer <= 0f) && isMoving){
                FireShockWave();
                shockWaveTimer = PowerUpManager.upgradableDatas.dashCooldown;
            }
            //dash shield//
            if(!isMoving){
                isMoving = true;
                dashShield = PowerUpManager.upgradableDatas.shieldLevel;
                shieldSprite.enabled = (dashShield>0)? true: false;
                
            }
            //bomb//
            if (bombTimer <= 0f) {
                DropBomb();
                bombTimer = PowerUpManager.upgradableDatas.dashCooldown;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var controller = other.gameObject.GetComponent<EnemyController>();
            var strength = controller.strength;
            TakeDamage(strength, other.transform);
            if(controller.type == EnemyType.Boss_Test)
            {
                KnockBack(other.transform.position);
            }
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            var strength = other.gameObject.GetComponent<Bullet>().strength;
            TakeDamage(strength, other.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DamageArea"))
        {
            //Debug.Log("player in contact with acid");
            var amount = collision.gameObject.GetComponent<DamageArea>().strength;
            TakeDamage(amount, collision.transform);               
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
        //timers before next damage//
            List<int> toRemove = new List<int>();
            for (int i = 0; i < damageTimers.Count; i++)
            {
                damageTimers[i] -= t;
                if (damageTimers[i] <= 0)
                {
                    toRemove.Add(i);
                }
            }

            if(toRemove.Count > 0)
            {
                for (int i = toRemove.Count-1; i >= 0; i--)
                {
                    damageTimers.RemoveAt(i);
                    damageTransforms.RemoveAt(i);
                }
            }
        
        //dash projectile timer//
        if ((projectileTimer > 0 ) && PowerUpManager.upgradableDatas.haveProjectile){
                projectileTimer -= t;
        }
        //shock wave timer//
        if((shockWaveTimer > 0 ) && PowerUpManager.upgradableDatas.haveWave){
            shockWaveTimer -= t;
        }
        //sword timer//
        if (swordTimer > 0 && PowerUpManager.upgradableDatas.haveSword) { 
            swordTimer -= t;
        }
        //bomb timer//
        if (bombTimer > 0f && PowerUpManager.upgradableDatas.haveBomb) {
            bombTimer -= t;
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
                        shockWaveTimer = PowerUpManager.upgradableDatas.dashCooldown;
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

        private void TakeDamage(float amount, Transform source){
            if (IsInList(source))
                return;

            if (dashShield > 0)
            {
                dashShield -= 1;
                shieldSprite.enabled = (dashShield > 0) ? true : false;
                return;
            }
            GameManagerV2.instance.ModifyHealth(-amount);
            AddToList(source);

            if (spriteRenderer != null)
        {
            StartCoroutine(DamageFlash()); 
        }
    }

        private void Heal(float amount){
            GameManagerV2.instance.ModifyHealth(amount);
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
        
        private bool IsInList(Transform transform)
        {
            foreach (var item in damageTransforms)
            {
                if (item == transform) return true;
            }
            return false;
        }

    private void AddToList(Transform transform)
    {
        damageTransforms.Add(transform);
        damageTimers.Add(cooldown);
    }
    private void KnockBack(Vector3 enemyPos)
    {
        Vector3 direction = transform.position - (enemyPos - transform.position).normalized;
        mousePos = direction* knockBackForce;
    }

    private IEnumerator DamageFlash()
    {
        // Flash white
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f); // Adjust the flash duration as needed

        // Return to the original color
        spriteRenderer.color = originalColor;
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
                        if((Mathf.Abs(projectile.position.x) - projectile.lossyScale.x > map.lossyScale.x/2) || (Mathf.Abs(projectile.position.y) - projectile.lossyScale.y > map.lossyScale.y/2) ){
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
                    float radius = Mathf.MoveTowards(controller.currentRadius, PowerUpManager.upgradableDatas.waveRadius, step);
                    float segment = 25f/7f*radius+150f/7f;
                    controller.SetRing(radius,(int)segment);
                    //check if radius > max radius
                    var mapMaxSize = Mathf.Max(map.lossyScale.x/2,map.lossyScale.y/2);
                    var maxRadius = Mathf.Min(mapMaxSize, PowerUpManager.upgradableDatas.waveRadius);
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
                        if(debug)
                            Debug.Log(enemy.name + "touched with sword");
                        var controller = enemy.GetComponent<EnemyController>();
                        controller.TakeDamage(PowerUpManager.upgradableDatas.swordDamage);
                    }
                }
                else
                {
                    swordTimer = PowerUpManager.upgradableDatas.dashCooldown;
                    movingSword = false;
                    swordTrail.emitting = false;
                }
            }

    #endregion
    #region Bomb Functions
    private void DropBomb()
    {

        if (bombs.Count > 0)
        {// check if there's a bomb object available
            bool reuseBomb = false;
            foreach (Transform bomb in bombs)
            {
                if (!bomb.gameObject.activeSelf)
                {
                    bomb.gameObject.SetActive(true);
                    newBomb = bomb;
                    reuseBomb = true;
                    break;
                }
            }
            if (!reuseBomb)
            {
                //create new bomb//
                newBomb = Instantiate(bombPrefab);
                bombs.Add(newBomb);
            }

        }
        else
        {
            //create new bomb//
            newBomb = Instantiate(bombPrefab);
            bombs.Add(newBomb);
        }

        newBomb.position = transform.position;
    }

    #endregion
}
