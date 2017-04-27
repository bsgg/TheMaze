using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheMaze.MazeGenerator
{
    public class MenuGeneratorUI : BaseUI
    {

        [SerializeField]
        private JSONMazeDataUI m_SectionJSON;

        [SerializeField]
        private PrefabMazeDataUI m_SectionPrefab;


        public override void Show()
        {
            base.Show();
            m_SectionJSON.Hide();
            m_SectionPrefab.Hide();
        }
       

        public void OnJSONButtonPress()
        {
            m_SectionJSON.Show();
            m_SectionPrefab.Hide();
            Hide();
        }

        public void OnPrefabButtonPress()
        {
            m_SectionJSON.Hide();
            m_SectionPrefab.Show();
            Hide();
        }


    }
}
