using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheMaze.MazeGenerator
{
    public class PrefabMazeDataUI : BaseUI
    {
        [SerializeField]
        private Text m_Messages;

        [SerializeField]
        private Text m_ListJSONToGenerate;

        private bool m_BlockInput;

        public override void Show()
        {
            base.Show();

            m_Messages.text = "";
            m_ListJSONToGenerate.text = "";


            List<string> lFiles = MazeJSONTool.ListJSONFiles();

            if ((lFiles != null) && (lFiles.Count > 0))
            {
                m_ListJSONToGenerate.text = "Number JSON Files to turn into prefab: " + lFiles.Count + "\n";
                for (int i = 0; i< lFiles.Count; i++)
                {
                    string list = " - " + lFiles[i] + "\n";
                    m_ListJSONToGenerate.text += list;
                }


                
            }
            else
            {
                m_ListJSONToGenerate.text = "No JSON files to turn into prefab. Please place the JSON files in " + MazeJSONTool.PATHJSONTOPREFAB;
            }

            m_BlockInput = false;           
        }

        private void MazeGenerator_OnEndGeneratePrefabsMaze(string message)
        {
            MazeGenerator.Instance.OnEndGeneratePrefabsMaze -= MazeGenerator_OnEndGeneratePrefabsMaze;
            m_Messages.text = "Prefabs generated";
            m_BlockInput = false;
        }

        public void OnPrefabGerate()
        {
            if (m_BlockInput) return;

            m_BlockInput = true;

            MazeGenerator.Instance.OnEndGeneratePrefabsMaze += MazeGenerator_OnEndGeneratePrefabsMaze;
            m_Messages.text = "The operation will take a while, be patient...";

            MazeGenerator.Instance.GenerateMazePrefabs();


        }
	}
}
