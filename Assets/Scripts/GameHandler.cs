using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public Player player;
    public Animator fadeToBlackAnim;
    public PauseMenu pauseMenu;

    private GameObject[] guards;
    private List<GuardMovement> guardMovements = new List<GuardMovement> ();

    private List<CheckpointSingle> seenCheckpoints = new List<CheckpointSingle> ();
    private CheckpointSingle currentCheckpoint;
    public Vector3 spawnpoint;

    private GameObject[] buttons;
    private List<DeactivationButton> deactivationButtons = new List<DeactivationButton> ();

    private List<char> shownMechanics = new List<char>(); // Keyboard key for mechanic

    private bool respawning = false;
    private bool endingGame = false;

    private void Awake()
    {
        guards = GameObject.FindGameObjectsWithTag("Guard");
        foreach(GameObject guard in guards)
        {
            guardMovements.Add(guard.GetComponent<GuardMovement>());
        }

        Transform checkpointsTransform = transform.Find("Checkpoints");

        currentCheckpoint = null;

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();
            checkpointSingle.SetGameHandler(this);
        }

        buttons = GameObject.FindGameObjectsWithTag("Deactivator");
        foreach(GameObject button in buttons)
        {
            deactivationButtons.Add(button.GetComponent<DeactivationButton>());
        }
    }

    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        // Only update checkpoint if player not being chased
        bool playerBeingChased = false;
        foreach(GuardMovement guardMovement in guardMovements)
        {
            if (guardMovement.IsChasing())
            {
                playerBeingChased = true;
            }
        }
        if (!playerBeingChased)
        {
            // Only update checkpoint if it hasnt been seen before
            if (!seenCheckpoints.Contains(checkpointSingle))
            {
                seenCheckpoints.Add(checkpointSingle);
                currentCheckpoint = checkpointSingle;
            }
        }
    }

    public void Respawn()
    {
        if (!respawning)
        {
            respawning = true;
            StartCoroutine(RespawnWithFade());
        }
    }

    IEnumerator RespawnWithFade()
    {
        Time.timeScale = 0.5f;
        fadeToBlackAnim.gameObject.transform.Find("GotCaughtText").gameObject.SetActive(true);
        fadeToBlackAnim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;
        // Reset player inventory
        player.ClearInventory();
        // Reset ItemWorldSpawners
        GameObject[] ItemWorldSpawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach(GameObject itemWorldSpawnerObj in ItemWorldSpawners)
        {
            itemWorldSpawnerObj.GetComponent<ItemWorldSpawner>().SpawnItem();
        }
        // Reset Cameras
        foreach(DeactivationButton d in deactivationButtons)
        {
            d.ResetCamera();
        }
        if (currentCheckpoint != null)
            player.transform.position = currentCheckpoint.transform.position;
        else
            player.transform.position = spawnpoint;
        fadeToBlackAnim.SetTrigger("FadeOut");
        fadeToBlackAnim.gameObject.transform.Find("GotCaughtText").gameObject.SetActive(false);
        respawning = false;
    }

    public bool MechanicShown(char c)
    {
        return shownMechanics.Contains(c);
    }

    public void AddMechanic(char c)
    {
        if (c != '0')
            if (!shownMechanics.Contains(c))
                shownMechanics.Add(c);
    }

    public void EndGame()
    {
        if (!endingGame)
        {
            endingGame = true;
            StartCoroutine(EndGameWithFade());
        }
    }

    IEnumerator EndGameWithFade()
    {
        Time.timeScale = 0.5f;
        fadeToBlackAnim.gameObject.transform.Find("EndGameText").gameObject.SetActive(true);
        fadeToBlackAnim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;
        fadeToBlackAnim.SetTrigger("FadeOut");
        fadeToBlackAnim.gameObject.transform.Find("EndGameText").gameObject.SetActive(false);
        respawning = false;
        endingGame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
