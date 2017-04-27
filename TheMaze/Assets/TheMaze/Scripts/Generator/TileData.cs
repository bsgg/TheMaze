using UnityEngine;
using System.Collections;

namespace TheMaze.MazeGenerator
{
    [System.Serializable]
    public class TileData 
    {
        /// <summary>
        /// Type Tile
        /// </summary>
        public MazeCell.ETypeCell TileType;
        /// <summary>
        ///  Tile Coordinates
        /// </summary>
        public IVector2 Coords;       

        
        /// <summary>
        /// Edges, each edge is a TEdge, and defines the type of connection with the rest of the tiles
        /// </summary>
        public MazeCellEdge.TypeEdgeEnum[] Edges = new MazeCellEdge.TypeEdgeEnum[MazeDirections.NumberDirections];

        /// <summary>
        /// Edges
        /// </summary>
        private int m_NumberDefinedEdges = 0;    
        
        public TileData()
        {
            Coords = new IVector2(-1, -1);
           // Room = -1;
            m_NumberDefinedEdges = 0;
            for (int i = 0; i < MazeDirections.NumberDirections; ++i)
            {
                Edges[i] = MazeCellEdge.TypeEdgeEnum.NONE;
            }

            TileType = MazeCell.ETypeCell.NONE;

        }    

        public TileData(IVector2 coords)
        {                       
            Coords = coords;
            //Room = -1;
            m_NumberDefinedEdges = 0;
            for (int i = 0; i < MazeDirections.NumberDirections; ++i)
            {
                Edges[i] = MazeCellEdge.TypeEdgeEnum.NONE;
            }
        }

        public void SetEdge(MazeDirection edge, MazeCellEdge.TypeEdgeEnum tEdge)
        {
            Edges[(int)edge] = tEdge;
            m_NumberDefinedEdges += 1;
        }

        /// <summary>
        /// Returns whether the tile has all of the edged defined
        /// </summary>
        public bool IsBoxed()
        {
            return (m_NumberDefinedEdges == MazeDirections.NumberDirections);
        }

        /// <summary>
        /// Gets a random direction within m_Edges that isn't defined yet or unboxed
        /// </summary>
        /// <returns></returns>
        public MazeDirection GetRandomUnboxedDirection()
        {
            // Gets how many uninitialized directions we should skip
            // For examples, this cells has 1 edge covered, and 3 holes.
            // skips = Random.Range(0, 4 - 1) = 2 (for example)
            // We're going to skip 2 edges inside m_Edges. 
            // Free edges = UP, RIGHT, DOWN
            // NoFree edge = LEFT
            // As we are skipping 2 edges, we'll have Down as a next random direction        
            int skips = Random.Range(0, MazeDirections.NumberDirections - m_NumberDefinedEdges);

            // Look for a hole in m_Edges array (neither a wall nor a passage) 
            for (int i= 0; i<MazeDirections.NumberDirections; ++i)
            {
                // Check if this one is defined
                if (Edges[i] == MazeCellEdge.TypeEdgeEnum.NONE)
                {
                    // If we're run out of skips, thats the random next direction
                    if (skips == 0)
                    {
                        return (MazeDirection)i;
                    }
                    skips--;
                }
            } 
            
            // If Maze Cell doesn't have any uninitialized direction cells we shouldn't call this method
            throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
        }
    }
}
