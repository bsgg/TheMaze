using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheMaze.MazeGenerator
{
    public class JSONMazeDataUI : BaseUI
    {
        [SerializeField]
        private Text m_Messages;

        [SerializeField]
        private InputField m_NameJSONFile;

        [SerializeField]
        private InputField m_NumberColumnsInput;
        private int m_NumberColums;

        [SerializeField]
        private InputField m_NumberRownsInput;
        private int m_NumberRows;

        private bool m_BlockInput;
        private string m_JSONPath = "";


        public override void Show()
        {
            base.Show();

            m_Messages.text = "";
            m_NameJSONFile.text = "Maze_O";
            m_NumberColums = 5;
            m_NumberRows = 5;
            m_NumberColumnsInput.text = m_NumberColums.ToString();
            m_NumberRownsInput.text = m_NumberRows.ToString();

            m_BlockInput = false;

        }

        public void OnJSONGenerate()
        {
            if (m_BlockInput) return;

            m_BlockInput = true;
            m_Messages.text = "Generating JSON file...";

           

            string nameMaze = "Maze_0";
            if (!string.IsNullOrEmpty(m_NameJSONFile.text))
            {
                nameMaze = m_NameJSONFile.text;
            }
            
            string outputName, outputPath;
            MazeJSONTool.CheckMazeJSONFile(nameMaze, out outputName, out outputPath);
            m_Messages.text = "JSON will be saved in " + outputPath;
            m_JSONPath = outputPath;


            // Columns and rows
            int nCols = 5;
            int nRows = 5;
            int auxParse = 0;
            if (int.TryParse(m_NumberColumnsInput.text, out auxParse))
            {
                if (auxParse > 0)
                {
                    nCols = auxParse;
                }
            }

            if (int.TryParse(m_NumberRownsInput.text, out auxParse))
            {
                if (auxParse > 0)
                {
                    nRows = auxParse;
                }
            }

            //MazeGenerator.Instance.GenerateRandomDataMaze(outputName, nCols, nRows);
            //MazeGenerator.Instance.OnEndGenerateJSONMaze += MazeGenerator_OnEndGenerateJSONMaze;


            // TEST CORRIDORS
            /*List<IVector2> path = new List<IVector2>() { new IVector2(3,2), new IVector2(3, 1), new IVector2(3, 0), new IVector2(4, 0), new IVector2(5,0),
                            new IVector2(5,1), new IVector2(5,2), new IVector2(6,2), new IVector2(6,3), new IVector2(6,4), new IVector2(5,4), new IVector2(5,5),
                            new IVector2(4,5), new IVector2(3,5),new IVector2(2,5),new IVector2(2,4), new IVector2(2,3), new IVector2(1,3), new IVector2(1,4),
                            new IVector2(0,4), new IVector2(0,3), new IVector2(0,2), new IVector2(0,1), new IVector2(0,0), new IVector2(1,0), new IVector2(2,0),
                            new IVector2(2,1), new IVector2(2,2)};
            MazeGenerator.Instance.GenerateDataMazeByPath("TestCorridor", 7, 6, path);*/

            List<IVector2> path = new List<IVector2>() { new IVector2(0,0), new IVector2(0,1), new IVector2(1,1),new IVector2(1,2),
                                                         new IVector2(2,2), new IVector2(3,2), new IVector2(3,1),new IVector2(3,0),
                                                         new IVector2(2,0), new IVector2(1,0)
                                                        };
            MazeGenerator.Instance.GenerateDataMazeByPath(nameMaze, 4, 3, path);
            MazeGenerator.Instance.OnEndGenerateJSONMaze += MazeGenerator_OnEndGenerateJSONMaze;
        }

        private void MazeGenerator_OnEndGenerateJSONMaze(string message)
        {
            MazeGenerator.Instance.OnEndGenerateJSONMaze -= MazeGenerator_OnEndGenerateJSONMaze;

            m_Messages.text = "JSON Saved in: " + m_JSONPath;

            m_BlockInput = false;

        }
    }
}
