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
        
        //GENERAL//
        general = EditorGUILayout.Foldout(general, "General Stats");
        if (general) {
            script.maxHealth = EditorGUILayout.FloatField("Max Health",script.maxHealth, GUILayout.ExpandWidth(false));
            EditorGUILayout.Space(5);
            script.dashSpeed = EditorGUILayout.FloatField("Dash Speed", script.dashSpeed, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("XP To Next Level");
            script.xpGoal = EditorGUILayout.FloatField(script.xpGoal);
            GUILayout.Label("Grow Factor");
            script.xpGoalGrowFactor = EditorGUILayout.FloatField(script.xpGoalGrowFactor);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        //DASH//
        dash = EditorGUILayout.Foldout(dash, "Dash Stats");
        if (dash)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dash Attacks Cooldown");
            script.dashCooldown = EditorGUILayout.FloatField(script.dashCooldown);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Trail Damage");
            script.trailDamage = EditorGUILayout.FloatField(script.trailDamage);
            GUILayout.Label("Trail Duration");
            script.trailDuration = EditorGUILayout.FloatField(script.trailDuration);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            script.projectileDamage = EditorGUILayout.FloatField("Projectile Damage", script.projectileDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wave Damage");
            script.waveDamage = EditorGUILayout.FloatField(script.waveDamage);
            GUILayout.Label("Wave Radius");
            script.waveRadius = EditorGUILayout.FloatField(script.waveRadius);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            script.swordDamage = EditorGUILayout.FloatField("Sword Damage", script.swordDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Bomb Damage");
            script.bombDamage = EditorGUILayout.FloatField(script.bombDamage);
            GUILayout.Label("Bomb Radius");
            script.bombRadius = EditorGUILayout.FloatField(script.bombRadius);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        //TRIANGLE//
        triangle = EditorGUILayout.Foldout(triangle, "Triangle Stats");
        if (triangle)
        {
            script.triangleDamage = EditorGUILayout.FloatField("Triangle Damage", script.triangleDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gravity Triangle Duration");
            script.triangleGravityduration = EditorGUILayout.FloatField(script.triangleGravityduration);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Triangle DOT");
            script.triangleDOT = EditorGUILayout.FloatField(script.triangleDOT);
            GUILayout.Label("Triangle DOT interval");
            script.triangleDOTInterval = EditorGUILayout.FloatField(script.triangleDOTInterval);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        //SQUARE//
        square = EditorGUILayout.Foldout(square, "Square Stats");
        if (square)
        {
            script.squareDamage = EditorGUILayout.FloatField("Square Damage", script.squareDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Square Flame Damage");
            script.squareFlameDamage = EditorGUILayout.FloatField(script.squareFlameDamage);
            GUILayout.Label("Flame Damage Interval");
            script.squareFlameDamageInterval = EditorGUILayout.FloatField(script.squareFlameDamageInterval);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            script.squareHeal = EditorGUILayout.FloatField("Square Heal", script.squareHeal, GUILayout.ExpandWidth(false));
        }

        EditorGUILayout.Space(10);
        //PENTAGON//
        pentagon = EditorGUILayout.Foldout(pentagon, "Pentagon Stats");
        if (pentagon)
        {
            script.pentagonDamage = EditorGUILayout.FloatField("Pentagon Damage", script.pentagonDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Blades Damage");
            script.pentagonBladesDamage = EditorGUILayout.FloatField(script.pentagonBladesDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Implosion Base Damage");
            script.pentagonImplosionBaseDamage = EditorGUILayout.FloatField(script.pentagonImplosionBaseDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Implosion Critical Damage");
            script.pentagonImplosionCriticalDamage = EditorGUILayout.FloatField(script.pentagonImplosionCriticalDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Life Drain");
            script.pentagonDrain = EditorGUILayout.FloatField(script.pentagonDrain);
            GUILayout.Label("Pentagon Drain Chance");
            script.pentagonDrainChance = EditorGUILayout.FloatField(script.pentagonDrainChance);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pentagon Bomb Damage");
            script.pentagonBombDamage = EditorGUILayout.FloatField(script.pentagonBombDamage);
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);
        //HEXAGON//
        hexagon = EditorGUILayout.Foldout(hexagon, "Hexagon Stats");
        if (hexagon)
        {
            script.hexagonDamage = EditorGUILayout.FloatField("Hexagon Damage", script.hexagonDamage, GUILayout.ExpandWidth(false));

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hexagon Meteor Damage");
            script.hexagonMeteorDamage = EditorGUILayout.FloatField(script.hexagonMeteorDamage);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hexagon Lightning Damage");
            script.hexagonLightningDamage = EditorGUILayout.FloatField(script.hexagonLightningDamage);
            GUILayout.EndHorizontal();
        }

    }
}
