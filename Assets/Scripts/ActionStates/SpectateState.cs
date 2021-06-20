using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpectateState : State
{
  Player[] players;
  string lookingAt;
  int playerIndex;
  public SpectateState(Character character, StateMachine stateMachine) : base(character, stateMachine){
    players = GameGlobals.globals.GetPlayers();
  }

  public override void Enter()
  {
      base.Enter();
      for(int i = 0; i < players.Length; i++) {
        if (!players[i].dead) {
          Camera.main.GetComponent<CameraFollow>().lookAt = players[i].transform;
          lookingAt = players[i].playerName;
          playerIndex = i;
        }
      }
  }
  public override void Exit()
  {
      base.Exit();
      Camera.main.GetComponent<CameraFollow>().lookAt = Player.localPlayer.transform;
  }

  public override void HandleInput()
  {
    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
      while (true) {
        if (playerIndex == 0) {
          playerIndex = players.Length - 1;
        } else {
          playerIndex -= 1;
        }
        if (!players[playerIndex].dead) {
          Camera.main.GetComponent<CameraFollow>().lookAt = players[playerIndex].transform;
          lookingAt = players[playerIndex].playerName;
          break;
        }
      }
    } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
      while (true) {
        if (playerIndex == players.Length) {
          playerIndex = 0;
        } else {
          playerIndex += 1;
        }
        if (!players[playerIndex].dead) {
          Camera.main.GetComponent<CameraFollow>().lookAt = players[playerIndex].transform;
          lookingAt = players[playerIndex].playerName;
          break;
        }
      }
    }
  }
  public override void LogicUpdate()
  {
      base.LogicUpdate();
      if (!MatchManager.manager.inGame) {
        state_machine.ChangeState(character.idle_state);
      }
  }
  public override void PhysicsUpdate(){}
}
