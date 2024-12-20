using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedShape : MonoBehaviour
{
    public GeometricalShape.Shape shape;
    public Vector2[] points;
    public float area;

    //triangle variables//
    private GameObject field;
    private CapsuleCollider2D fieldCollider;
    private bool enemyOverlap = false;

    //Pentagon variables//
    private Transform bladePrefab, wavePrefab, bombWave, bombPrefab;
    private Transform[] blades = new Transform[5], bombs = new Transform[5];
    private int[] bladeTarget = new int[5];
    bool bladeUpdate = false, implose = false, waveUpdate = false;
    float timer = 2f, waveMaxRadius;
    private List<Collider2D> enemies = new List<Collider2D>();
    private Collider2D[] attractedEnemies;
    Vector3 bombPosition;

    #region Unity Functions
    private void Update()
    {

        if (bladeUpdate)
        {
            for (int i = 0; i < 5; i++)
            {
                blades[i].localPosition = Vector3.MoveTowards(blades[i].localPosition, points[bladeTarget[i]], Time.deltaTime * 2f);
                if(Vector3.Distance(blades[i].localPosition, points[bladeTarget[i]]) <= 0.1f)
                {
                    bladeTarget[i]++;
                    if (bladeTarget[i] >= 5)
                    {
                        bladeTarget[i] = 0;
                    }
                }
            }
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                bladeUpdate = false;
                timer = 2f;
                HideBlades();
            }
        }

        if (implose)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one * 0.1f, Time.deltaTime * 4f);
        }

        if (waveUpdate && bombWave)
        {
            ShockWave controller = bombWave.gameObject.GetComponent<ShockWave>();
            float step = 10f*Time.deltaTime;
            float radius = Mathf.MoveTowards(controller.currentRadius, waveMaxRadius, step);
            float segment = 25f/7f*radius+150f/7f;
            controller.SetRing(radius,(int)segment);
            if(Mathf.Abs(radius - waveMaxRadius) <= 0.1f){
                waveUpdate = false;
                bombWave.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemies.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemies.Remove(collision);
        }
    }

    #endregion
    #region Triangle Gravity Functions
    public void AddEnemy()
    {
        if (!enemyOverlap)
        {
            enemyOverlap = true;
        }
        
    }

    public IEnumerator HasField(bool value)
    {
        if (enemyOverlap)
            enemyOverlap = false;

        yield return new WaitForFixedUpdate();

        if (!fieldCollider)
        {
            CreateFieldChild();
        }
      

        if (value && !enemyOverlap)
        {
            
            fieldCollider.enabled = true;
            SetColliderShape(); 
        }
        else
        {
            fieldCollider.enabled = false;
        }
    }

    private void CreateFieldChild()
    {
        field = new GameObject("ShapeField", typeof(CapsuleCollider2D));
        field.tag = "ShapeField";
        fieldCollider = field.GetComponent<CapsuleCollider2D>();
        fieldCollider.enabled = false;
        fieldCollider.isTrigger = true;
        fieldCollider.transform.SetParent(transform);
    }
    private IEnumerator FieldCountdown()
    {
        yield return new WaitForSeconds(GameManager.skillVariables.triangleGravityDuration);
        //Debug.Log("field deactivated");
        enemyOverlap = false;
        fieldCollider.enabled = false;
    }

    private void SetColliderShape()
    {
        //defining triangle points//
        Vector2 A = points[0], B = points[0], C = points[0], center = Vector2.zero;
        foreach (var point in points) //A is the highest point of the triangle
        {
            center += point;
            if(point.y > A.y)
            {
                A = point;
            }
        }
        center /= 3;
        foreach (var point in points) //C is the rightest point
        {
            if (point.x > C.x && point != A)
            {
                C = point;
            }
        }
        foreach (var point in points) //point B
        {
            if (point != C && point != A)
            {
                B = point;
                break;
            }
        }

        float height = Vector2.Distance((B+C)/2,A);//height on angle A
        float sideBC = Vector2.Distance(B, C); //length of the opposite side

        if(height > sideBC) // need to determine the capsule orientation for the resizing to work
        {
            fieldCollider.direction = CapsuleDirection2D.Vertical;
            
        }
        else
        {
            fieldCollider.direction = CapsuleDirection2D.Horizontal;
        }

        //rotate the capsule to be parallel to BC side
        float angle = Vector2.Angle(C - B, Vector2.right);
        field.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        //resize the capsule and move to the center of the triangle
        float X = Vector2.Distance(B,center)+Vector2.Distance(C,center);
        float Y = Vector2.Distance(A,center)*2f;
        X += Mathf.Sqrt(area);
        Y += Mathf.Sqrt(area);
        fieldCollider.size = new Vector2(X, Y);
        fieldCollider.transform.localPosition = Vector3.zero;

        //start countdown//
        StartCoroutine(FieldCountdown());
    }
    #endregion

    #region Pentagon Functions
        public void Blades()
    {
        if (bladePrefab == null)
        {
            bladePrefab = Resources.Load<Transform>("Prefabs/Blade");

            if (bladePrefab == null)
            {
                Debug.Log("blade prefab not found!");
                return;
            }

            for(int i = 0; i< 5; i++)
            {
                blades[i] = Instantiate(bladePrefab, this.transform);
                blades[i].localPosition = points[i];
                bladeTarget[i] = i;
                bladeUpdate = true;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                blades[i].gameObject.SetActive(true);
                blades[i].localPosition = points[i];
                bladeTarget[i] = i;
                bladeUpdate = true;
            }
        }
        //Debug.Log("blade timer = " + timer);
        
    }

    public IEnumerator Implosion()
    {
        yield return new WaitForSeconds(2f); //wait for the default shape cooldown

        attractedEnemies = enemies.ToArray();
        SetEnemiesAttraction(true, attractedEnemies);//attract enemies inside the shape
        bombPosition = transform.position; //to set the wave position
        implose = true;

        yield return new WaitForSeconds(0.5f);
        //stop attraction//
        implose = false;
        transform.localScale = Vector3.one;
        SetEnemiesAttraction(false, attractedEnemies);
        //start bomb wave//
        if(wavePrefab == null)
        {
            wavePrefab = Resources.Load<Transform>("Prefabs/ShockWave");
            if(wavePrefab == null)
            {
                Debug.Log("shock wave prefab not found");
                yield break;
            }

            bombWave = Instantiate(wavePrefab, this.transform);
        }
        else
        {
            bombWave.gameObject.SetActive(true);
        }
        bombWave.position = bombPosition;
        waveMaxRadius = Mathf.Sqrt(area/Mathf.PI);
        ShockWave controller = bombWave.gameObject.GetComponent<ShockWave>();
        controller.SetRing(0.1f, 25);
        waveUpdate = true;
        
        
    }

    public void Bomb()
    {
        Vector2 center = transform.position;
        
        if(bombPrefab == null)
        {
            bombPrefab = Resources.Load<Transform>("Prefabs/Bomb");
            if(bombPrefab == null)
            {
                Debug.Log("can't load bomb prefab");
                return;
            }

            
            GameObject bombParent = GameObject.Find("PentagonBombParent");
            if (bombParent == null)
            {
                bombParent = new GameObject("PentagonBombParent");
            }

            for (int i = 0; i < 5; i++)
            {
                bombs[i] = Instantiate(bombPrefab, bombParent.transform);
                bombs[i].position = points[i] + center;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                bombs[i].gameObject.SetActive(true);
                bombs[i].position = points[i] + center;
            }
        }
    }
    private void HideBlades()
    {
        foreach (var blade in blades)
        {
            blade.gameObject.SetActive(false);
        }
    }

    private void SetEnemiesAttraction(bool value, Collider2D[] currentEnemies)
    {
        foreach (var enemy in currentEnemies)
        {
            if (!enemy.gameObject.activeSelf)
            {
                continue;
            }
            var controller = enemy.gameObject.GetComponent<EnemyController>();
            controller.isAttracked = value;
            if(value)
                controller.attractionTarget = transform.position;
            //Debug.Log(enemy.name + "attraction set to " + value);
        }
    }

   
    #endregion
}
