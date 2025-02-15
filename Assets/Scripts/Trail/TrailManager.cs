using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailManager : MonoBehaviour
{
    [SerializeField]
    Material shapeMaterial;
    [SerializeField]
    bool DebugShape = true;
    [SerializeField]
    Transform[] shapePool;
    TrailRenderer myTrail;
    EdgeCollider2D myCollider;
 
    static List<EdgeCollider2D> unusedColliders = new List<EdgeCollider2D>();

    List<Transform> closedShapes = new List<Transform>();
    List<float> shapeTimers = new List<float>();
    Transform newObject;
    GameObject closedShapeParent;
    
    #region Unity Functions
    void Start()
    {
        myTrail = this.GetComponent<TrailRenderer>();   
        myTrail.time = GameManagerV2.instance.skills.trailDuration;
        myCollider = GetValidCollider();
        PowerUpManager.trailIncrease.AddListener(IncreaseTrailDuration);
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
        if((unusedColliders.Count>0) && (unusedColliders[0] != null))
        {
            validCollider = unusedColliders[0];
            validCollider.enabled = true;
            unusedColliders.RemoveAt(0);
            
            return validCollider;
            
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
                if(Vector3.Distance(lastPosition,myTrail.GetPosition(i)) < 0.2f ){ //testing out myTrail.minVertexDistance
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
                
                Vector2[] anglePoints = FindAngles(points).ToArray();
                if(anglePoints.Length > 2){

                    //find the  center of the shape//
                    Vector2 center = Vector2.zero;
                    foreach (var point in anglePoints)
                    {
                        center += point;
                    }
                    center /= anglePoints.Length;

                    //translating all points based on center//
                    for (int i = 0; i < anglePoints.Length; i++)
                    {
                        anglePoints[i] -= center;
                    }

                    //create shape object//
                    CreateMesh(anglePoints, center);
                }
                

            }
          
        }
    }

    void CreateMesh(Vector2[] points, Vector2 position){
        bool createGO = false;
        //GameObject newObject;
        int availableGO = -1;
        GeometricalShape.Shape shape = GeometricalShape.DetectShape(points);

        if (shape == GeometricalShape.Shape.Hexagon && GameManagerV2.instance.skills.hexagonArea > 0f) {
            points = ExtendShape(points, position);
        }

        //check if there's an instantiated shape available//
        if (closedShapes.Count > 0){
            for (int i = 0; i < closedShapes.Count; i++)
            {
                ClosedShape shapeController = closedShapes[i].GetComponent<ClosedShape>();
                if ((shapeTimers[i] <= 0f) && (shapeController.shape == shape)){
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
        //if not create one//
        if (createGO)
        {
            if(closedShapeParent == null)
            {
                closedShapeParent = new GameObject("ClosedShapeParent");
            }
            bool prefabFound = false;
            foreach (var prefab in shapePool)
            {
                ClosedShape controller = prefab.GetComponent<ClosedShape>();
                if (controller.shape == shape)
                {
                    newObject = Instantiate(prefab,closedShapeParent.transform);
                    prefabFound = true;
                    break;
                }
            }

            if (!prefabFound)
            {
                foreach (var prefab in shapePool)
                {
                    ClosedShape controller = prefab.GetComponent<ClosedShape>();
                    if (controller.shape == GeometricalShape.Shape.None)
                    {
                        newObject = Instantiate(prefab, closedShapeParent.transform);
                        break;
                    }
                }
            }

        }

        PolygonCollider2D newCollider = newObject.GetComponent<PolygonCollider2D>();
        MeshFilter meshFilter = newObject.GetComponent<MeshFilter>(); 
        ClosedShape closedShape = newObject.GetComponent<ClosedShape>();

        newCollider.SetPath(0,points);
        var mesh = newCollider.CreateMesh(false,false);
        meshFilter.mesh = mesh;
        newObject.position = position;
        if(closedShape.shape != shape)
        {
            closedShape.shape = shape;
        }

        float t = 2f;

        if(Area(points) > 0.5f){
            myTrail.Clear();

            if (DebugShape){
                Debug.Log(shape);
            }

            closedShape.points = points;
            closedShape.area = Area(points);
            if (shape == GeometricalShape.Shape.Triangle)
            {
                SC_ClosedShape_Triangle controller = newObject.GetComponent<SC_ClosedShape_Triangle>();
                StartCoroutine(controller.HasField(GameManagerV2.instance.skills.triangleGravity));
            }
            else if (shape == GeometricalShape.Shape.Square)
            {
                GameManagerV2.instance.ModifyHealth(GameManagerV2.instance.skills.squareHeal);
                if(GameManagerV2.instance.skills.squareSlow > 0f) { 
                t += 5f;}
            }
            else if((shape == GeometricalShape.Shape.Pentagon))
            {
                SC_ClosedShape_Pentagon controller = newObject.GetComponent<SC_ClosedShape_Pentagon>();
                if (GameManagerV2.instance.skills.pentagonBlade)
                {

                    controller.Blades();
                }
                if(GameManagerV2.instance.skills.pentagonCriticalChance > 0f)
                {
                    StartCoroutine(controller.Implosion());
                    t += 0.5f;
                }
                if (GameManagerV2.instance.skills.pentagonBomb)
                {
                    controller.Bomb();
                }
            }
            else if ((shape == GeometricalShape.Shape.Hexagon))
            {
                SC_ClosedShape_Hexagon controller = newObject.GetComponent<SC_ClosedShape_Hexagon>();
                if(GameManagerV2.instance.skills.hexagonMeteor > 0f)
                {
                    t += GameManagerV2.instance.skills.hexagonMeteor;
                    StartCoroutine(controller.Meteor());
                }
                if (GameManagerV2.instance.skills.hexagonLightning)
                {
                    StartCoroutine(controller.FireLightning());
                }
                
            }

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
            HideShape(closedShapes[i],100f);
        }
    }

    float Area(Vector2[] points)
    {
        float result = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 v = points[i];
            Vector2 w = Vector2.zero;
            if (i + 1 >= points.Length)
            {
                w = points[0];
            }
            else
            {
                w = points[i + 1];
            }
            //Debug.Log("from " + v + " to " + w);
            float width = w.x - v.x;
            //Debug.Log("width = " + width);
            float height = ((v + w) / 2).y;
            //Debug.Log("heigth = " + height);
            float area = width * height;
            result += area;
        }

        return Mathf.Abs(result);
    }

    void HideShape(Transform gObject, float f){
        Vector3 newPosition = Vector3.one * f;
        gObject.position = newPosition;

    }
    private List<Vector2> FindAngles(Vector2[] points){
        List<Vector2> anglePoints = new List<Vector2>();
        float[] angles = new float[points.Length];
        Vector2 vA = Vector2.zero;
        Vector2 vB = Vector2.zero;
        //calculate the angle at each point//
        for (int i = 0; i < points.Length; i++)
        {
            if(i-1 < 0){
                vA = points[points.Length-1] - points[i];
            }
            else{
                vA = points[i-1] - points[i];
            }

            if(i+1 >= points.Length){
                vB = points[0] - points[i];
            }
            else{
                vB = points[i+1] - points[i];
            }

            float angle = Vector2.Angle(vA,vB); //angle in degre
            angles[i] = angle;
        }

        //comparing each angle//
        for (int i = 0; i < points.Length; i++)
        {
            float Min = 0;
            if(i-1 < 0){
                if(i+1 >= points.Length){
                    Min = Mathf.Min(angles[i],angles[angles.Length -1],angles[0]);
                }
                else{
                    Min = Mathf.Min(angles[i],angles[angles.Length -1],angles[i+1]);
                }
            }
            else{
                if(i+1 >= points.Length){
                    Min = Mathf.Min(angles[i],angles[i-1],angles[0]);
                }
                else{
                    Min = Mathf.Min(angles[i],angles[i-1],angles[i+1]);
                }
            }
            //adding the angle to anglePoints if it's a corner//
            if(angles[i] < 170f && Min == angles[i]){
                anglePoints.Add(points[i]);
            }
        }

        /*if(anglePoints.Count > 0){
            Debug.Log("there is " + anglePoints.Count + " angles.");
        }*/

        return anglePoints;

    }

    private Vector2[] ExtendShape(Vector2[] points, Vector2 center) {
        Vector2[] newPoints = new Vector2[points.Length];
        float factor = GameManagerV2.instance.skills.hexagonArea;
        for (int i = 0; i < points.Length; i++)
        {
            float r = points[i].magnitude;
            newPoints[i] = points[i].normalized * r * (1 - factor + factor * r);
        }
        return newPoints; }
    #endregion

    void IncreaseTrailDuration(float value){
        myTrail.time = value;
        
    }

}
