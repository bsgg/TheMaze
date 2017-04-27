using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{
    [System.Serializable]
    public class MazeRoom
    {
        // <summary>
        /// Index room
        /// </summary>
        public int IndexRoom;

        // Index to set up which is the room setting for this room
        //public int SettingsIndex;


        // List of cells for this room
        public List<MazeCell> Cells = new List<MazeCell>();
        /*public List<MazeCell> Cells
        {
            get { return m_Cells; }
        }*/

        public void AddCell(MazeCell cell)
        {
            // Asign the room to the current cell
            //cell.Room = this;
            Cells.Add(cell);
        }
        
    }
}
