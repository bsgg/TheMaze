using UnityEngine;
using System.Collections;

namespace TheMaze
{
    public enum MazeDirection
    {
        UP = 0,
        RIGHT,
        DOWN,
        LEFT
    }
    public static class MazeDirections
    {
        public static int NumberDirections
        {
            get
            {
                return (System.Enum.GetNames(typeof(MazeDirection)).Length);
            }
        }
        
        // Convertion between direction and a 2D Vector
        private static IVector2[] m_DirectionToVector =
        {
            // UP
            new IVector2(0,1),
            // RIGHT
            new IVector2(1,0),
            // DOWN
            new IVector2(0,-1),
            // LEFT
            new IVector2(-1,0)
        };

        public static IVector2 ToIntVector2(this MazeDirection direction)
        {
            return m_DirectionToVector[(int)direction];
        }

        


        public static string ToString(this MazeDirection direction)
        {
            return direction.ToString();
        }

        // Oposite direction
        private static MazeDirection[] m_OpositeDirection =
        {
            // Up
            MazeDirection.DOWN,
            // Right
            MazeDirection.LEFT,
            // Down
            MazeDirection.UP,
            // Left
            MazeDirection.RIGHT
        };


        /// <summary>
        /// Gets a random direction
        /// </summary>
        /// <returns></returns>
        public static MazeDirection GetRandomDirection()
        {
            return (MazeDirection)Random.Range(0, NumberDirections);            
        }

        /// <summary>
        /// Gets the oposite direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static MazeDirection GetOpposite(this MazeDirection direction)
        {
            return m_OpositeDirection[(int)direction]; 
        }

        /// <summary>
        /// Array of rotations according to the direction
        /// </summary>
        private static Quaternion[] m_Rotation =
        {
            // Up
            Quaternion.identity,
            // Right
            Quaternion.Euler(0.0f,90.0f,0.0f),
            // Down
            Quaternion.Euler(0.0f,180.0f,0.0f),
            // Left
            Quaternion.Euler(0.0f,270.0f,0.0f)
        };

        /// <summary>
        /// Gets the rotation (Quaternion) acording to the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Quaternion ToRotation(this MazeDirection direction)
        {
            int indexDir = (int)direction;
            if (indexDir < m_Rotation.Length)
            {
                return m_Rotation[indexDir];
            }
            return Quaternion.identity;
        }


        public static MazeDirection GetNextClockwise(this MazeDirection direction)
        {
            return (MazeDirection)(((int)direction + 1) % NumberDirections);
        }

        public static MazeDirection GetNextCounterclockwise(this MazeDirection direction)
        {
            return (MazeDirection)(((int)direction + NumberDirections - 1) % NumberDirections);
        }


    }
}
