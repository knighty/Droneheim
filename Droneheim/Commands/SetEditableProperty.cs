using Droneheim.GUI.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Droneheim.Commands
{
	internal class SetEditableProperty<T> : Command
	{
		public override string Name => "Set Value";

		private EditableProperty<T> editableProperty;
		private T value;
		private T previousValue;

		public SetEditableProperty(EditableProperty<T> editableProperty, T value)
		{
			this.editableProperty = editableProperty;
			this.previousValue = this.editableProperty.Value;
			this.value = value;
		}

		public override void Execute()
		{
			this.editableProperty.Value = value;
		}

		public override void Undo()
		{
			this.editableProperty.Value = previousValue;
		}
	}
}
