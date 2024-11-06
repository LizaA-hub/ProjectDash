using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailManager : MonoBehaviour
{
    [SerializeField]
    Material shapeMaterial;
    [SerializeField]
    float trailDuration = 1f, step = 1f;
    TrailRenderer myTrail;
    EdgeCollider2D myCollider;
 
    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

    List<GameObject> closedShapes = new List<GameObject>();
    List<float> shapeTimers = new List<float>();
    GameObject newObject;
    
    #region Unity Functions
    void Awake()
    {
        myTrail = this.GetComponent<TrailRenderer>();   
        myTrail.time = trailDuration;
        myCollider = GetValidCollider();
        GameManager.trailIncrease.AddListener(IncreaseTrailDuration);
    }   
 
    void Update()
    {
        SetColliderPointsFromTrail(myTrail, myCollider);
        CheckForClosedShape();

        UpdateTimers(Time.deltaTime);
    }

    #endregion
    #region Trail Collider
    //Gets from unused pool or creates one if none in pool
    EdgeCollider2D GetValidCollider()
    {
        EdgeCollider2D validCollider;
        if(unusedColliders.Count>0)
        {
            validCollider = unusedColliders[0];
            validCollider.enabled = true;
            unusedColliders.RemoveAt(0);
            if (validCollider != null){
                return validCollider;
            }
        }
        
        validCollider = new GameObject("TrailCollider",typeof(EdgeCollider2D)).GetComponent<EdgeCollider2D>();
        validCollider.isTrigger = true;
        validCollider.gameObject.tag = "PlayerTrail";
        
        return validCollider;
    }
 
    void SetColliderPointsFromTrail(TrailRenderer trail, EdgeCollider2D collider)
    {
        List<Vector2> points = new List<Vector2>();
        //avoid having default points at (-.5,0),(.5,0)
        if(trail.positionCount == 0)
        {
            points.Add(transform.position);
            points.Add(transform.position);
        }
        else for(int position = 0; position<trail.positionCount; position++)
        {
            //ignores z axis when translating vector3 to vector2
            points.Add(trail.GetPosition(position));
        }
        collider.SetPoints(points);
    }
 
    void OnDestroy()
    {
        if(myCollider!=null)
        {
            myCollider.enabled = false;
            unusedColliders.Add(myCollider);
        }
    }
    #endregion

    #region Shape Detection
    void CheckForClosedShape(){
        if(myTrail.positionCount > 3){
            var lastPosition = myTrail.GetPosition(myTrail.positionCount-1);
            int crossPoint = 0;
            bool trailContact = false;
            for (int i = 0; i < myTrail.positionCount-3; i++)
            {
                if(Vector3.Distance(lastPosition,myTrail.GetPosition(i)) < myTrail.minVertexDistance){
                    //Debug.Log("trail closed!");
                    crossPoint = i;
                    trailContact = true;
                }
            }
            //if there's a closed shape//
            if(trailContact){
                //make an array with the points composing the shape//
                Vector2[] points = new Vector2[myTrail.positionCount-crossPoint];
                for (int i = crossPoint, n = 0; i < myTrail.positionCount; i++, n++)
                {
                    points[n] = myTrail.GetPosition(i);
                }

                CreateMesh(points);

            }
          
        }
    }

    void CreateMesh(Vector2[] points){
        bool createGO = false;
        //GameObject newObject;
        int availableGO = -1;
        if(closedShapes.Count > 0){
            for (int i = 0; i < closedShapes.Count; i++)
            {
                if (shapeTimers[i] <= 0f){
                    availableGO = i;
                }
            }
            if (availableGO != -1){
                newObject = closedShapes[availableGO];
                HideShape(newObject,0f); //put the shape back on the map
            }
            else{
                createGO = true;
            }
        }
        else{
            createGO = true;
        }
        
        if(createGO){
            newObject = new GameObject("TrailShape",typeof(PolygonCollider2D),typeof(MeshRenderer),typeof(MeshFilter));
        }

        PolygonCollider2D newCollider = newObject.GetComponent<PolygonCollider2D>();
        MeshFilter meshFilter = newObject.GetComponent<MeshFilter>(); 
        MeshRenderer meshRenderer = newObject.GetComponent<MeshRenderer>();

        newObject.tag = "ClosedShape";
        newCollider.SetPath(0,points);
        newCollider.isTrigger = true;
        var mesh = newCollider.CreateMesh(false,false);
        //Debug.Log(Area(mesh));
        meshFilter.mesh = mesh;
        meshRenderer.material = shapeMaterial;

        float t = 2f;

        if(Area(mesh) > 0.1f){
            //HideShape(newObject,0f);
            myTrail.Clear();
        }
        else{
            HideShape(newObject,100f);
            t = 0f;
        }
        
        if(createGO){
            closedShapes.Add(newObject);
            shapeTimers.Add(t);
        }
        else{
            shapeTimers[availableGO] = t;
        }
        
    }

    void UpdateTimers(float t){
        List<int> toErase = new List<int>();
        for (int i = 0; i < shapeTimers.Count; i++)
        {   
            if(shapeTimers[i] > 0f){
                shapeTimers[i] -= t;           
                if(shapeTimers[i] <= 0){
                    toErase.Add(i);
                }
            }
        }
        if(toErase.Count > 0) foreach (int i in toErase)
        {
            HideShape(closedShapes[i],100f);;
        }
    }

    float Area(Mesh mesh){
        Vector3[] vertices = mesh.vertices;
        Vector3 result = Vector3.zero;
        for (int i = vertices.Length - 1, n = 0 ; n < vertices.Length; i=n++)
        {
            result += Vector3.Cross(vertices[n],vertices[i]);
        }
        result *= 0.5f;
        return result.magnitude;
    }

    void HideShape(GameObject gObject, float f){
        Vector3 newPosition = Vector3.one * f;
        gObject.transform.position = newPosition;

    }
    #endregion

    void IncreaseTrailDuration(int level){
        trailDuration += step*level; 
        myTrail.time = trailDuration;
        ;
    }

}
