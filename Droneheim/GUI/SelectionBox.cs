using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Droneheim.GUI
{
	public class SelectionBox : MaskableGraphic
	{
		private Rect rect;
		public Rect Rect { get => rect; set => rect = value; }
	}
}
