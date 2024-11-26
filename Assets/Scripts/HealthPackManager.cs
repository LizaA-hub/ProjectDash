using System.Collections.Generic;
using UnityEngine;

public class HealthPackManager : MonoBehaviour
{
    [SerializeField]
    Transform squarePrefab, trianglePrefab, map;
    float timer = 60f;
    Vector2 spawningPoint;
    bool countdown = true;
    List<Transform> instantiatedTransform = new List<Transform>();
    Transform newHealthPack;

    private void Update()
    {
        if (countdown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SpawnHealthPack();
                timer = 60f;
                countdown = false;
            }
        }
    }

    public void RemoveHealthPack(Transform healthPack)
    {
        healthPack.gameObject.SetActive(false);
        countdown = true;
    }

    protected void SpawnHealthPack()
    {
        float random = Random.Range(0f, 1f);
        if(random <= 0.5f)
        {
            int Id = HasType(PackType.Square);
            if (Id > -1) { 
                newHealthPack = instantiatedTransform[Id];
                newHealthPack.gameObject.SetActive(true);
            }
            else
            {
                newHealthPack = Instantiate(squarePrefab, transform);
                instantiatedTransform.Add(newHealthPack);
            }
            
        }
        else
        {
            int Id = HasType(PackType.Triangle);
            if (Id > -1)
            {
                newHealthPack = instantiatedTransform[Id];
                newHealthPack.gameObject.SetActive(true);
            }
            else
            {
                newHealthPack = Instantiate(trianglePrefab, transform);
                instantiatedTransform.Add(newHealthPack);
            }
        }

        float x = Random.Range(0f, map.lossyScale.x/2 - 2f);
        float y = Random.Range(0f, map.lossyScale.y/2 - 2f);

        newHealthPack.position = new Vector3(x, y, 0f);
    }

    private int HasType(PackType type)
    {
        if (instantiatedTransform.Count > 0) {
            for (int i = 0;i<instantiatedTransform.Count;i++) {
                HealthPack controller = instantiatedTransform[i].gameObject.GetComponent<HealthPack>();
                if (controller.type == type){
                    return i;
                }
            }
            
        }
        return -1;
    }
}
