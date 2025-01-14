using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerPrefabScript : MonoBehaviour
{
    public GameObject passengerTarget;
    public SpriteRenderer spriteRenderer;
    public Item spiritEssence;
    public PassengerData passengerData;

    public bool isReady;

    public void Initialise(PassengerData passengerData)
    {
        this.passengerData = passengerData;

        if (!isReady)
        {
            passengerTarget = GameObject.Find("PassengerTarget");
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (passengerData.seatID == "A")
            {
                gameObject.name = "PassengerA";
                transform.position = new Vector3(passengerTarget.transform.position.x - 0.031f, passengerTarget.transform.position.y, -2);
            }
            else
            {
                gameObject.name = "PassengerB";
                transform.position = new Vector3(passengerTarget.transform.position.x, passengerTarget.transform.position.y, -2);
                transform.Rotate(new Vector3(0, -180, 0));
            }

            spiritEssence = Items.FindByID("CAT000");
        }

        isReady = true;

        UpdatePassengerData();
    }

    public void UpdatePassengerData()
    {
        if (isReady)
        {
            if (passengerData.isActive)
            {
                spriteRenderer.sprite = SpriteFactory.GetPassengerByID(passengerData.spriteID);

                CalculateFare();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Attempted to set up passenger before it was ready.");
        }
    }

    void CalculateFare()
    {
        float distance = CalculateDistance(passengerData.origin, passengerData.destination);
        Debug.Log("Distance between A and B: " + distance);
        passengerData.fare = (int)(distance * distance) * (int)Mathf.Ceil(Player.GetCount(StaticTags.GuildLicense, name) * 0.1f);
        Debug.Log("Fare calculated to " + passengerData.fare);
    }

    public static float CalculateDistance(Location pointA, Location pointB)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(pointB.mapX - pointA.mapX, 2) + Mathf.Pow(pointB.mapY - pointA.mapY, 2));
        return distance;
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            if (passengerData.destination.objectID == TransientDataScript.GetCurrentLocation().objectID)
            {
                int fate = Player.GetCount(StaticTags.Fate, "PassengerPrefabScript");

                MoneyExchange.AddHighestDenomination(passengerData.fare, true, out var totalAdded);
                AudioManager.PlaySoundEffect("handleCoins", -0.2f);

                if (passengerData.fare < totalAdded)
                {
                    LogAlert.QueueTextAlert("I was unable to accept the total fare. Better go to the bank!");
                }
                else
                {
                    LogAlert.QueueTextAlert($"{passengerData.passengerName} paid {passengerData.fare} shillings.");
                }

                // Tip is paid out in hellers and scripted items are updated to track
                Player.Add(StaticTags.Heller, Random.Range(0, passengerData.fare), true);
                Player.Add(StaticTags.TotalPassengers);
                Player.Add(StaticTags.TotalFare, passengerData.fare);

                // Spirit Essence drop
                if (Random.Range(0, 100) > 80 - (fate * 4))
                {
                    Player.Add(spiritEssence.objectID);
                    AudioManager.PlaySoundEffect("cloth3");
                }

                if (Player.GetCount(StaticTags.OnBoardService, name) > 1)
                {
                    PassengerReviewManager.RollForReview(passengerData);
                }

                passengerData.isActive = false;
                passengerData.characterID = string.Empty;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnMouseOver()
    {
        if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
        {
            TransientDataScript.PrintFloatText(passengerData.passengerName + "\n" + passengerData.destination.name);
        }
    }

    public void OnMouseExit()
    {
        TransientDataScript.DisableFloatText();
    }
}
