using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{
    public class MazeEdgeCollision : MonoBehaviour
    {
        [SerializeField]
        private  MazeCellEdge m_MazeEdge;

        public MazeCellEdge MazeEdge
        {
            get { return m_MazeEdge; }
            set { m_MazeEdge = value; }
        }

    }
}
