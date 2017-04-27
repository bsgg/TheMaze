using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{
    public class MazeCellEdge : MonoBehaviour
    {
        public enum TypeEdgeEnum
        {
            NONE = -1,
            WALL,
            PASSAGE
        }

        [SerializeField]
        private GameObject m_TileObject;


        /// <summary>
        /// Cell in this edge
        /// </summary>
        [SerializeField]
        protected MazeCell m_Cell;
        public MazeCell Cell
        {
            get { return m_Cell; }
            set { m_Cell = value;  }
        }

        /// <summary>
        /// Cell connected to the edge
        /// </summary>
        [SerializeField]
        protected MazeCell m_ConnectedCell;
        public MazeCell ConnectedCell
        {
            get { return m_ConnectedCell; }
            set { m_ConnectedCell = value; }
        }

        /// <summary>
        /// Orientation of the cell according to its own cell and connected cell
        /// </summary>
        [SerializeField]
        protected MazeDirection m_Direction;
        public MazeDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }


        [SerializeField]
        private TypeEdgeEnum m_TypeEdge = TypeEdgeEnum.PASSAGE;
        public TypeEdgeEnum TypeMazeCell
        {
            get { return m_TypeEdge; }
            set
            {
                if (value == TypeEdgeEnum.PASSAGE)
                {
                    gameObject.SetActive(false);
                }                
                m_TypeEdge = value;
            }
        }

        public void SetMaterial(Material mat)
        {
            m_TileObject.GetComponent<MeshRenderer>().material = mat;
        }


        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
