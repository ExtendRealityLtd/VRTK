using UnityEngine;
using UnityEditor;
using VRTK;

[CustomEditor(typeof(VRTK_InteractableObject))]
public class VRTK_InteractableObject_Editor : Editor
{
	void OnInspectorGUI() {
		GUILayout.Label("Model Override");
	}
}



/*


 @CustomEditor(Enemy)
 class EnemyEditor extends Editor {
     private var showParams : boolean = true;
     function OnInspectorGUI() {
         showParams = EditorGUILayout.Foldout(showParams, "Enemy Parameters");
         if(showParams) {
             target.Health = EditorGUILayout.FloatField("Health", target.Health);
             target.Damage = EditorGUILayout.FloatField("Damage", target.Damage);
             target.moveSpeed = EditorGUILayout.FloatField("moveSpeed", target.moveSpeed);
             target.normalSpeed = EditorGUILayout.FloatField("normalSpeed", target.normalSpeed);
         }
     }
 }

 * */