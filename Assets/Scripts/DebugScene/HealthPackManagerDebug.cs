using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackManagerDebug : HealthPackManager
{
    private void Update()
    {
        //cancel the count down
    }
    public void SpawnPack()
    {
        Debug.Log("Health pack spawned");
        SpawnHealthPack();
    }
}
