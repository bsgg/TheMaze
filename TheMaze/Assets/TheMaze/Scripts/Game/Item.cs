using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMazeGame
{
    public class Item : MonoBehaviour
    {
        public delegate void OnItemAction();
        public event OnItemAction OnSpawnItem;

       // [SerializeField] private ADSUtility.ShrinkEffect m_ShrinkObject;

        /// <summary>
        /// Number cells
        /// </summary>        
        [SerializeField] private int m_Points;
        public int Points
        {
            get { return m_Points; }
        }

        [SerializeField]
         private TheMaze.MazeCell m_OwnerCell;
        public TheMaze.MazeCell OwnerCell
        {
            get { return m_OwnerCell; }
            set { m_OwnerCell = value; }
        }


        private BoxCollider m_Collider;

        [SerializeField]
        private GameObject m_ConsumeParticlesPrefab;
        [SerializeField]
        private Transform m_ConsumeParticlesSpawn;
        [SerializeField]
        private AudioSource m_ConsumeSound;


        private void Start()
        {
            //StartCoroutine(WaitforRespawn());
            m_Collider = GetComponent<BoxCollider>();

        }

        public void DoStart(TheMaze.MazeCell ownerCell)
        {
            m_OwnerCell = ownerCell;

            // Start respawn
            StopAllCoroutines();
            StartCoroutine(WaitforRespawn());
        }      

        private IEnumerator WaitforRespawn()
        {
            float seconds = Random.Range(15.0f, 30.0f);
            yield return new WaitForSeconds(seconds);
            // Shrink item;
            //yield return StartCoroutine(m_ShrinkObject.ShrinkRoutine());

            // Free cell
            if (m_OwnerCell != null)
            {
                m_OwnerCell.TypeCell = TheMaze.MazeCell.ETypeCell.NONE;
            }

            // Destroy item
            if (OnSpawnItem != null)
            {
                OnSpawnItem();
            }
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }



        public void ConsumeItem()
        {
            StopAllCoroutines();
            StartCoroutine(WaitToDestroy());
        }

        private IEnumerator WaitToDestroy()
        {
            // Destroy item
            /*if (OnSpawnItem != null)
            {
                OnSpawnItem();
            }*/
            m_Collider.enabled = false;

            if (m_ConsumeParticlesPrefab != null)
            {
                GameObject go = Instantiate(m_ConsumeParticlesPrefab);
                if (go != null)
                {
                    go.transform.position = m_ConsumeParticlesSpawn.position;
                }

            }
            if (m_ConsumeSound != null)
            {
                m_ConsumeSound.Play();
            }

            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }

    }
}
