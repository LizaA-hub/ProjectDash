using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SC_ClosedShape_Hexagon: ClosedShape
{
    [SerializeField]
    Transform meteorPrefab;
    [SerializeField]
    MeshRenderer m_renderer;
    [SerializeField]
    PolygonCollider2D m_collider;
    float timer, meteorDuration;
    private List<Transform> meteors = new List<Transform>();
    private List<EnemyController> enemiesTouched = new List<EnemyController>();
    private Transform newMeteor;
    private Vector2 maxCoord, minCoord;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var controller = collision.gameObject.GetComponent<EnemyController>();
            if(controller != null)
            {
                enemiesTouched.Add(controller);
            }
        }
    }

    #region Meteor Functions
    public IEnumerator Meteor()
    {
        meteorDuration = GameManager.skillVariables.hexagonMeteor;
        SetShapeBounds();
        yield return new WaitForSeconds(2f);
        m_renderer.enabled = false;
        m_collider.enabled = false;
        while (timer < meteorDuration) {
            float delay = Random.Range(0f, meteorDuration/2f);
            SpawnMeteor();
            timer += delay;
            yield return new WaitForSeconds(delay);
        }
        m_renderer.enabled = true;
        m_collider.enabled = true;
        timer = 0f;
    }

    private void SpawnMeteor()
    {
        bool createObject = true;
        if (meteors.Count > 0) {
            for (int i = 0; i < meteors.Count; i++)
            {
                if (!meteors[i].gameObject.activeSelf)
                {
                    newMeteor = meteors[i];
                    newMeteor.gameObject.SetActive(true);
                    createObject = false;
                    break;
                }
            }
        }
        if (createObject)
        {
            newMeteor = Instantiate(meteorPrefab, this.transform);
        }

        newMeteor.localPosition = GetRandomPosition();
    }
    
    private Vector3 GetRandomPosition()
    {
        Vector2 meteorPosition = Vector2.zero;
        meteorPosition.y = Random.Range(minCoord.y, maxCoord.y);
        meteorPosition.x = Random.Range(minCoord.x, maxCoord.x);
        while (!IsInShape(meteorPosition)) {
            meteorPosition.x = Random.Range(minCoord.x, maxCoord.x);
        }
        return new Vector3(meteorPosition.x,meteorPosition.y,0f);
    }

    private void SetShapeBounds()
    {
        minCoord = maxCoord = points[0];
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].x < minCoord.x)
            {
                minCoord.x = points[i].x;
            }
            if (points[i].x > maxCoord.x)
            {
                maxCoord.x = points[i].x;
            }
            if (points[i].y < minCoord.y)
            {
                minCoord.y = points[i].y;
            }
            if (points[i].y > maxCoord.y)
            {
                maxCoord.y = points[i].y;
            }
        }
    }

    private bool IsInShape(Vector2 pos)
    {
        int count = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 A = points[i];
            Vector2 B = (i+1)>=points.Length? points[0] : points[i + 1];
            if (((pos.y < A.y) != (pos.y < B.y)) && (pos.x < A.x + ((pos.y - A.y) / (B.y - A.y)) * (B.x - A.x)))
            {
                count++;
            }
        }
        if (count % 2 == 0) {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    public IEnumerator FireLightning()
    {
        yield return new WaitForSeconds(2f);
        float damage = GameManager.skillVariables.lightningDamage*(1f+enemiesTouched.Count*0.1f);
        foreach (var enemy in enemiesTouched)
        {
            if (enemy.gameObject.activeSelf) {
                enemy.TakeDamage(damage, true);
            }
        }

        enemiesTouched.Clear();
    }
}
