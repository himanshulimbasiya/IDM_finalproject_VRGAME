using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoxAtkMotionPath : MonoBehaviour {
    [HideInInspector]
    public float m_version = 1.071f;

    [HideInInspector]
    public bool m_initialized = false;

    public Animator m_animator;
    public Transform m_motionPathObject;
    [HideInInspector]
    public int m_animClipIndex = 0;

    public bool m_autoUpdate = true;

    public Color m_pathColor = Color.green;

    public int m_pathStep = 60;
    [HideInInspector]
    public Vector3[] m_pathPositions = new Vector3[60];

    [Range(0.0f, 1.0f)]
    public float m_timeStart = 0.0f;
    [Range(0.0f, 1.0f)]
    public float m_timeEnd = 1.0f;

    [Range(0.0f, 1.0f)]
    public float m_animationSlider = 0.0f;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        if (!m_initialized)
            return;

        Gizmos.color = m_pathColor;
        if (m_animator == null)
        {
            for (int i = 1; i < m_pathPositions.Length; i++)
            {
                Gizmos.DrawLine(
                    transform.TransformPoint(m_pathPositions[i - 1]),
                    transform.TransformPoint(m_pathPositions[i])
                    );
            }
        }
        else
        {
            for (int i = 1; i < m_pathPositions.Length; i++)
            {
                Gizmos.DrawLine(
                    m_animator.transform.TransformPoint(m_pathPositions[i - 1]),
                    m_animator.transform.TransformPoint(m_pathPositions[i])
                    );
            }
        }
    }

    private void OnValidate()
    {
        m_pathStep = Mathf.Max(2, m_pathStep);
        m_timeStart = Mathf.Min(m_timeStart, m_timeEnd);
    }
}
