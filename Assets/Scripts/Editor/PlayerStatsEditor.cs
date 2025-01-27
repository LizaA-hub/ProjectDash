using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(PlayerStatsScriptableObject))]
public class PlayerStatsEditor : Editor
{
    bool general = false, dash = false, triangle = false, square = false, pentagon = false, hexagon = false;
    private GUIStyle titleStyle;


    public override void OnInspectorGUI()
    {
        titleStyle = EditorStyles.foldout;
        titleStyle.fontStyle = FontStyle.Bold;

        PlayerStatsScriptableObject script = (PlayerStatsScriptableObject)target;

        EditorGUILayout.LabelField("All these variables are initial stats that can be improved with upgrades or skills");
        
        general = EditorGUILayout.Foldout(general, "General Stats");
        if (general) {
            EditorGUILayout.FloatField("Max Health",script.maxHealth, GUILayout.ExpandWidth(false));
            EditorGUILayout.Space(5);
            EditorGUILayout.FloatField("Dash Speed", script.dashSpeed, GUILayout.ExpandWidth(false));
        }

        EditorGUILayout.Space(10);
        dash = EditorGUILayout.Foldout(dash, "Dash Attacks");
        if (dash)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dash Attacks Cooldown");
            EditorGUILayout.FloatField(script.dashCooldown);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Trail Damage");
            EditorGUILayout.FloatField(script.trailDamage);
            GUILayout.Label("Trail Duration");
            EditorGUILayout.FloatField(script.trailDuration);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.FloatField("Projectile Damage", script.projectileDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wave Damage");
            EditorGUILayout.FloatField(script.waveDamage);
            GUILayout.Label("Wave Radius");
            EditorGUILayout.FloatField(script.waveRadius);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            EditorGUILayout.FloatField("Sword Damage", script.swordDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Bomb Damage");
            EditorGUILayout.FloatField(script.bombDamage);
            GUILayout.Label("Bomb Radius");
            EditorGUILayout.FloatField(script.bombRadius);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        triangle = EditorGUILayout.Foldout(triangle, "Triangle Stats");
        if (triangle)
        {
            EditorGUILayout.FloatField("Triangle Damage", script.triangleDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gravity Triangle Duration");
            EditorGUILayout.FloatField(script.triangleGravityduration);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        square = EditorGUILayout.Foldout(square, "Square Stats");
        if (square)
        {
            EditorGUILayout.FloatField("Square Damage", script.squareDamage, GUILayout.ExpandWidth(false));

        }

        EditorGUILayout.Space(10);
        pentagon = EditorGUILayout.Foldout(pentagon, "Pentagon Attacks");
        if (pentagon)
        {
            EditorGUILayout.FloatField("Pentagon Damage", script.pentagonDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Blades Damage");
            EditorGUILayout.FloatField( script.pentagonBladesDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Implosion Base Damage");
            EditorGUILayout.FloatField(script.pentagonImplosionBaseDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Implosion Critical Damage");
            EditorGUILayout.FloatField(script.pentagonImplosionCriticalDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Bomb Damage");
            EditorGUILayout.FloatField(script.pentagonBombDamage);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        hexagon = EditorGUILayout.Foldout(hexagon, "Hexagon Attacks");
        if (hexagon)
        {
            EditorGUILayout.FloatField("Hexagon Damage", script.hexagonDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hexagon Meteor Damage");
            EditorGUILayout.FloatField(script.hexagonMeteorDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hexagon Lightning Damage");
            EditorGUILayout.FloatField(script.hexagonLightningDamage);
            GUILayout.EndHorizontal();
        }

    }
}
