using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class DestructableObject : NetworkBehaviour
{
    [SyncVar(hook=nameof(DestroyChunk))]
    bool destroyed;

    public void DestroyChunk(bool oldVal, bool newVal) {
      Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision other) {
      if (other.gameObject.tag == "Player") {
        this.destroyed = true;
        Character character = other.gameObject.GetComponent<Character>();
        character.action_machine.ChangeState(character.idle_state);
        character.movement_machine.ChangeState(character.falling_state);
      }
    }
}
