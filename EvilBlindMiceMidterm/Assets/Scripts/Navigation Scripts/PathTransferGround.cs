using UnityEngine;

public class PathTransferGround : MonoBehaviour
{
    GameObject player;
    Chunk chunkComponent;
    bool isInBounds = false;
    bool hasChosenPath = false;
    bool canAlterPlayer = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if (isInBounds && !hasChosenPath && !player.GetComponent<DefaultMovementState>().isOnCieling)
        {
            if (Input.GetButtonDown("ChangeDirectionRight") && !hasChosenPath
                && chunkComponent.possiblePaths.Contains(PossiblePaths.LSRPaths.right))
            {
                MakePathChoice(true);
                hasChosenPath = true;
            }
            else if (Input.GetButtonDown("ChangeDirectionLeft") && !hasChosenPath 
                && chunkComponent.possiblePaths.Contains(PossiblePaths.LSRPaths.left))
            {
                MakePathChoice(false);
                hasChosenPath = true;
            }
        }

        if (hasChosenPath && player.GetComponent<DefaultMovementState>().cancelPlayerMovement != 0 && canAlterPlayer)
        {
            player.GetComponent<DefaultMovementState>().cancelPlayerMovement = 0;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chunkComponent = transform.parent.GetComponentInChildren<Chunk>();
            if (player.transform.eulerAngles.z == 0 || player.transform.eulerAngles.z == 360)
            {
                isInBounds = true;
            }
            else if (player.transform.eulerAngles.z == -180 || player.transform.eulerAngles.z == 180)
            {
                isInBounds = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAlterPlayer = false;
            player.GetComponent<DefaultMovementState>().cancelPlayerMovement = 1;
        }
    }

    void MakePathChoice(bool isRight)
    {
        if (!isRight)
        {
            player.transform.rotation = Quaternion.Euler(
               player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y - 90, player.transform.rotation.eulerAngles.z);
        }
        else
        {
            player.transform.rotation = Quaternion.Euler(
              player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 90, player.transform.rotation.eulerAngles.z);
        }
    }

}
