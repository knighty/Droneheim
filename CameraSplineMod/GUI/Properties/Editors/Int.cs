using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droneheim.GUI.Properties.Editors
{
	public class Int : SimplePropertyEditor<int>
	{
		public override int ConvertFromString(string str)
		{
			return int.Parse(str);
		}

		public override string ConvertToString(int obj)
		{
			return obj.ToString();
		}
	}
}