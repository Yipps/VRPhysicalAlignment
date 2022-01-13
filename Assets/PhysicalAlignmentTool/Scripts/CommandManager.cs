using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEditor;
using UnityEngine;


public static class CommandManager
{
    //public static Stack<IAlignmentCommand> commands;

    public static Stack<IAlignmentCommand> undoCommands;

    public static Stack<IAlignmentCommand> redoCommands;

    public static void DoCommand(IAlignmentCommand command)
    {
        command.Execute();
        
        undoCommands.Push(command);
        
        redoCommands.Clear();
    }

    static void Undo()
    {
        if (undoCommands.Count > 0)
        {
            IAlignmentCommand undoCommand = undoCommands.Pop();
            undoCommand.Undo();
            
            redoCommands.Push(undoCommand);
            
        }
    }

    static void Redo()
    {
        if (undoCommands.Count > 0)
        {
            IAlignmentCommand redoCommand = redoCommands.Pop();
            redoCommand.Execute();
            
            undoCommands.Push(redoCommand);
        }
    }
    

}
