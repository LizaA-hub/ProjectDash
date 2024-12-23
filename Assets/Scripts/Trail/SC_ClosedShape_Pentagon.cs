using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_ClosedShape_Pentagon : ClosedShape
{

    //Pentagon variables//
    [SerializeField]
    private Transform bladePrefab, wavePrefab, bombPrefab;
    private Transform bombWave;
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
                if (Vector3.Distance(blades[i].localPosition, points[bladeTarget[i]]) <= 0.1f)
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
            float step = 10f * Time.deltaTime;
            float radius = Mathf.MoveTowards(controller.currentRadius, waveMaxRadius, step);
            float segment = 25f / 7f * radius + 150f / 7f;
            controller.SetRing(radius, (int)segment);
            if (Mathf.Abs(radius - waveMaxRadius) <= 0.1f)
            {
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

    #region Pentagon Functions
    public void Blades()
    {
        if (blades[0] == null)
        {
            if (bladePrefab == null)
            {
                Debug.Log("blade prefab not found!");
                return;
            }

            for (int i = 0; i < 5; i++)
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
        if (bombWave == null)
        {
            if (wavePrefab == null)
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
        waveMaxRadius = Mathf.Sqrt(area / Mathf.PI);
        ShockWave controller = bombWave.gameObject.GetComponent<ShockWave>();
        controller.SetRing(0.1f, 25);
        waveUpdate = true;


    }

    public void Bomb()
    {
        Vector2 center = transform.position;

        if (bombs[0] == null)
        {
            if (bombPrefab == null)
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
            if (value)
                controller.attractionTarget = transform.position;
            //Debug.Log(enemy.name + "attraction set to " + value);
        }
    }


    #endregion


}
