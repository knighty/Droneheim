using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droneheim.Commands
{
	abstract public class Command
	{
		abstract public string Name { get; }
		abstract public void Execute();
		abstract public void Undo();

		public override string ToString()
		{
			return Name;
		}
	}
}
