using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class HidingSpot : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 2f;
    public Transform hidePosition;
    public Vector3 hiddenRotation = new Vector3(0, 180, 0);


    [Header("Optional FX")]
    public GameObject interactionPrompt;

    private bool isPlayerHiding;
    private GameObject hidingPlayer;
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private CharacterController playerCharacterController;
    private PlayerMovement playerMovement;
    private Animator playerAnimator;

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!isPlayerHiding)
        {
            CheckForNearbyPlayer();
        }
    }

    private void CheckForNearbyPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= interactionDistance)
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(true);
            }
            else
            {
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(false);
            }
        }
    }

    // Call this from PlayerMovement's Interact function
    public void TryInteract(GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= interactionDistance)
        {
            if (!isPlayerHiding)
                HidePlayer(player);
            else if (player == hidingPlayer)
                UnhidePlayer();
        }
    }

    private void HidePlayer(GameObject player)
    {
        if (hidePosition == null) return;

        isPlayerHiding = true;
        hidingPlayer = player;

        // Store original transform
        originalPlayerPosition = player.transform.position;
        originalPlayerRotation = player.transform.rotation;

        // Get and disable components
        playerCharacterController = player.GetComponent<CharacterController>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAnimator = player.GetComponent<Animator>();

        if (playerCharacterController != null)
            playerCharacterController.enabled = false;

        if (playerMovement != null)
        {
            // Disable specific behaviors but keep script enabled for input
            playerMovement.enabled = false;
        }

        // Move player to hiding spot
        player.transform.position = hidePosition.position;
        player.transform.rotation = Quaternion.Euler(hiddenRotation);


        if (playerAnimator != null)
            playerAnimator.SetBool("Hiding", true);

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void UnhidePlayer()
    {
        if (!isPlayerHiding || hidingPlayer == null) return;

        // Restore position
        hidingPlayer.transform.position = originalPlayerPosition;
        hidingPlayer.transform.rotation = originalPlayerRotation;

        // Re-enable components
        if (playerCharacterController != null)
            playerCharacterController.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerAnimator != null)
            playerAnimator.SetBool("Hiding", false);

        isPlayerHiding = false;
        hidingPlayer = null;
    }

    public bool IsPlayerHiding()
    {
        return isPlayerHiding;
    }

}