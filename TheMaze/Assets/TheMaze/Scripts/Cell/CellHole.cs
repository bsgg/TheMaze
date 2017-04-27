using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze
{
    [System.Serializable]
    public class HoleData
    {
        public enum ETypeHole { NONE, PORTAL };
        public ETypeHole TypeHole;

        public IVector2 Coords;
        public IVector2 ConnectedCoords;
    }

    [System.Serializable]
    public class ObstacleData
    {
        public enum ETypeObstacle { NONE, SPIKEBALL };
        public ETypeObstacle TypeObstacle;

        public IVector2 Coords;
    }

    public class CellHole : MonoBehaviour
    { 
        [Header("Hole Data")]
        [SerializeField]
        private HoleData m_HoleData;
        public HoleData HoleData
        {
            get { return m_HoleData; }
            set { m_HoleData = value; }
        }

        [SerializeField]
        private GameObject m_HoleParticles;

        [SerializeField]
        private GameObject[] m_HolePortalParticles;

        [SerializeField]
        private Transform m_SpawnCollisionParticles;
        public Transform SpawnCollisionParticles
        {
            get { return m_SpawnCollisionParticles; }
        }
      
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetHoleData(HoleData data)
        {
            m_HoleData = data;
            
            switch (m_HoleData.TypeHole)
            {
                case HoleData.ETypeHole.NONE: 
                    m_HoleParticles.SetActive(true);
                break;

                case HoleData.ETypeHole.PORTAL:
                    m_HolePortalParticles[Random.Range(0, m_HolePortalParticles.Length)].SetActive(true);
                break;
            }
        }        

    }
}
