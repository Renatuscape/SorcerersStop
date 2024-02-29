using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class StationShop : MonoBehaviour
{
    public Character shopkeeper;
    public Shop shop;
    public ShopMenu shopMenu;
    public string shopkeeperID;
    public string shopID;
    public TransientDataScript transientData;
    public BoxCollider2D col;

    void Start()
    {
        shopMenu = TransientDataCalls.GetMenuSystem().shopMenu;
        transientData = TransientDataCalls.GetTransientData();
        col = GetComponent<BoxCollider2D>();
        Debug.Log($"Station spawned with shopkeeperID {shopkeeperID} and shopID {shopID}.");
    }

    private void Update()
    {
        if (TransientDataScript.CameraView == CameraView.Normal)
        {
            if (transientData != null)
            {
                if (TransientDataScript.CameraView == CameraView.Normal)
                {
                    col.enabled = true;
                }

                else
                {
                    col.enabled = false;
                }
            }
            else
            {
                transientData = TransientDataCalls.GetTransientData();
            }
        }
    }
    public void OnMouseDown()
    {
        if (NullCheck())
        {
            if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
            {
                shopMenu.SetUpShop(shopkeeper, shop);
            }
        }

    }

    public void OnMouseOver()
    {
        if (NullCheck())
        {
            if (TransientDataScript.GameState == GameState.Overworld && TransientDataScript.CameraView == CameraView.Normal)
            {
                transientData.PrintFloatText($"{shop.name}");
            }
        }
    }

    public void OnMouseExit()
    {
        transientData.DisableFloatText();
    }

    private bool NullCheck()
    {
        if (shopkeeper == null || string.IsNullOrEmpty(shopkeeper.objectID))
        {
            shopkeeper = Characters.all.FirstOrDefault(c => c.objectID == shopkeeperID);
            Debug.Log($"Station found shopkeeper: {shopkeeper.name}");
        }
        if (shop == null || string.IsNullOrEmpty(shop.objectID))
        {
            shop = shopkeeper.GetShop(shopID);
            Debug.Log($"Shop found: {shop.name}");

        }
        if (shopMenu == null)
        {
            shopMenu = TransientDataCalls.GetMenuSystem().shopMenu;
        }

        return shopkeeper != null && shop != null;
    }
}
