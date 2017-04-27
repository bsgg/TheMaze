using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

namespace TheMaze.MazeGenerator
{
    public class MazeJSONTool
    {
        public static string PATHJSONFILES = "/Resources/MazeData/";

        public static void CheckMazeJSONFile(string nameFile, out string outputName, out string outputFullPath)
        {
            string root = Application.dataPath + PATHJSONFILES;

            outputName = nameFile;
            outputFullPath = root + nameFile + ".json";

            // Directore doesn't exit, create a new one and return same name of the file
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);                
            }

            if (File.Exists(outputFullPath))
            {

                outputName = nameFile + "_copy";
                outputFullPath = root + nameFile + ".json";

            }
        }



        /// <summary>
        /// Maze to save the maze on a file
        /// </summary>
        /// <param name="mazeData"></param>
        public static void SaveMazeOnFile(MazeJSONData mazeData)
        {            
            string jsonString = LitJson.JsonMapper.ToJson(mazeData);

            string root = Application.dataPath + PATHJSONFILES;

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
                
            // Check path
            string path = root + mazeData.Name + ".json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = File.Create(path))
            {
                byte[] info = new System.Text.UTF8Encoding(true).GetBytes(jsonString);
                fs.Write(info, 0, info.Length);
                fs.Close();
            }
        }


        public static string PATHJSONTOPREFAB = "/Resources/MazeDataGenerated/";

        public static List<string> ListJSONFiles()
        {
            List<string> lfiles = new List<string>();
            // Get name files
            string root = Application.dataPath + PATHJSONTOPREFAB;
            if (Directory.Exists(root))
            {
                string[] auxFiles = Directory.GetFiles(root, "*.json");
                if (auxFiles != null)
                {
                    
                    for (int i= 0; i< auxFiles.Length; i++)
                    {
                        string filename = Path.GetFileNameWithoutExtension(auxFiles[i]);
                        lfiles.Add(filename);
                    }
                }
            }

            return lfiles;
        }

        


       
        public static bool LoadMazeJSON(string nameMaze, out MazeJSONData mazeData)
        {
            mazeData = null;
            // Load data from resources
            string pathFile = "MazeDataGenerated/" + nameMaze ;            

            TextAsset text_asset = (TextAsset)Resources.Load(pathFile, typeof(TextAsset));
            if (text_asset == null)
            {
                Debug.Log("ERROR: Could not find file: Assets/Resources/" + pathFile);
                return false;
            }

            string json_string = text_asset.ToString();
            if (!string.IsNullOrEmpty(json_string))
            {
                mazeData = JsonMapper.ToObject<MazeJSONData>(json_string);
                return true;
            }

            return false;

        }
    }
}
