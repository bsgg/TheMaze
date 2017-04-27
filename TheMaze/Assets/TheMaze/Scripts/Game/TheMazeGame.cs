using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheMaze;

namespace TheMazeGame
{
    public class TheMazeGame : MonoBehaviour
    {
        #region Instance
        private static TheMazeGame m_Instance;
        public static TheMazeGame Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (TheMazeGame)FindObjectOfType(typeof(TheMazeGame));

                    if (m_Instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(TheMazeGame) + " is needed in the scene, but there is none.");
                    }
                }
                return m_Instance;
            }
        }
        #endregion Instance

        [SerializeField]
        private Player m_Player;
        private int m_ItemsCollected;

        [SerializeField]
        private bool m_EnableInput = false;


        [SerializeField]
        private GameObject[] m_MazePrefabs;
        private MazeBase m_Maze;
        private int m_CurrentLevel = 0;

        // Maze movement control
        public enum Direction { NONE, UP, RIGHT, DOWN, LEFT };
        private Direction m_CurrentDirection = Direction.NONE;
        private float m_AmountRotation = 20.0f;
        private Quaternion m_MazeCurrentRotation;
        private Quaternion m_MazeNextRotation;
        private float m_MazeCurrentLerpTime = 0.0f;
        [SerializeField]
        private float m_MazeRotationActionTime = 0.1f;        

       // [Header("UI")]
        //[SerializeField] private MazeGameUI m_MazeGameUI;

        public void Initialize()
        {

            m_ItemsCollected = 0;
            m_EnableInput = false;

            // Initialize maze and player
            m_CurrentLevel = 0;
            InitMaze();
            InitPlayer();            

            // Player events
            m_Player.OnPlayerDead += M_Player_OnPlayerDead;
            m_Player.OnPlayerRespawn += M_Player_OnPlayerRespawn;
            m_Player.OnPlayerSolveMaze += M_Player_OnPlayerSolveMaze;
            m_Player.OnPlayerConsumeItem += M_Player_OnPlayerConsumeItem;
            m_Player.OnPlayerHitObstacle += M_Player_OnPlayerHitObstacle;

            // Best score
            /*m_MazeGameUI.SetBestScore(ScoreManager.Instance.BestScore);
            m_MazeGameUI.SetScore(m_Player.Points);
            m_MazeGameUI.SetCurrentLevel(m_CurrentLevel);*/
        }

        
        private void InitMaze()
        {
            m_CurrentLevel = Mathf.Clamp(m_CurrentLevel, 0, (m_MazePrefabs.Length - 1));

            if (m_Maze != null)
            {
                if (m_Maze.gameObject != null)
                {
                    Destroy(m_Maze.gameObject);
                }
            }

            // Instance maze depending on the current level
            GameObject mazeObj = Instantiate(m_MazePrefabs[m_CurrentLevel], transform);          

            m_Maze = mazeObj.GetComponent<MazeBase>();

            if (m_Maze != null)
            {
                // Initialize maze
                m_MazeCurrentRotation = m_Maze.transform.rotation;
                m_MazeNextRotation = m_Maze.transform.rotation;
                m_MazeCurrentLerpTime = 0.0f;
                m_Maze.DoStart();
                m_Maze.DisableExit();

                m_ItemsCollected = 0;
                //m_MazeGameUI.SetItems(m_ItemsCollected, m_Maze.NumberItems);
            }
        }

        private void InitPlayer()
        {
            // Set player in first cell
            if (m_Maze != null)
            {
                IVector2 startLocation = m_Maze.MazeSettings.StartPoint;
                MazeCell startCell = m_Maze.GetCell(startLocation);
                m_Player.transform.parent = m_Maze.transform;
                m_Player.SetLocation(startCell);
                m_Player.enabled = true;
            }
        }       


        public void OnStartGame()
        {
            m_EnableInput = true;
            m_Player.EnableInput = true;
        }

        public void OnDestroyGame()
        {
            m_EnableInput = false;
            m_Player.EnableInput = false;
            //m_MazeGameUI.SetScore(m_Player.Points);
        }

        public void OnEndGame()
        {
            m_EnableInput = false;
            m_Player.EnableInput = false;
            // Add points

            //ScoreManager.Instance.BestScore = m_Player.Points;
            //m_MazeGameUI.SetScore(m_Player.Points);
        }

       
        private void M_Player_OnPlayerDead()
        {
            m_EnableInput = false;
            m_Player.EnableInput = false;
            /*ScoreManager.Instance.BestScore = m_Player.Points;
            m_MazeGameUI.SetScore(m_Player.Points);
            m_MazeGameUI.SetBestScore(ScoreManager.Instance.BestScore);   */        

            StopAllCoroutines();
            StartCoroutine(RoutinePlayerDead());
        }

        private void M_Player_OnPlayerHitObstacle()
        {
            m_EnableInput = false;
            m_Player.EnableInput = false;
            /*ScoreManager.Instance.BestScore = m_Player.Points;
            m_MazeGameUI.SetScore(m_Player.Points);
            m_MazeGameUI.SetBestScore(ScoreManager.Instance.BestScore);*/

            StopAllCoroutines();
            StartCoroutine(RoutinePlayerDead());
        }


        private IEnumerator RoutinePlayerDead()
        {
            yield return new WaitForSeconds(1.0f);

            // Reset Rotation maze
            m_Maze.transform.rotation = Quaternion.identity;
            m_MazeCurrentRotation = m_Maze.transform.rotation;
            m_MazeNextRotation = m_Maze.transform.rotation;
            m_MazeCurrentLerpTime = 0.0f;

            // Reset player position after a few seconds
            InitPlayer();

            m_EnableInput = true;
            m_Player.EnableInput = true;
            m_Player.EnableRagdoll();
        }


        private void M_Player_OnPlayerConsumeItem()
        {
            //ScoreManager.Instance.BestScore = m_Player.Points;
            // Remove items collected
            m_ItemsCollected += 1;
            //m_MazeGameUI.SetItems(m_ItemsCollected, m_Maze.NumberItems);
            if (m_ItemsCollected >= m_Maze.NumberItems)
            {
                // Open exit in current map
                m_Maze.EnableExit();
            }
            //m_MazeGameUI.SetScore(m_Player.Points);
        }
        

        private void M_Player_OnPlayerRespawn(IVector2 coords)
        {
            m_EnableInput = false;
            m_Player.EnableInput = false;

            StopAllCoroutines();
            StartCoroutine(RoutinePlayerPortal(coords));
        }        

        private IEnumerator RoutinePlayerPortal(IVector2 newcoords)
        {
            yield return new WaitForSeconds(1.0f);

            // Reset Rotation maze
            m_Maze.transform.rotation = Quaternion.identity;
            m_MazeCurrentRotation = m_Maze.transform.rotation;
            m_MazeNextRotation = m_Maze.transform.rotation;
            m_MazeCurrentLerpTime = 0.0f;

            // Set player in new coords
            MazeCell respawnCell = m_Maze.GetCellFromList(newcoords);
            m_Player.transform.parent = m_Maze.transform;
            m_Player.SetLocation(respawnCell);

            // Enable input
            m_EnableInput = true;
            m_Player.EnableInput = true;
            m_Player.EnableRagdoll();
        }

        private void M_Player_OnPlayerSolveMaze()
        {
            m_EnableInput = false;
            m_Player.EnableInput = false;

           /* ScoreManager.Instance.BestScore += m_Player.Points;
            m_MazeGameUI.SetScore(m_Player.Points);
            m_MazeGameUI.SetBestScore(ScoreManager.Instance.BestScore);*/

            // Disable player events
            m_Player.OnPlayerDead -= M_Player_OnPlayerDead;
            m_Player.OnPlayerRespawn -= M_Player_OnPlayerRespawn;
            m_Player.OnPlayerSolveMaze -= M_Player_OnPlayerSolveMaze;
            m_Player.OnPlayerConsumeItem -= M_Player_OnPlayerConsumeItem;
            m_Player.OnPlayerHitObstacle -= M_Player_OnPlayerHitObstacle;

            StopAllCoroutines();
            StartCoroutine(RoutinePlayerEndMaze());
        }        

        private IEnumerator RoutinePlayerEndMaze()
        {
            yield return new WaitForSeconds(1.0f);

            // TODO GENERATE A NEW MAZE
            // Add 1 level
            m_CurrentLevel++;
            // Initialize maze again
            Initialize();

            yield return new WaitForSeconds(0.2f);

            m_EnableInput = true;
            m_Player.EnableInput = true;
            m_Player.EnableRagdoll();

        }

       



        private void Update()
        {
            if (!m_EnableInput) return;

            // Update points
            //m_MazeGameUI.SetScore(m_Player.Points);

            HandleInput();

            UpdateMovement();
        }

        #region Input
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveDown();
            }

            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }
        }
        

        private void UpdateMovement()
        {
            m_MazeCurrentLerpTime += Time.deltaTime;
            if (m_MazeCurrentLerpTime > m_MazeRotationActionTime)
            {
                m_MazeCurrentLerpTime = m_MazeRotationActionTime;
            }
            float perc = m_MazeCurrentLerpTime / m_MazeRotationActionTime;

            m_Maze.transform.rotation = Quaternion.Lerp(m_MazeCurrentRotation, m_MazeNextRotation, perc);
            m_MazeCurrentRotation = m_Maze.transform.rotation;

            if (perc == 1)
            {
                m_MazeCurrentLerpTime = 0.0f;
            }
        }

        public void MoveUp()
        {
            if (m_CurrentDirection != Direction.UP)
            {
                m_CurrentDirection = Direction.UP;

                m_MazeCurrentRotation = m_Maze.transform.rotation;
                m_MazeNextRotation = Quaternion.Euler(new Vector3(40.0f, m_MazeCurrentRotation.y, m_MazeCurrentRotation.z));

                m_MazeCurrentLerpTime = 0.0f;
                m_Player.Move(m_CurrentDirection);
            }
        }

        public void MoveDown()
        {
            if (m_CurrentDirection != Direction.DOWN)
            {
                m_CurrentDirection = Direction.DOWN;

                m_MazeCurrentRotation = m_Maze.transform.rotation;
                m_MazeNextRotation = Quaternion.Euler(new Vector3(-(m_AmountRotation), m_MazeCurrentRotation.y, m_MazeCurrentRotation.z));

                m_MazeCurrentLerpTime = 0.0f;
                m_Player.Move(m_CurrentDirection);
            }
        }

        public void MoveLeft()
        {
            if (m_CurrentDirection != Direction.LEFT)
            {
                m_CurrentDirection = Direction.LEFT;

                m_MazeCurrentRotation = m_Maze.transform.rotation;
                m_MazeNextRotation = Quaternion.Euler(new Vector3(m_MazeCurrentRotation.x, m_MazeCurrentRotation.y, m_AmountRotation));
                m_MazeCurrentLerpTime = 0.0f;
                m_Player.Move(m_CurrentDirection);
            }
        }

        public void MoveRight()
        {
            if (m_CurrentDirection != Direction.RIGHT)
            {
                m_CurrentDirection = Direction.RIGHT;

                m_MazeCurrentRotation = m_Maze.transform.rotation;
                m_MazeNextRotation = Quaternion.Euler(new Vector3(m_MazeCurrentRotation.x, m_MazeCurrentRotation.y, -m_AmountRotation));
                m_MazeCurrentLerpTime = 0.0f;
                m_Player.Move(m_CurrentDirection);
            }
        }             

        #endregion Input

    }
}
