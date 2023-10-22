using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    private GameHandler gameHandler;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            gameHandler.PlayerThroughCheckpoint(this);
        }
    }

    public void SetGameHandler(GameHandler gameHandler)
    {
        this.gameHandler = gameHandler;
    }
}