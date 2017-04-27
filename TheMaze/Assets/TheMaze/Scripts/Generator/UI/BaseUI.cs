using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheMaze.MazeGenerator
{
    public class BaseUI : MonoBehaviour
    {
		public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
	}
}
