using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelEditor :  Editor
{
    GUIContent
        insertContent = new GUIContent("+", "增加元素"),
        deleteContent = new GUIContent("-", "删除元素"),
        noContent = GUIContent.none;

    GUILayoutOption
        shortButtonWidth = GUILayout.MaxWidth(25f),
        longButtonWidth = GUILayout.MaxWidth(1000f);

    SerializedObject targetLevel;
    SerializedProperty balloonCoordProp;
    SerializedProperty balloonPrefabProp;
    SerializedProperty randomBalloonPrefabProp;
    SerializedProperty randomStartBalloonProp;
    SerializedProperty targetScoreProp;

    void OnEnable()
	{
        balloonCoordProp = serializedObject.FindProperty("elementsCoord");
        balloonPrefabProp = serializedObject.FindProperty("balloonPrefab");
        randomBalloonPrefabProp = serializedObject.FindProperty("randomBalloonPrefab");
        randomStartBalloonProp = serializedObject.FindProperty("randomStartBalloon");
        targetScoreProp = serializedObject.FindProperty("targetScore");
    }

	public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(targetScoreProp);
        EditorGUILayout.PropertyField(randomStartBalloonProp);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("随机气球");
        EditorGUILayout.IntField(randomBalloonPrefabProp.arraySize);
        if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, shortButtonWidth))
        {
            randomBalloonPrefabProp.InsertArrayElementAtIndex(randomBalloonPrefabProp.arraySize);
        }
        if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, shortButtonWidth))
        {
            randomBalloonPrefabProp.DeleteArrayElementAtIndex(randomBalloonPrefabProp.arraySize-1);
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < randomBalloonPrefabProp.arraySize; i++)
        {
            SerializedProperty prefab = randomBalloonPrefabProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(prefab);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("气球数量:");
        GUILayout.Label(balloonCoordProp.arraySize.ToString());
        if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, longButtonWidth))
        {
            balloonCoordProp.InsertArrayElementAtIndex(balloonCoordProp.arraySize);
            balloonPrefabProp.InsertArrayElementAtIndex(balloonPrefabProp.arraySize);
        }
        EditorGUILayout.EndHorizontal();

        for (int i=0;i< balloonCoordProp.arraySize;i++)
        {
            EditorGUILayout.BeginHorizontal();
            //坐标
            SerializedProperty point = balloonCoordProp.GetArrayElementAtIndex(i);
            GUILayout.Label("X");
            EditorGUILayout.PropertyField(point.FindPropertyRelative("x"), noContent);
            GUILayout.Label("Y");
            EditorGUILayout.PropertyField(point.FindPropertyRelative("y"), noContent);
            //实例
            GUILayout.Label("气球");
            SerializedProperty prefab = balloonPrefabProp.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(prefab,noContent);
            //增减按钮
            if (GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, shortButtonWidth))
            {
                balloonCoordProp.InsertArrayElementAtIndex(i);
                balloonPrefabProp.InsertArrayElementAtIndex(i);
            }
            if (GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, shortButtonWidth))
            {
                balloonCoordProp.DeleteArrayElementAtIndex(i);
                balloonPrefabProp.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        this.serializedObject.ApplyModifiedProperties();
    }
}
