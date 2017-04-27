using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{
    [System.Serializable]
    public struct IVector2
    {
        /// <summary>
        /// Columns == X
        /// </summary>
        [Range(1, 100)]
        public int Column;

        /// <summary>
        /// Rows == Z
        /// </summary>
        [Range(1, 100)]
        public int Row;

        public IVector2(int col, int row)
        {
            Column = col;
            Row = row;
        }

        /// <summary>
        /// Operator + for supporting the add of this vector and another one.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IVector2 operator +(IVector2 a, IVector2 b)
        {
            a.Column += b.Column;
            a.Row += b.Row;
            return a;
        }

        public static IVector2 operator -(IVector2 a, IVector2 b)
        {
            a.Column -= b.Column;
            a.Row -= b.Row;
            return a;
        }

        public static bool operator ==(IVector2 c1, IVector2 c2)
        {
            return ((c1.Column == c2.Column) && (c1.Row == c2.Row));
        }

        public static bool operator !=(IVector2 c1, IVector2 c2)
        {
            return ((c1.Column != c2.Column) || (c1.Row != c2.Row));
        }       
    }

    [System.Serializable]
    public class MazeSettings
    {
        public string NameMaze;
        public IVector2 MazeSize;
        public IVector2 StartPoint;
        public IVector2 EndPoint;
        public List<HoleData> ListHoles;

        public MazeSettings(int sizeX, int sizeZ)
        {
            ListHoles = new List<HoleData>();
            MazeSize = new IVector2(sizeX, sizeZ);
        }

        public void AddHole(HoleData data)
        {
            ListHoles.Add(data);
        }        
    }

    public class MazeBase : MonoBehaviour
    {
        /// <summary>
        /// Maze Settings
        /// </summary>
        [SerializeField]
        protected MazeSettings           m_MazeSettings;
        public MazeSettings MazeSettings
        {
            get { return m_MazeSettings; }
        }


        protected List<MazeRoom>                          m_ListRooms;

        /// <summary>
        /// Number cells
        /// </summary>        
        protected MazeCell[,]                                 m_Cells;
        public MazeCell[,] Cells
        {
            get { return m_Cells; }
        }

        [SerializeField]
        private List<MazeCell> m_ListCells;


        [Header("Items")]
        [SerializeField]
        private int m_NumberItems = 5;
        public int NumberItems
        {
            get { return m_NumberItems; }
        }

        [SerializeField]
        private GameObject m_ItemPrefab;

        public virtual void InitializeMaze(MazeSettings mazeSettings)
        {
            m_MazeSettings = mazeSettings;

            m_ListRooms = new List<MazeRoom>();
            m_Cells = new MazeCell[m_MazeSettings.MazeSize.Column, m_MazeSettings.MazeSize.Row];
            m_ListCells = new List<MazeCell>();
        }

        public void CreateRooms(int numberRooms)
        {
            m_ListRooms = new List<MazeRoom>();
            for (int i= 0; i< numberRooms; i++)
            {
                MazeRoom newRoom = new MazeRoom();
                newRoom.IndexRoom = i;
                m_ListRooms.Add(newRoom);
            }
        }
        
        public void AddCell(MazeCell cell)
        {
            m_Cells[cell.Coords.Column, cell.Coords.Row] = cell;
            m_ListCells.Add(cell);
        }

        public MazeCell GetCell(IVector2 coords)
        {
            if (
               (coords.Row >= 0) &&
               (coords.Row < m_MazeSettings.MazeSize.Row) &&
               (coords.Column >= 0) &&
               (coords.Column < m_MazeSettings.MazeSize.Column))
            {
                if (m_Cells != null)
                {
                    return m_Cells[coords.Column, coords.Row];
                }
            }
            return null;
        }

        public MazeCell GetCellFromList(IVector2 coords)
        {
            if (
               (coords.Row >= 0) &&
               (coords.Row < m_MazeSettings.MazeSize.Row) &&
               (coords.Column >= 0) &&
               (coords.Column < m_MazeSettings.MazeSize.Column))
            {
                if (m_ListCells != null)
                {
                    int index = coords.Row + (coords.Column * m_MazeSettings.MazeSize.Column);
                    return m_ListCells[index];
                }
            }
            return null;
        }
       

        public void DoStart()
        {
            if (m_ListCells != null)
            {
                int iCol = 0;
                int iRow = 0;

                // Relink
                m_Cells = new MazeCell[m_MazeSettings.MazeSize.Column, m_MazeSettings.MazeSize.Row];

                for (int i = 0; i < m_ListCells.Count; i++)
                {
                    iRow = i % m_MazeSettings.MazeSize.Row;
                    iCol = i / m_MazeSettings.MazeSize.Row;
                    m_Cells[iCol, iRow] = m_ListCells[i];
                }
                
                for (int i = 0; i < m_NumberItems; i++)
                {
                    RespawnItem();
                }
            }
        }

        public void EnableExit()
        {
            MazeCell exitCell = GetCellFromList(m_MazeSettings.EndPoint);
            if (exitCell != null)
            {
                exitCell.ActiveTriggerFloor(true);
                exitCell.ActiveExitEffect(true);
            }
        }

        public void DisableExit()
        {
            MazeCell exitCell = GetCellFromList(m_MazeSettings.EndPoint);
            if (exitCell != null)
            {
                exitCell.ActiveTriggerFloor(false);
                exitCell.ActiveExitEffect(false);
            }
        }

        #region Items

        private void RespawnItem()
        {
            MazeCell cellItem = GetFreeCell();
            if (m_ItemPrefab != null)
            {
                GameObject go = Instantiate(m_ItemPrefab);
                TheMazeGame.Item itemObject = go.GetComponent<TheMazeGame.Item>();

                if (itemObject != null)
                {
                    go.transform.parent = cellItem.transform;
                    go.transform.localPosition = new Vector3(0.0f, 20.0f, 0.0f);
                    cellItem.TypeCell = MazeCell.ETypeCell.ITEM;

                    itemObject.OnSpawnItem += ItemObject_OnSpawnItem;
                    itemObject.DoStart(cellItem);
                }
            }
        }

        private void ItemObject_OnSpawnItem()
        {
            RespawnItem();
        }        

        public MazeCell GetFreeCell()
        {                     
            if (m_ListCells != null)
            {
                int nTriesToFindLoc = 100;
                do
                {
                    // Random location
                    int iCol = Random.Range(0, m_MazeSettings.MazeSize.Column);
                    int iRow = Random.Range(0, m_MazeSettings.MazeSize.Row);

                    if (m_Cells[iCol, iRow].TypeCell == MazeCell.ETypeCell.NONE)
                    {
                        return (m_Cells[iCol, iRow]);
                    }
                    else
                    {
                        // Avoid to look for ever
                        nTriesToFindLoc--;                       
                    }

                } while (nTriesToFindLoc > 0);                
            }

            return null;
        }

        #endregion Items
    }
}
