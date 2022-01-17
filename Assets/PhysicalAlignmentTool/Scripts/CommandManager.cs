using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEditor;
using UnityEngine;


public static class CommandManager
{
    //public static Stack<IAlignmentCommand> commands;

    public static Stack<IAlignmentCommand> undoCommands = new Stack<IAlignmentCommand>();

    public static Stack<IAlignmentCommand> redoCommands = new Stack<IAlignmentCommand>();

    public static void DoCommand(IAlignmentCommand command)
    {
        command.Execute();
        
        undoCommands.Push(command);
        
        redoCommands.Clear();
    }

    public static void Undo()
    {
        if (undoCommands.Count > 0)
        {
            IAlignmentCommand undoCommand = undoCommands.Pop();
            undoCommand.Undo();
            
            redoCommands.Push(undoCommand);
            DebugMe();
        }
    }

    public static void Redo()
    {
        if (redoCommands.Count > 0)
        {
            IAlignmentCommand redoCommand = redoCommands.Pop();
            redoCommand.Execute();
            
            undoCommands.Push(redoCommand);
            DebugMe();
        }
    }

    private static void DebugMe()
    {
        Debug.Log("Undo Stack: " + undoCommands.Count);
        Debug.Log("Redo Stack " + redoCommands.Count);
    }
    

}
