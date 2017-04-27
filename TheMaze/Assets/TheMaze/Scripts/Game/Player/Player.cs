using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheMaze;

namespace TheMazeGame
{
    public class Player : MonoBehaviour
    {
        public delegate void OnPlayerAction();
        public event OnPlayerAction OnPlayerDead;
        public event OnPlayerAction OnPlayerSolveMaze;
        public event OnPlayerAction OnPlayerConsumeItem;
        public event OnPlayerAction OnPlayerHitObstacle;

        public delegate void OnPlayerCellAction(IVector2 coords);
        public event OnPlayerCellAction OnPlayerRespawn;


        [SerializeField]
        private MazeDirection m_CurrentDirection;

        private bool m_IsRigidBodyEnabled = true;

        [SerializeField]
        private float m_Force = 200.0f;
        [SerializeField]
        private float m_Height = 15.0f;

        [SerializeField]
        private Rigidbody m_RigidBody;

        [SerializeField]
        private float m_HorizontalAxis;
        [SerializeField]
        private float m_VerticalAxis;


        [SerializeField]
        private bool m_EnableInput = false;
        public bool EnableInput
        {
            get { return m_EnableInput; }
            set { m_EnableInput = value; }
        }
        [SerializeField]
        private float m_FallForce = 0.0f;


        private TheMazeGame.Direction m_CurrentDirectionMovement;

        [Header("Particles")]
        [SerializeField]
        private GameObject m_HitParticlePrefab;

        [SerializeField] private int m_Points = 0;
        public int Points
        {
            get { return m_Points; }
            set { m_Points = value; }
        }

        void LateUpdate()
        {
            if (m_IsRigidBodyEnabled)
            {                
                Vector3 movement = new Vector3 (m_HorizontalAxis, m_FallForce, m_VerticalAxis);
                m_RigidBody.AddForce(movement * m_Force);
            }
        }
                
        public void Move(TheMazeGame.Direction dir)
        {
            if (!m_EnableInput) return;

            m_CurrentDirectionMovement = dir;

            if (dir == TheMazeGame.Direction.DOWN)
            {
                m_HorizontalAxis = 0.0f;
                m_VerticalAxis = -1.0f;
            }
            if (dir == TheMazeGame.Direction.UP)
            {
                m_HorizontalAxis = 0.0f;
                m_VerticalAxis = 1.0f;
            }
            if (dir == TheMazeGame.Direction.LEFT)
            {
                m_HorizontalAxis = -1.0f;
                m_VerticalAxis = 0.0f;
            }
            if (dir == TheMazeGame.Direction.RIGHT)
            {
                m_HorizontalAxis = 1.0f;
                m_VerticalAxis = 0.0f;
            }
        }
        
        public void SetLocation(MazeCell nextCell)
        {
            if (nextCell)
            {
                transform.localPosition = new Vector3(nextCell.transform.localPosition.x, m_Height, nextCell.transform.localPosition.z);
            }
        }


        public void SetDirection(MazeDirection direction)
        {
            m_CurrentDirection = direction;
            transform.localRotation = direction.ToRotation();
        }

        #region Collision

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "TriggerFloor")
            {
                MazeTileFloorCollision tileCollision = other.GetComponent<MazeTileFloorCollision>();
                if (tileCollision != null)
                {                
                    if (tileCollision != null)
                    {
                        if (tileCollision.MazeCell.TypeCell == MazeCell.ETypeCell.ENDLOCATION)
                        {
                            m_EnableInput = false;
                            m_HorizontalAxis = 0.0f;
                            m_VerticalAxis = 0.0f;
                            m_FallForce = 0.0f;
                            DisableRagdoll();
                            m_Points += 20;
                            if (OnPlayerSolveMaze != null)
                            {
                                OnPlayerSolveMaze();
                            }
                        }
                        if (tileCollision.MazeCell.TypeCell == MazeCell.ETypeCell.HOLE)
                        {
                            // Check type hole
                            if (tileCollision.CellHole != null)
                            {
                                m_EnableInput = false;
                                m_HorizontalAxis = 0.0f;
                                m_VerticalAxis = 0.0f;
                                m_FallForce = -1.0f;
                            }                           
                        }
                    }                    
                }
            }else if (other.tag == "TriggerEndHole") // Trigger end hole
            {
                m_EnableInput = false;
                m_HorizontalAxis = 0.0f;
                m_VerticalAxis = 0.0f;
                m_FallForce = 0.0f;
                //m_RigidBody.useGravity = false;
                DisableRagdoll();

                MazeTileFloorCollision holeCollision = other.GetComponent<MazeTileFloorCollision>();
                if (holeCollision != null)
                {
                    if (holeCollision.CellHole.HoleData.TypeHole == HoleData.ETypeHole.NONE)
                    {
                        // Spawn particles
                        GameObject objHitParticle = Instantiate(m_HitParticlePrefab);
                        objHitParticle.transform.position = holeCollision.CellHole.SpawnCollisionParticles.position;

                        m_Points -= 1; 
                        if (m_Points < 0)
                        {
                            m_Points = 0;
                        }

                        if (OnPlayerDead != null)
                        {
                            OnPlayerDead();
                        }
                    }
                    else if (holeCollision.CellHole.HoleData.TypeHole == HoleData.ETypeHole.PORTAL)
                    {
                        // Respawn player
                        if (OnPlayerRespawn != null)
                        {                            
                            OnPlayerRespawn(holeCollision.CellHole.HoleData.ConnectedCoords);
                        }
                    }
                }                
            }

            // Hit item
            if (other.tag == "Item")
            {
                Item item = other.GetComponent<Item>();
                if (item != null)
                {
                    m_Points += item.Points;
                    item.ConsumeItem();
                }

                if (OnPlayerConsumeItem != null)
                {
                    OnPlayerConsumeItem();
                }
            }

            // Hit item
            if (other.tag == "Obstacle")
            {
                // Spawn particles
                GameObject objHitParticle = Instantiate(m_HitParticlePrefab);
                objHitParticle.transform.position = transform.position;
                m_EnableInput = false;
                m_HorizontalAxis = 0.0f;
                m_VerticalAxis = 0.0f;
                m_FallForce = 0.0f;
                //m_RigidBody.useGravity = false;
                DisableRagdoll();

                Debug.LogError("OBSTACLE!!");
                if (OnPlayerHitObstacle != null)
                {
                    OnPlayerHitObstacle();
                }
            }
        }


        public void EnableRagdoll()
        {
            m_IsRigidBodyEnabled = true;
            m_RigidBody.isKinematic = false;
            m_RigidBody.detectCollisions = true;
            m_RigidBody.useGravity = true;
        }
        public void DisableRagdoll()
        {
            m_IsRigidBodyEnabled = false;
            m_RigidBody.isKinematic = true;
            m_RigidBody.detectCollisions = false;
            m_RigidBody.useGravity = false;
        }

        #endregion Collision
    }
}
