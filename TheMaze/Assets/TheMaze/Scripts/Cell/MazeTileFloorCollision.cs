using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{
    public class MazeTileFloorCollision : MonoBehaviour
    {
        [SerializeField] private MazeCell m_MazeCell;
        public MazeCell MazeCell
        {
            get { return m_MazeCell; }
            set { m_MazeCell = value; }
        }

        [SerializeField]
        private CellHole m_CellHole;
        public CellHole CellHole
        {
            get { return m_CellHole; }
            set { m_CellHole = value; }
        }
    }
}
