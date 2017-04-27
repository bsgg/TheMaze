using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{    
    public class MazeCell : MonoBehaviour
    {

        /// <summary>
        /// NONE = Normal one
        /// STARTLOCATION = Start location map
        /// ENDLOCATION = End location map
        /// HOLE = Hole
        /// ITEM = Cell has an item
        /// INVISIBLE = Invisible tile
        /// </summary>        
        public enum ETypeCell { NONE, STARTLOCATION, ENDLOCATION, HOLE, ITEM, INVISIBLE };

        [Header("Cell Settings")]
        [SerializeField]
        private ETypeCell m_TypeCell = ETypeCell.NONE;
        public ETypeCell TypeCell
        {
            get { return m_TypeCell; }
            set { m_TypeCell = value; }
        }

        [SerializeField]
        private IVector2 m_Coords;
        public IVector2 Coords
        {
            get { return m_Coords; }
            set { m_Coords = value; }
        }

        [Space(10)]
        [Header("Edges")]         
        /// <summary>
        /// Array of edges in a cell (4)
        /// </summary>
        [SerializeField]
        private MazeCellEdge[] m_Edges = new MazeCellEdge[MazeDirections.NumberDirections];
        public MazeCellEdge[] Edges
        {
            get { return m_Edges; }
        }

        [Space(10)]
        [Header("Tile Floor")]
        [SerializeField]
        private GameObject m_TileFloorObject;
        [SerializeField]
        private GameObject m_TriggerFloor;
        [SerializeField]
        private GameObject m_ExitTileEffect;

        [SerializeField]
        private GameObject m_TileFRoofObject;

        [SerializeField]
        private Material[] m_FloorMaterials;

        [Space(10)]
        [Header("Hole")]
        [SerializeField]
        private CellHole m_Hole;

        [Header("Obstacles")]
        [SerializeField]
        private GameObject[] m_Obstacles;

       /* [SerializeField]
        private bool m_IsVisible;
        public bool IsVisible
        {
            get { return m_IsVisible; }
        }*/


        /*public void SetVisible(bool visible)
        {
            m_IsVisible = visible;
            if (!m_IsVisible)
            {
                SetTypeCell(ETypeCell.NONE);
                ActiveTriggerFloor(false);
                ActiveExitEffect(false);
                m_Hole.Disable();
                m_TriggerFloor.SetActive(false);
                m_TileFloorObject.SetActive(false);
                m_TileFRoofObject.SetActive(false);

                for (int i= 0; i< m_Edges.Length; i++ )
                {
                    m_Edges[i].Hide();
                }
            }
        }*/


        public void SetTypeCell(ETypeCell typeCell)
        {
            m_TypeCell = typeCell;
            ActiveExitEffect(false);

            // Set the material acording with the type of cell

            int indexCell = (int)typeCell;

            if (indexCell >= m_FloorMaterials.Length)
            {
                Debug.LogError("WRONG INDEX CELL " + m_Coords.Column + "_" + m_Coords.Row + " INDEX: " + indexCell);
            }

            switch (m_TypeCell)
            {
                case ETypeCell.NONE:
                    m_TriggerFloor.SetActive(false);
                    SetMaterial(m_FloorMaterials[(int)m_TypeCell]);
                    //m_TubeFloorObject.SetActive(false);
                    m_Hole.Disable();
                    break;
                case ETypeCell.STARTLOCATION:
                    
                    m_TriggerFloor.SetActive(true);
                    SetMaterial(m_FloorMaterials[(int)m_TypeCell]);
                    //m_TubeFloorObject.SetActive(false);
                    m_Hole.Disable();
                    break;
                case ETypeCell.ENDLOCATION:
                    m_TriggerFloor.SetActive(true);
                    SetMaterial(m_FloorMaterials[(int)m_TypeCell]);
                    //m_TubeFloorObject.SetActive(false);
                    m_Hole.Disable();
                    break;
                case ETypeCell.HOLE:
                    m_TriggerFloor.SetActive(false);
                    m_TileFloorObject.GetComponent<BoxCollider>().enabled = false;
                    SetMaterial(m_FloorMaterials[(int)m_TypeCell]);
                    //m_TubeFloorObject.SetActive(true);
                    //
                    m_Hole.Enable();
                    break;

                case ETypeCell.INVISIBLE:
                    ActiveTriggerFloor(false);
                    ActiveExitEffect(false);
                    m_Hole.Disable();
                    m_TriggerFloor.SetActive(false);
                    m_TileFloorObject.SetActive(false);
                    m_TileFRoofObject.SetActive(false);

                    for (int i = 0; i < m_Edges.Length; i++)
                    {
                        m_Edges[i].Hide();
                    }
                break;
            }           
        }

        public void ActiveTriggerFloor(bool active)
        {
            m_TriggerFloor.SetActive(active);
        }

        public void ActiveExitEffect(bool active)
        {
            if (m_ExitTileEffect != null)
            {
                m_ExitTileEffect.SetActive(active);
            }
        }

        public void SetHole(HoleData data)
        {
            m_Hole.SetHoleData(data);            
        }

        public void SetObstacle(ObstacleData data)
        {
            for (int i= 0; i< m_Obstacles.Length; i++)
            {
                m_Obstacles[i].SetActive(false);
            }
            m_Obstacles[(int)(data.TypeObstacle)].SetActive(true);
        }


        public void SetMaterial(Material mat)
        {
            m_TileFloorObject.GetComponent<MeshRenderer>().material = mat;
        }      

       

        /// <summary>
        /// Gets the edge in a direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public MazeCellEdge GetEdge(MazeDirection direction)
        {
            return m_Edges[(int)direction];
        }

        /// <summary>
        /// Wheter or not the cell has the four edgeds initialized
        /// </summary>
        private int m_NInitializedEdges = 0;
        public bool IsCompleteBoxed
        {
            get { return m_NInitializedEdges == MazeDirections.NumberDirections; }
        }

        public void SetEdge(MazeDirection dir, MazeCellEdge.TypeEdgeEnum tEdge)
        {
            int indexEdge = (int)dir;
            m_Edges[indexEdge].Cell = this;
            m_Edges[indexEdge].TypeMazeCell = tEdge;
            m_Edges[indexEdge].Direction = dir;
            m_NInitializedEdges += 1;
        }

        public void SetEdge(MazeDirection dir, MazeCellEdge.TypeEdgeEnum tEdge, MazeCell connectedCell)
        {
            int indexEdge = (int)dir;

            m_Edges[indexEdge].Cell = this;
            m_Edges[indexEdge].TypeMazeCell = tEdge;
            m_Edges[indexEdge].Direction = dir;
            m_Edges[indexEdge].ConnectedCell = connectedCell;

            // Set material
            /*if (m_IndexSettingsRoom < m_WallsMaterials.Length)
            {
                m_Edges[indexEdge].SetMaterial(m_WallsMaterials[m_IndexSettingsRoom]);
            }*/

            m_NInitializedEdges += 1;
        }
        
    }
}
