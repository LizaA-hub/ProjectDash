using UnityEngine;

public static class GeometricalShape
{
    public enum Shape{Triangle, IsoscelesTriangle, Square, Rectangle, Parallelepiped, Pentagram, None};
    public static Shape DetectShape(Vector2[] points){
        //3 sides//
        if(points.Length == 3){
            float sideA = Vector2.Distance(points[0],points[1]);
            float sideB = Vector2.Distance(points[1],points[2]);
            float sideC = Vector2.Distance(points[2],points[0]);
            if(Mathf.Abs(sideA - sideB) < 0.1f && Mathf.Abs(sideC - sideB) < 0.1f){
                return Shape.IsoscelesTriangle;
            }
            else{
                return Shape.Triangle;
            }
        }
        //4 sides//
        if(points.Length == 4){
            float sideA = Vector2.Distance(points[0],points[1]);
            float sideB = Vector2.Distance(points[1],points[2]);
            float sideC = Vector2.Distance(points[2],points[3]);
            
            float angleA = Vector2.Angle(points[0],points[1]);
            float angleB = Vector2.Angle(points[1],points[2]);
            if(Mathf.Abs(angleA - 90) < 0.1f && Mathf.Abs(angleB - 90) < 0.1f){
                //square//
                if(Mathf.Abs(sideA - sideB) < 0.1f){
                    return Shape.Square;
                }
                //rectangle//
                else{
                    return Shape.Rectangle;
                }
            }
            else if(Mathf.Abs(sideA - sideC) < 0.1f){
                return Shape.Parallelepiped;
            }
            else{
                return Shape.None;
            }
            

        }
        //5 sides//
        if(points.Length == 5){
            float med = 0;
            float side = Vector2.Distance(points[0],points[1]);
            for (int i = 0; i < points.Length; i++)
            {
                if(i+1 >= points.Length){
                    med += Vector2.Distance(points[i],points[0]);
                }
                else{
                    med += Vector2.Distance(points[i],points[i+1]);
                }
            }

            if(Mathf.Abs(side-med/5)<0.1f){
                return Shape.Pentagram;
            }
            else{
                return Shape.None;
            }
        }

        return Shape.None;
    }
}
