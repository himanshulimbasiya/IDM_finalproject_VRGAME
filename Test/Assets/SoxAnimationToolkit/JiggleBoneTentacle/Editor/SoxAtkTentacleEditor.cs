using UnityEngine;
//using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SoxAtkTentacle))]
public class SoxAtkTentacleEditor : Editor
{
    private SoxAtkTentacle tentacle;

    void OnEnable()
    {
        tentacle = (SoxAtkTentacle)target;

        // 프로젝트 창에서 선택한 프리팹을 버전체크하면 문제가 발생한다. Selection.transforms.Length가 0이면 Project View 라는 뜻
        if (Selection.transforms.Length > 0 && Application.isPlaying && tentacle.gameObject.activeInHierarchy)
        {
            tentacle.MyValidate();
        }
    }

    public override void OnInspectorGUI()
    {
        tentacle = (SoxAtkTentacle)target;

        // GUI레이아웃 시작=======================================================
        Undo.RecordObject(target, "Tentacle Changed Settings");
        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Auto register nodes"))
        {
            AutoRegisterNodes(tentacle);
        }

        if (GUILayout.Button("Clear nodes"))
        {
            ClearNodes(tentacle);
        }
        GUILayout.EndHorizontal();

        tentacle.m_animated = EditorGUILayout.Toggle(new GUIContent("Animated", "This should only be used if Animation is active. Even if it is not Animation, it uses all the changes that operate on Update(). Please use it only when necessary, as it may affect performance."), tentacle.m_animated);
        if (tentacle.m_animated)
            GUI.enabled = false;
        tentacle.m_keepInitialRotation = EditorGUILayout.Toggle("Keep Initial Rotation", tentacle.m_keepInitialRotation);
        GUI.enabled = true;

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            // 프로젝트 창에서 선택한 프리팹을 버전체크하면 문제가 발생한다. Selection.transforms.Length가 0이면 Project View 라는 뜻
            if (Selection.transforms.Length > 0 && Application.isPlaying && tentacle.gameObject.activeInHierarchy)
            {
                tentacle.MyValidate();
            }
        }
        Undo.FlushUndoRecordObjects();

        // GUI레이아웃 끝========================================================
    } // end of OnInspectorGUI()

    private void AutoRegisterNodes(SoxAtkTentacle tentacle)
    {
        for (int i = 1; i < tentacle.m_nodes.Length; i++)
        {
            if (tentacle.m_nodes[i] == null && tentacle.m_nodes[i - 1] != null)
            {
                if (tentacle.m_nodes[i - 1].childCount > 0)
                {
                    tentacle.m_nodes[i] = tentacle.m_nodes[i - 1].GetChild(0);
                }
            }
        }
    }

    private void ClearNodes(SoxAtkTentacle tentacle)
    {
        for (int i = 0; i < tentacle.m_nodes.Length; i++)
        {
            tentacle.m_nodes[i] = null;
        }
    }
}
