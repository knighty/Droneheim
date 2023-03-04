using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Droneheim.GUI.Properties.Editors
{
	public interface Factory
	{
		GameObject GetEditor(object property);
	}
}
