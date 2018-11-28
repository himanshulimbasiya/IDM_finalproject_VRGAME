using UnityEngine;
//using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SoxAtkMotionPath))]
public class SoxAtkMotionPathEditor : Editor
{
    private SoxAtkMotionPath m_motionPath;
    private AnimationClip[] m_animClips;
    private string[] m_animClipNames;

    private void OnEnable()
    {
        m_motionPath = (SoxAtkMotionPath)target;
        if (!m_motionPath.m_animator)
            return;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please note that applying Animator will cause actual changes to the target objects of Animator. (Animator를 적용하면 Animator의 대상 오브젝트들에 실제로 변경이 발생하므로 주의해주세요.)", MessageType.Info);

        Undo.RecordObject(target, "MotionPath Changed Settings");

        EditorGUILayout.BeginHorizontal();
        if (!m_motionPath.m_motionPathObject || m_motionPath.m_animator == null)
            GUI.enabled = false;
        if (GUILayout.Button("Create MotionPath"))
        {
            UpdateMotionPath(m_motionPath, m_animClips);
        }
        GUI.enabled = true;

        if (GUILayout.Button("Clear MotionPath"))
        {
            Clear(m_motionPath);
            SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();

        if (m_motionPath.m_animator == null)
        {
            EditorGUILayout.HelpBox("Please assign Animator", MessageType.Info);
            DrawDefaultInspector();
            Undo.FlushUndoRecordObjects();
            return;
        }

        // m_motionPath.m_animator가 Null이 아니더라도 Missing prefab 등의 상황에서 runtimeAnimatorController는 null 일 수 있다
        if (m_motionPath.m_animator.runtimeAnimatorController == null)
        {
            EditorGUILayout.HelpBox("There is no runtimeAnimatorController in Animator", MessageType.Info);
            DrawDefaultInspector();
            Undo.FlushUndoRecordObjects();
            return;
        }

        m_animClips = m_motionPath.m_animator.runtimeAnimatorController.animationClips;
        m_animClipNames = new string[m_animClips.Length];
        for (int i = 0; i < m_animClips.Length; i++)
        {
            m_animClipNames[i] = m_animClips[i].name;
        }

        // 외부 요인에 의해 애니메이션 클립 수가 변경된 경우 에러를 막아준다. 예를 들어 갑자기 애니메이터가 바뀌었는데 애니메이션이 전혀 없다거나.
		m_motionPath.m_animClipIndex = Mathf.Min(m_motionPath.m_animClipIndex, m_animClips.Length - 1);
		if (m_motionPath.m_animClipIndex < 0)
        {
            EditorGUILayout.HelpBox("There is no animation in Animator", MessageType.Info);
            m_motionPath.m_animClipIndex = 0; // 0으로 초기화
            DrawDefaultInspector();
            Undo.FlushUndoRecordObjects();
            return;
        }

        // Select Animation
        m_motionPath.m_animClipIndex = EditorGUILayout.Popup("Animation Clip", m_motionPath.m_animClipIndex, m_animClipNames);

        // Animation Slider
        m_animClips[m_motionPath.m_animClipIndex].SampleAnimation(m_motionPath.m_animator.gameObject, m_motionPath.m_animationSlider);

        Undo.FlushUndoRecordObjects();

        DrawDefaultInspector();

        // Auto Update
        if (m_motionPath.m_autoUpdate && m_motionPath.m_initialized)
            UpdateMotionPath(m_motionPath, m_animClips);

        // GUI레이아웃 끝========================================================
    } // end of OnInspectorGUI()

    private void UpdateMotionPath(SoxAtkMotionPath motionPath, AnimationClip[] animClips)
    {
        if (!motionPath.m_motionPathObject)
            return;

        if (motionPath.m_animator == null)
            return;

        if (motionPath.m_pathPositions.Length != motionPath.m_pathStep)
        {
            motionPath.m_pathPositions = new Vector3[motionPath.m_pathStep];
        }

        for (int i = 0; i < motionPath.m_pathStep; i++)
        {
            float stepTime = (float)i / ((float)motionPath.m_pathStep - 1); // 0 ~ 1
            stepTime = Mathf.Lerp(motionPath.m_timeStart, motionPath.m_timeEnd, stepTime); // Min ~ Max
            animClips[motionPath.m_animClipIndex].SampleAnimation(motionPath.m_animator.gameObject, stepTime);
            
            // 애니메이터가 적용된 오브젝트의 로컬 스페이스로 포지션 배열을 저장함
            motionPath.m_pathPositions[i] = motionPath.m_animator.transform.InverseTransformPoint(motionPath.m_motionPathObject.position);
        }

        animClips[motionPath.m_animClipIndex].SampleAnimation(motionPath.m_animator.gameObject, motionPath.m_animationSlider);
        motionPath.m_initialized = true;
    }

    private void Clear(SoxAtkMotionPath motionPath)
    {
        motionPath.m_pathPositions = new Vector3[2];
        motionPath.m_initialized = false;
    }
}
