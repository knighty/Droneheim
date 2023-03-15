using System;
using System.Collections.Generic;
using System.Linq;

namespace Droneheim.Commands
{
	public class CommandList
	{
		private const int MAX_SIZE = 100;
		private Stack<Command> undoStack;
		private Stack<Command> redoStack;

		public Action<Command> OnAdd;
		public Action<Command> OnUndo;
		public Action<Command> OnRedo;

		private static CommandList instance;
		public static CommandList Instance
		{
			get => instance ??= new CommandList();
		}

		private CommandList()
		{
			undoStack = new Stack<Command>(MAX_SIZE);
			redoStack = new Stack<Command>();
		}

		public Command Add(Command command)
		{
			undoStack.Push(command);
			command.Execute();
			redoStack.Clear();
			OnAdd?.Invoke(command);
			return command;
		}

		public Command Undo()
		{
			if (undoStack.Count == 0)
				return null;
			Command command = undoStack.Pop();
			command.Undo();
			redoStack.Push(command);
			OnUndo?.Invoke(command);
			return command;
		}

		public Command Redo()
		{
			if (redoStack.Count == 0)
				return null;
			Command command = redoStack.Pop();
			command.Execute();
			undoStack.Push(command);
			OnRedo?.Invoke(command);
			return command;
		}

		public override string ToString()
		{
			return
				$"[Undo Stack {undoStack.Count})]\n" +
				undoStack.Aggregate("", (str, command) => str + "- " + command.Name + "\n") +
				$"[Redo Stack {redoStack.Count})]\n" +
				redoStack.Aggregate("", (str, command) => str + "- " + command.Name + "\n");
		}
	}
}
