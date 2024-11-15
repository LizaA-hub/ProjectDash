using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    EdgeCollider2D m_collider;
    public float currentRadius;
 
    private void Start() {
        //lineRenderer = gameObject.GetComponent<LineRenderer>();
        if(lineRenderer == null){
            Debug.Log("line renderer not found!");
        }

        //m_collider = gameObject.GetComponent<EdgeCollider2D>();
        if(m_collider == null){
            Debug.Log("edge collider not found!");
        }        
    }
    public void SetRing(float _radius, int _segments)
	{
        currentRadius = _radius;
		Vector3[] points = new Vector3[_segments + 2];

		for (int i = 0; i < _segments; i++)
		{
			float angle = (i * Mathf.PI * 2) / _segments;

			float x = Mathf.Cos( angle) * _radius;
			float y = Mathf.Sin( angle) * _radius;

			points[i] = new Vector3( x, y, 0);
		}

        for (int i = 0; i < 2; i++)
		{
			float angle = (i * Mathf.PI * 2) / _segments;

			float x = Mathf.Cos( angle) * _radius;
			float y = Mathf.Sin( angle) * _radius;

			points[_segments + i] = new Vector3( x, y, 0);
		}

		lineRenderer.positionCount = _segments + 2;
		lineRenderer.SetPositions( points);

        SetColliderFromLine(points);
	}

    private void SetColliderFromLine(Vector3[] linePoints){
        List<Vector2> colliderPoints = new List<Vector2>();
        if(linePoints.Length == 0){
            colliderPoints.Add(transform.position);
            colliderPoints.Add(transform.position);
        }
        else for (int i = 0; i < linePoints.Length; i++)
        {
            Vector3 point = linePoints[i];
            colliderPoints.Add(point);
        }

        m_collider.SetPoints(colliderPoints);
    }
}
