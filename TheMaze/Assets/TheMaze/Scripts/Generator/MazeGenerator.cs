using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UI;

namespace TheMaze.MazeGenerator
{   
    public class Location
    {
        public IVector2 Coords;
    }

    public class MazeJSONData
    {
        public string Name = "";
        public int Columns = 0;
        public int Rows = 0;
        public int NumberRooms = 0;

        /// <summary>
        ///  Tile Coordinates for start point
        /// </summary>
        //public IVector2 StartLocation;

        /// <summary>
        ///  Tile Coordinates for end point
        /// </summary>
        //public IVector2 EndLocation;

        public List<TileData> ListTiles;

        public List<HoleData> ListHoles;

        public List<ObstacleData> ListObstacles;

        public MazeJSONData()
        {
            ListHoles = new List<HoleData>();
            ListTiles = new List<TileData>();
            ListObstacles = new List<ObstacleData>();
        }

        public void AddTileData(TileData tile)
        {
            ListTiles.Add(tile);
        }

    }   

    public class MazeGenerator : MonoBehaviour
    {
        #region Instance
        private static MazeGenerator m_Instance;
        public static MazeGenerator Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (MazeGenerator)FindObjectOfType(typeof(MazeGenerator));

                    if (m_Instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(MazeGenerator) + " is needed in the scene, but there is none.");
                    }
                }
                return m_Instance;
            }
        }
        #endregion Instance  

        [Header("Prefab Maze Generation")]
        [SerializeField]
        private float m_TileSize = 4.0f;
        [SerializeField] private GameObject m_CellPrefab;        
        [SerializeField] private GameObject[] m_ObstaclesPrefab;

        #region GenerateJSON

        public delegate void GenerateMazeAction(string message);
        public event GenerateMazeAction OnEndGenerateJSONMaze;
        public event GenerateMazeAction OnEndGeneratePrefabsMaze;        

        #endregion GenerateJSON


        #region GeneratePrefabs

        public void GenerateMazePrefabs()
        {  
            List<string> lFiles = MazeJSONTool.ListJSONFiles();

            if ((lFiles != null) && (lFiles.Count> 0))
            {
                StartCoroutine(RoutineGeneratePrefabs(lFiles));
            }           
        }

        private IEnumerator RoutineGeneratePrefabs(List<string> lFiles)
        {
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < lFiles.Count; i++)
            {
                GenerateMazePrefab(lFiles[i]);
                yield return new WaitForSeconds(0.2f);
            }

            if (OnEndGeneratePrefabsMaze != null)
            {
                OnEndGeneratePrefabsMaze("");
            }
        }

        #endregion GeneratePrefabs

        #region GenerateMazeData

        public void GenerateDataMazeByPath(string nameMaze, int numberColumns, int numberRows, List<IVector2> PathCoords)
        {
            // Initialize grid and list active tiles
            TileData[,] grid = new TileData[numberColumns, numberRows];

            // Set all tiles to invisible
            for (int iCol = 0; iCol < numberColumns; ++iCol)
            {
                for (int iRow = 0; iRow < numberRows; ++iRow)
                {
                    IVector2 currentCoords = new IVector2(iCol, iRow);
                    TileData cTile = new TileData(currentCoords);
                    cTile.TileType = MazeCell.ETypeCell.INVISIBLE;

                    // Check if this tile is in coords
                    for (int iTile = 0; iTile < PathCoords.Count; iTile++)
                    {
                        if (currentCoords == PathCoords[iTile])
                        {
                            cTile.TileType = MazeCell.ETypeCell.NONE;
                            // Add walls
                            for (int iEdge = 0; iEdge < MazeDirections.NumberDirections; iEdge++)
                            {
                                cTile.SetEdge((MazeDirection)iEdge, MazeCellEdge.TypeEdgeEnum.WALL);
                            }

                            break;
                        }
                    }
                    //Add tile
                    grid[iCol, iRow] = cTile;
                }
            }


            // Convertion between direction and a 2D Vector
            IVector2[] m_directions =
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

            // Follow the path and open walls
            // Check if this tile is in coords
            for (int iTile = 0; iTile < (PathCoords.Count-1); iTile++)
            {
                IVector2 currentCoords = PathCoords[iTile];

                // Next tile
                int nextTile = iTile + 1;

                // Check if next tile is the last one, then next tile would be the first one
                if (nextTile >= PathCoords.Count)
                {

                    break;
                    //nextTile = 0;
                }

                IVector2 nextCoords = PathCoords[nextTile];


                // Check coords
                // Same column (move up or down)
                MazeDirection moveDirection = MazeDirection.UP;
                bool findDir = false;
                if (currentCoords.Column == nextCoords.Column)
                {
                    // Check if up or down
                    if (currentCoords.Row < nextCoords.Row)
                    {
                        moveDirection = MazeDirection.UP;
                        findDir = true;
                    }
                    else
                    {
                        moveDirection = MazeDirection.DOWN;
                        findDir = true;
                    }

                }else if (currentCoords.Row == nextCoords.Row) // Same row, move left or right
                {
                    if (currentCoords.Column < nextCoords.Column)
                    {
                        moveDirection = MazeDirection.RIGHT;
                        findDir = true;
                    }
                    else
                    {
                        moveDirection = MazeDirection.LEFT;
                        findDir = true;
                    }

                }                

                if (findDir)
                {
                    // Set new edge
                    TileData cTile = grid[currentCoords.Column, currentCoords.Row];
                    cTile.SetEdge(moveDirection, MazeCellEdge.TypeEdgeEnum.PASSAGE);
                    grid[currentCoords.Column, currentCoords.Row] = cTile;


                    // Set next tile
                    TileData nTile = grid[nextCoords.Column, nextCoords.Row];
                    nTile.SetEdge(moveDirection.GetOpposite(), MazeCellEdge.TypeEdgeEnum.PASSAGE);
                    grid[nextCoords.Column, nextCoords.Row] = nTile;
                }
            }

            for (int iCol = 0; iCol < numberColumns; ++iCol)
            {
                for (int iRow = 0; iRow < numberRows; ++iRow)
                {
                    Debug.LogFormat("Tile {0}x{1}", iCol, iRow);
                    // Add walls
                    for (int iEdge = 0; iEdge < MazeDirections.NumberDirections; iEdge++)
                    {
                        Debug.LogFormat("Edge {0} : {1}", (MazeDirection)iEdge, grid[iCol,iRow].Edges[iEdge].ToString());
                    }
                }
            }

            // Save in JSON
            Debug.Log("[MazeGenerator] Saving in JSON file ");            

            MazeJSONData mazeData = new MazeJSONData();
            mazeData.Columns = numberColumns;
            mazeData.Rows = numberRows;
            mazeData.NumberRooms = 0;
            mazeData.Name = nameMaze;
            //mazeData.StartLocation = new IVector2();
            //mazeData.EndLocation = new IVector2();

            for (int iCol = 0; iCol < numberColumns; ++iCol)
            {
                for (int iRow = 0; iRow < numberRows; ++iRow)
                {
                    mazeData.AddTileData(grid[iCol, iRow]);
                    /*Debug.Log("Tile(" + iCol + "x" + iRow + ") - Coord ( " + grid[iCol, iRow].Coords.Column + "," + grid[iCol, iRow].Coords.Row + ")");
                    for (int iEdge = 0; iEdge < grid[iCol, iRow].Edges.Length; iEdge++)
                    {
                        Debug.Log("Edge " + ((MazeDirection)iEdge).ToString() + ":" + grid[iCol, iRow].Edges[iEdge].ToString());
                    }*/
                }
            }

            Debug.Log("Saving " + mazeData.Name + " to JSON...");

            MazeJSONTool.SaveMazeOnFile(mazeData);

            if (OnEndGenerateJSONMaze != null)
            {
                OnEndGenerateJSONMaze("");
            }
            
        }

        public void GenerateRandomDataMaze(string nameMaze, int numberColumns, int numberRows)
        {
            // Initialize grid and list active tiles
            TileData[,]  grid = new TileData[numberColumns, numberRows];

            // Generate a temporary list of tiles
            List<TileData> lActiveTiles = new List<TileData>();
            
            int numberRoom = 0;

            // Gets first tile in random coord
            IVector2 firstCoord = new IVector2(Random.Range(0, numberColumns), Random.Range(0, numberRows));
            TileData firstTile = new TileData(firstCoord);

            //firstTile.Room = numberRoom;

            grid[firstCoord.Column, firstCoord.Row] = firstTile;
            lActiveTiles.Add(firstTile);

            // Generate the rest of the tiles until the list is empty
            while (lActiveTiles.Count > 0)
            {
                // Gets random index to select next tile
                int cIndex = Random.Range(0, lActiveTiles.Count - 1);
                TileData cTile = lActiveTiles[cIndex];

                // Check if the current tile doesn't have all of the edges already defined (it already has boxed by 4 edges)
                if (!cTile.IsBoxed())
                {
                    // Get a random direction from the current tile
                    MazeDirection randomDir = cTile.GetRandomUnboxedDirection();
                    // Gets the coords for the random direction + current tile (this way we can get the new tile)
                    IVector2 newCoords = cTile.Coords + randomDir.ToIntVector2();              

                    // Check if these coords are within the maze boundaries
                    bool coordsInBoundaries = ((newCoords.Column >= 0) && (newCoords.Column < numberColumns) &&
                                              (newCoords.Row >= 0) && (newCoords.Row < numberRows));
                    if (coordsInBoundaries)
                    {
                        // Gets the tile for this coords
                        TileData nextTile = grid[newCoords.Column, newCoords.Row];
                        // This tile doesn't exist yet, create the cell and a passage between the current tile and this new one
                        if (nextTile == null)
                        {
                            nextTile = new TileData(newCoords);

                            // Add tile to the grid
                            grid[newCoords.Column, newCoords.Row] = nextTile;


                            // Add passage
                            // Sets a passage for the random edge for the current tile
                            cTile.SetEdge(randomDir, MazeCellEdge.TypeEdgeEnum.PASSAGE);

                            // Sets the opposite for this random direction or edge for the next tile as a Door
                            // For instance> if random dir for current tile was right, next tile has left as edge
                            nextTile.SetEdge(randomDir.GetOpposite(), MazeCellEdge.TypeEdgeEnum.PASSAGE);
                            // Both tiles are in the same room
                            //nextTile.Room = numberRoom;

                            // Add this nextTile to active tiles
                            lActiveTiles.Add(nextTile);
                        }
                        else 
                        {
                            // Tile exits create a wall
                            // Sets a wall for the random edge for the current tile
                            cTile.SetEdge(randomDir, MazeCellEdge.TypeEdgeEnum.WALL);
                        }
                    }
                    else  
                    {
                        // coords not in boundary, create a wall for the current cell
                        // Sets a wall for the random edge for the current tile
                        cTile.SetEdge(randomDir, MazeCellEdge.TypeEdgeEnum.WALL);
                    }
                }else 
                {
                    // Remove the tile from the active tiles
                    lActiveTiles.RemoveAt(cIndex);
                }
            }

            // Save in JSON
            Debug.Log("[MazeGenerator] Saving in JSON file ");            

            MazeJSONData mazeData = new MazeJSONData();
            mazeData.Columns = numberColumns;
            mazeData.Rows = numberRows;
            mazeData.NumberRooms = numberRoom + 1;
            mazeData.Name = nameMaze;
            //mazeData.StartLocation = new IVector2();
            //mazeData.EndLocation = new IVector2();

            for (int iCol = 0; iCol < numberColumns; ++iCol)
            {
                for (int iRow = 0; iRow < numberRows; ++iRow)
                {
                    mazeData.AddTileData(grid[iCol, iRow]);
                    /*Debug.Log("Tile(" + iCol + "x" + iRow + ") - Coord ( " + grid[iCol, iRow].Coords.Column + "," + grid[iCol, iRow].Coords.Row + ")");
                    for (int iEdge = 0; iEdge < grid[iCol, iRow].Edges.Length; iEdge++)
                    {
                        Debug.Log("Edge " + ((MazeDirection)iEdge).ToString() + ":" + grid[iCol, iRow].Edges[iEdge].ToString());
                    }*/
                }
            }

            Debug.Log("Saving " + mazeData.Name + " to JSON...");

            MazeJSONTool.SaveMazeOnFile(mazeData);

            if (OnEndGenerateJSONMaze != null)
            {
                OnEndGenerateJSONMaze("");
            }
        }        

        #endregion GenerateMazeData

        #region GenerateMazePrefab       

        public void GenerateMazePrefab(string nameMaze)
        {
            MazeJSONData mazeJSONData;
            bool tryLoadMaze = MazeJSONTool.LoadMazeJSON(nameMaze, out mazeJSONData);
            if (tryLoadMaze)
            {
                GameObject obj = new GameObject(nameMaze);
                obj.layer = LayerMask.NameToLayer("Game");
                MazeBase maze = obj.AddComponent<MazeBase>();

                // Maze settings
                MazeSettings mazeSettings = new MazeSettings(mazeJSONData.Columns, mazeJSONData.Rows);
                mazeSettings.NameMaze = nameMaze;
                //mazeSettings.StartPoint = mazeJSONData.StartLocation;
                //mazeSettings.EndPoint = mazeJSONData.EndLocation;

                maze.InitializeMaze(mazeSettings);

                // Create rooms
                Debug.Log("number rooms: " + mazeJSONData.NumberRooms);
                maze.CreateRooms(mazeJSONData.NumberRooms);

                // Create cells
                Debug.Log("number tiles: " + mazeJSONData.ListTiles.Count);
                for (int i = 0; i < mazeJSONData.ListTiles.Count; i++)
                {
                    IVector2 coordsCell = new IVector2(mazeJSONData.ListTiles[i].Coords.Column, mazeJSONData.ListTiles[i].Coords.Row);

                    // Instance cell
                    GameObject goCell = Instantiate(m_CellPrefab);
                    goCell.layer = LayerMask.NameToLayer("Game");
                    goCell.name = "Cell_" + coordsCell.Column + "_" + coordsCell.Row;
                    goCell.transform.parent = obj.transform;
                    // Set position tiles    
                    goCell.transform.localPosition = new Vector3(coordsCell.Column * m_TileSize, 0.0f, coordsCell.Row * m_TileSize);
                    // Setup index room
                    MazeCell mCell = goCell.GetComponent<MazeCell>();
                    mCell.Coords = coordsCell;

                    // Set type cell                    
                    mCell.SetTypeCell(mazeJSONData.ListTiles[i].TileType);
                    if (mazeJSONData.ListTiles[i].TileType == MazeCell.ETypeCell.STARTLOCATION)
                    {
                        mazeSettings.StartPoint = coordsCell;
                    }
                    if (mazeJSONData.ListTiles[i].TileType == MazeCell.ETypeCell.ENDLOCATION)
                    {
                        mazeSettings.EndPoint = coordsCell;
                    }




                    /*bool bVisibleTile = true;
                    if (mazeJSONData.ListTiles[i].IsVisible == 0)
                    {
                        bVisibleTile = false;
                    }
                    if (mCell.IsVisible)
                    {

                        // Set material for start point
                        if ((coordsCell.Column == mazeSettings.StartPoint.Column) && (coordsCell.Row == mazeSettings.StartPoint.Row))
                        {
                            mCell.SetTypeCell(MazeCell.ETypeCell.STARTLOCATION);
                        }

                        // Set material for start point
                        else if ((coordsCell.Column == mazeSettings.EndPoint.Column) && (coordsCell.Row == mazeSettings.EndPoint.Row))
                        {
                            mCell.SetTypeCell(MazeCell.ETypeCell.ENDLOCATION);

                        }
                        else
                        {
                            // Check if the coords are in the list of holes
                            bool isHole = false;
                            for (int iHole = 0; iHole < mazeJSONData.ListHoles.Count; iHole++)
                            {
                                if ((coordsCell.Column == mazeJSONData.ListHoles[iHole].Coords.Column) && (coordsCell.Row == mazeJSONData.ListHoles[iHole].Coords.Row))
                                {
                                    mCell.SetTypeCell(MazeCell.ETypeCell.HOLE);
                                    isHole = true;

                                    Debug.LogFormat("HOLE DATA: Coords {0}x{1} TypeHole: {2} ConnectedCoords: {3}x{4} ", mazeJSONData.ListHoles[iHole].Coords.Column, mazeJSONData.ListHoles[iHole].Coords.Row, mazeJSONData.ListHoles[iHole].TypeHole, mazeJSONData.ListHoles[iHole].ConnectedCoords.Column, mazeJSONData.ListHoles[iHole].ConnectedCoords.Row);

                                    mazeSettings.AddHole(mazeJSONData.ListHoles[iHole]);
                                    mCell.SetHole(mazeJSONData.ListHoles[iHole]);
                                    break;
                                }
                            }

                            if (!isHole)
                            {
                                mCell.SetTypeCell(MazeCell.ETypeCell.NONE);
                            }
                        }

                        // Add obstacles
                        for (int iObstacle = 0; iObstacle < mazeJSONData.ListObstacles.Count; iObstacle++)
                        {
                            if ((mCell.Coords.Column == mazeJSONData.ListObstacles[iObstacle].Coords.Column) && (mCell.Coords.Row == mazeJSONData.ListObstacles[iObstacle].Coords.Row))
                            {
                                // Instance obstacle
                                mCell.SetObstacle(mazeJSONData.ListObstacles[iObstacle]);
                                break;
                            }
                        }

                    }*/
                    // Add cell to the maze
                    maze.AddCell(mCell);
                    
                }
                // Segup each edge for each cells
                for (int i = 0; i < mazeJSONData.ListTiles.Count; i++)
                {
                    MazeCell currentCell = maze.GetCell(mazeJSONData.ListTiles[i].Coords);

                    if (currentCell.TypeCell != MazeCell.ETypeCell.INVISIBLE)
                    {

                        // Setup each cell
                        for (int iEdge = 0; iEdge < mazeJSONData.ListTiles[i].Edges.Length; iEdge++)
                        {
                            // Gets connected cell with this edge
                            // Gets direction for this edge
                            MazeDirection dir = (MazeDirection)iEdge;

                            IVector2 coordsConnectedCell = mazeJSONData.ListTiles[i].Coords + dir.ToIntVector2();

                            // Get connected cell (it could be null)
                            MazeCell connectedCell = maze.GetCell(coordsConnectedCell);

                            // Get type edge
                            MazeCellEdge.TypeEdgeEnum tEdge = mazeJSONData.ListTiles[i].Edges[iEdge];

                            currentCell.SetEdge(dir, tEdge, connectedCell);
                        }
                    }
                }

#if UNITY_EDITOR
                // Create prefab
                string fileLocation = "Assets/TheMaze/Prefabs/Mazes/" + nameMaze + ".prefab";
                Object emptyObj = PrefabUtility.CreateEmptyPrefab(fileLocation);

                PrefabUtility.ReplacePrefab(obj, emptyObj, ReplacePrefabOptions.ConnectToPrefab);
#endif
            }
            
            
        }

        #endregion GenerateMazePrefab

    }
}
