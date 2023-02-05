using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin values")]
    public int value = 10;
    public float pickupRange = 10f;
    public float pickupSpeed = 10f;

    bool coinSummoned = false;
    bool coinCollected = false;


    private void Start()
    {
        Destroy(gameObject, 300f); // Destroy the game object automatically after 5 minutes, if it has not been destroyed by being collected.
    }

    void FixedUpdate()
    {

        // Create a Raycast sphere to find all enemies within the pull radius
        Collider[] itemsNearby = Physics.OverlapSphere(transform.position, pickupRange, Physics.AllLayers);

        // apply pull force to each enemy
        foreach (Collider item in itemsNearby)
        {
            if (item.transform.tag == "Player")
            {
                print("item = " + item.name);
                Transform playerTransform = item.GetComponent<Transform>();
                PlayerMoney playerMoney = item.GetComponent<PlayerMoney>();
                // If the coin is less than 1 unit away from the player then add to the player's balance and destroy the coin.
                if (Vector3.Distance(playerTransform.position, transform.position) < 1f && !coinCollected)
                {
                    coinCollected = true;
                    print("Player collecting coin.");
                    playerMoney.PlayPickupSound(); // Play a sound effect when collecting the coin.
                    playerMoney.AddToBalance(value);
                    Destroy(gameObject);
                }
                else if (!coinSummoned)
                {
                    print("Moving coin towards player.");
                    coinSummoned = true;
                    StartCoroutine(SummonCoin(playerTransform));
                }
            }
        }
    }

    private IEnumerator SummonCoin(Transform playerTransform) // Called when the player presses the "coinPickupKey" when looking at the weapon.
    {

        // Start keeping track of the time, t will be used to decide how far along slerp we are.
        float t = 0;

        // Remember the original coin's rotation and postition.
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;

        // Calculate how far away the coin is from the player.
        float distanceFromPlayer = Vector3.Distance(originalPos, playerTransform.position);

        // Work out the centre of the two points and move it down slightly so that the Slerp arcs UP instead of to the side.
        Vector3 centrePos = (originalPos + playerTransform.position) / 2f;
        centrePos -= new Vector3(0, 1, 0);
        Vector3 coinRelCenter = transform.position - centrePos; // Work out the relative centre of the coin object.

        // While the coin is in transit...
        while (distanceFromPlayer > 0.1f && t <= 1)
        {
            Vector3 playerRelCenter = playerTransform.position - centrePos; // Work out the relative centre of the coinplayer in the loop incase the player moves.
            transform.position = Vector3.Lerp(coinRelCenter, playerRelCenter, t) + centrePos; // Move the coin...

            t += Time.deltaTime * pickupSpeed; // Increment t.
            yield return new WaitForEndOfFrame(); // Wait for the next frame
        }
        coinSummoned = false;
        yield return null;
    }
}
