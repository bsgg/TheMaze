using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMazeGame
{
    public class CameraPlayer : MonoBehaviour
    {

       [SerializeField] private Player m_Player;

        private Vector3 m_Offset;

        void Start()
        {
            m_Offset = transform.position - m_Player.transform.position;
        }

        void LateUpdate()
        {
            transform.position = (m_Player.transform.position + m_Offset);     
        }

    }
}
