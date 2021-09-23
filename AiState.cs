using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AiState
{
    void Enter();
    void Update();
    void Exit();
}
