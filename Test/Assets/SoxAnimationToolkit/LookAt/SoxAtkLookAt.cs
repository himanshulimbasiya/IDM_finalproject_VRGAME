using UnityEngine;
using System.Collections.Generic;

// 3ds Max의 LookAt constraint 와 같은 기능을 하도록 만들려고 했으나 m_lookAtAxis, m_sourceAxis 기능은 미 구현

[ExecuteInEditMode]
public class SoxAtkLookAt : MonoBehaviour
{
    [HideInInspector]
    public float m_version = 1.071f;

    public bool m_EditorLookAt = false;  // 에디터에서도 작동할지를 선택적으로 할 수 있도록 한다.
    public enum lookType
    {
		Camera,
		Nodes
    }
    public enum upType
    {
		Camera,
		Node,
		World
    }
    public enum axisType
    {
		X,
		Y,
		Z
    }
    public enum upCtrType
    {
		LootAt,
		AxisAlignment
    }
	
	public lookType m_lookAtType = lookType.Camera;
	public List<Transform> m_lookAtNodeList = new List<Transform>();
	//public axisType m_lookAtAxis = axisType.Z;
	public bool m_lookAtFilp = false;
	
	public upType m_upAxisType = upType.World;
	public Transform m_upNode;
	public upCtrType m_upControl = upCtrType.AxisAlignment;
	
	//public axisType m_sourceAxis = axisType.Y;
	public bool m_sourceAxisFilp = false;
	public axisType m_alignedToUpnodeAxis = axisType.Y;

    private Vector3 m_lookPos;
	
	void Update()
	{
        if (!m_EditorLookAt && !Application.isPlaying)
            return;

        //SolveOverlapAxis();
        transform.rotation = Quaternion.LookRotation(GetForwardVec(), GetUpwardVec());
	}

    private Vector3 GetForwardVec()
    {
        SetLookPos();
        return (m_lookPos - transform.position);
    }

    private Vector3 GetUpwardVec()
    {
        Vector3 posFrom = new Vector3(0, 0, 0);
        Vector3 posTo = new Vector3(0, 1, 0);

        Vector3 alignedToAxis = new Vector3(0, 1, 0);
        switch (m_alignedToUpnodeAxis)
        {
            case axisType.X:
                alignedToAxis = new Vector3(1, 0, 0);
                break;
            case axisType.Y:
                alignedToAxis = new Vector3(0, 1, 0);
                break;
            case axisType.Z:
                alignedToAxis = new Vector3(0, 0, 1);
                break;
        }
        if (m_sourceAxisFilp)
        {
            alignedToAxis *= -1;
        }

        switch (m_upControl)
        {
            case upCtrType.AxisAlignment:
                switch (m_upAxisType)
                {
                    case upType.World:
                        posFrom = new Vector3(0, 0, 0);
                        posTo = alignedToAxis;
                        break;
                    case upType.Node:
                        if (m_upNode != null)
                        {
                            posFrom = m_upNode.transform.position;
                            posTo = m_upNode.TransformPoint(alignedToAxis);
                        }
                        break;
                    case upType.Camera:
                        if (Camera.main != null)
                        {
                            posFrom = Camera.main.transform.position;
                            posTo = Camera.main.transform.TransformPoint(alignedToAxis);
                        }
                        break;
                }
                break;
            case upCtrType.LootAt:
                posFrom = transform.position;
                switch (m_upAxisType)
                {
                    case upType.World:
                        posTo = new Vector3(0, 0, 0);
                        break;
                    case upType.Node:
                        if (m_upNode != null)
                        {
                            posTo = m_upNode.transform.position;
                        }
                        break;
                    case upType.Camera:
                        if (Camera.main != null)
                        {
                            posTo = Camera.main.transform.position;
                        }
                        break;
                }
                break;
        }
        
        return (posTo - posFrom);
    }

    // m_lookPos 변수 세팅하는 함수
    private void SetLookPos()
    {
        switch (m_lookAtType) {
            case lookType.Camera:
                if (Camera.main)
                {
                    m_lookPos = Camera.main.transform.position;
                }
                else
                {
                    m_lookPos = transform.position + transform.forward;
                }
                break;
            case lookType.Nodes:
                //노드들에 오브젝트가 등록되지 않을 경우도 있으므로 일단 안전 값을 먼저 넣어준다.
                m_lookPos = transform.position + transform.forward;
                int tempCount = 0;
                Vector3 tempPos = new Vector3(0, 0, 0);
                foreach (Transform node in m_lookAtNodeList)
                {
                    if (node != null)
                    {
                        tempCount++;
                        tempPos += node.position;
                    }
                }
                m_lookPos = tempPos / (float)tempCount;
                break;
        }

        if (m_lookAtFilp)
        {
            m_lookPos = transform.position + (transform.position - m_lookPos);
        }
    }

    /*
    // Look At Axis 와 Source Axis 가 겹치는 상황을 해결한다. (Look At Axis 우선)
    private void SolveOverlapAxis()
    {
        if (m_lookAtAxis == m_sourceAxis)
        {
            switch (m_lookAtAxis)
            {
                case axisType.X:
                    m_sourceAxis = axisType.Y;
                    break;
                case axisType.Y:
                    m_sourceAxis = axisType.X;
                    break;
                case axisType.Z:
                    m_sourceAxis = axisType.Y;
                    break;
            }
        }
    }
    */
}