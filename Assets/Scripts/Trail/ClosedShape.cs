using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedShape : MonoBehaviour
{
    public GeometricalShape.Shape shape;
    public Vector2[] points;
    public float area;
    private GameObject field;
    private CapsuleCollider2D fieldCollider;
    private bool enemyOverlap = false;

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

        //resize the capsule and move to the center of the triangle(not always the same as the parent transform)
        float X = Vector2.Distance(B,center)+Vector2.Distance(C,center);
        float Y = Vector2.Distance(A,center)*2f;
        X += Mathf.Sqrt(area);
        Y += Mathf.Sqrt(area);
        fieldCollider.size = new Vector2(X, Y);
        fieldCollider.transform.position = center;

        //start countdown//
        StartCoroutine(FieldCountdown());
    }
    #endregion
}
