using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADSUtility
{
    public class IntensityLightEffect : Effect
    {

        [SerializeField]
        private float m_MinIntensity;
        [SerializeField]
        private float m_MaxIntensity;
        private float m_CurrentIntensity;
        private bool m_UpdateToMax;

        private Light m_Light;

        [SerializeField]
        private float m_Speed;

        protected override void DoStart()
        {
            m_Light = GetComponent<Light>();
            m_CurrentIntensity = m_Light.intensity;
            m_Light.intensity = m_MinIntensity;
            m_UpdateToMax = true;

            base.DoStart();
        }

        protected override void DoUpdate()
        {
            base.DoUpdate();

            if (m_UpdateToMax)
            {
                if (m_CurrentIntensity < m_MaxIntensity)
                {
                    m_CurrentIntensity += (m_Speed * Time.deltaTime);
                    m_Light.intensity = m_CurrentIntensity;
                }else
                {
                    m_CurrentIntensity = m_MaxIntensity;
                    m_Light.intensity = m_CurrentIntensity;
                    m_UpdateToMax = false;

                }
            }else
            {
                if (m_CurrentIntensity > m_MinIntensity)
                {
                    m_CurrentIntensity -= (m_Speed * Time.deltaTime);
                    m_Light.intensity = m_CurrentIntensity;
                }
                else
                {
                    m_CurrentIntensity = m_MinIntensity;
                    m_Light.intensity = m_CurrentIntensity;
                    m_UpdateToMax = true;

                }
            }

        }
    }
}
