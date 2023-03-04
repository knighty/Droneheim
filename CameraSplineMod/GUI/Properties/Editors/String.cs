using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droneheim.GUI.Properties.Editors
{
	public class String : SimplePropertyEditor<string>
	{
		public override string ConvertFromString(string str)
		{
			return str;
		}

		public override string ConvertToString(string obj)
		{
			return obj;
		}
	}
}
