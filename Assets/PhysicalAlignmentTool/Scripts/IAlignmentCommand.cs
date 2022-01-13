using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAlignmentCommand
{
    // Start is called before the first frame update
    void Execute();

    void Undo();
}
