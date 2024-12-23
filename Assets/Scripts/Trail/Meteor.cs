using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private float timer;
    private void OnEnable()
    {
        timer = 0.5f;   
    }
    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
