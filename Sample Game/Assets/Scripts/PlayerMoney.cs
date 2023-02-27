using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoney : MonoBehaviour
{
    public int currentBalance;
    public TMPro.TextMeshProUGUI balanceUI;
    public AudioSource pickupSoundSource;

    // Start is called before the first frame update
    void Start()
    {
        int savedBalance = GetSavedBalance();
        if (savedBalance > 0)
            currentBalance = savedBalance;

        UpdateHUD();
    }

    public int GetSavedBalance()
    {
        int savedBalance = PlayerPrefs.GetInt("MONEY_CurrentBalance"); // Get the balance saved and set it.
        return savedBalance;
    }

    private void UpdateHUD()
    {
        // Set the value on the HUD / UI so the player can see how much money they have.
        balanceUI.text = currentBalance.ToString();
    }

    public void PlayPickupSound()
    {
        pickupSoundSource.Play();
    }

    public void AddToBalance(int value)
    {
        print("Added " + value + " to player balance.");
        currentBalance += value;
        PlayerPrefs.SetInt("MONEY_CurrentBalance", currentBalance); // Save the value of the current balance after increasing it.
        UpdateHUD();
    }
}
