using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    float timer = 2f;
    Transform explosionArea;
    SpriteRenderer m_renderer;
    Color normal, red = Color.red;
  
    private void OnEnable()
    {
        if (explosionArea == null) {
            explosionArea = transform.GetChild(0);
        }

        timer = 2f;

        if (m_renderer == null) {
            m_renderer = GetComponent<SpriteRenderer>();
            normal = m_renderer.color;
        }
        m_renderer.enabled = true;

        StartCoroutine(Timer());
    }

    private void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        
        else{
            StartCoroutine(Explode());
        }
    }

    IEnumerator Timer()
    {
        while(timer > 0f)
        {
            m_renderer.color = red;
            yield return new WaitForSeconds(0.5f);
            m_renderer.color = normal;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Explode(){
        m_renderer.color = normal;
        m_renderer.enabled = false;
        explosionArea.gameObject.SetActive(true);
        explosionArea.localScale = Vector3.one * GameManager.bombRadius;
        yield return new WaitForSeconds(0.5f);
        explosionArea.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
