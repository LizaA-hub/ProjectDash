using UnityEngine;

public static class GeometricalShape
{
    public enum Shape{Triangle, Square, Pentagon, Hexagon, None};
    public static Shape DetectShape(Vector2[] points){
        //3 sides//
        if(points.Length == 3){
            return Shape.Triangle;
        }
        //4 sides//
        if(points.Length == 4){
            float sideA = Vector2.Distance(points[0],points[1]);
            float sideB = Vector2.Distance(points[1],points[2]);
            float sideC = Vector2.Distance(points[2],points[3]);
            
            if(Mathf.Abs(sideA - sideC) < 1f){
                return Shape.Square;
            }
            else{
                return Shape.None;
            }
            

        }
        //5 sides//
        if(points.Length == 5){
            Vector2[] sides = { points[0] - points[1],points[1]-points[2], points[2] - points[3], points[3] - points[4], points[4] - points[0] };
            float sum = 0f;
            for (int i = 0; i < 5; i++)
            {
                float angle = 0f;
                if(i+1 >= 5)
                {
                    angle = Vector2.Angle(sides[i], -sides[0]);
                }
                else
                {
                    angle = Vector2.Angle(sides[i], -sides[i + 1]);
                }
                sum += angle;
            }
            if(sum != 540f)
            {
                return Shape.None;
            }
            return Shape.Pentagon;
        }

        if(points.Length == 6)
        {
            Vector2[] sides = new Vector2[6];
            for (int i = 0; i < 6; i++)
            {
                if(i+1 < 6)
                {
                    sides[i] = points[i+1] - points[i];
                }
                else
                {
                    sides[i] = points[0] - points[i];
                }
            }
            float sum = 0f;
            for (int i = 0; i < 6; i++)
            {
                float angle = 0f;
                if (i + 1 >= 6)
                {
                    angle = Vector2.Angle(sides[i], sides[0]);
                }
                else
                {
                    angle = Vector2.Angle(sides[i], sides[i + 1]);
                }
                sum += angle;
            }
            if(sum != 360f)
            {
                return Shape.None;
            }
            return Shape.Hexagon;
        }


        return Shape.None;
    }
}
