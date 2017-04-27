using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADSUtility
{
    public class DestroyAfterWhile : MonoBehaviour
    {
        [SerializeField]
        private float m_DelayToDestroy;

	    void Start ()
        {
            DestroyObject(gameObject, m_DelayToDestroy);
		
	    }	
		
	}
}
