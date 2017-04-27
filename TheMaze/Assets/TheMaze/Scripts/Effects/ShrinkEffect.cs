using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADSUtility
{
    public class ShrinkEffect : Effect
    {        
        [SerializeField] private float m_TargetScale;
        [SerializeField]
        private float m_Speed;
        private float m_CurrentScale;

        protected override void DoStart()
        {
            base.DoStart();

            m_CurrentScale = transform.localScale.x;
        }

        public override void DoEffect()
        {
            base.DoEffect();

            StartCoroutine(ShrinkRoutine());           
        }

        public IEnumerator ShrinkRoutine()
        {
            while (m_CurrentScale > m_TargetScale)
            {
                m_CurrentScale -= m_Speed * Time.deltaTime;

                transform.localScale = new Vector3(m_CurrentScale, m_CurrentScale, m_CurrentScale);

                yield return new WaitForEndOfFrame();
            }
            transform.localScale = new Vector3(m_TargetScale, m_TargetScale, m_TargetScale);
        }
    }
}
