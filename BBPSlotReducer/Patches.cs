using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Net;
using System.IO;
//BepInEx stuff
using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HarmonyLib; //god im hoping i got the right version of harmony
using BepInEx.Configuration;
using System.Collections.Generic;
using TMPro;

namespace BBPSlotReducer
{
    [HarmonyPatch(typeof(ItemManager))]
    [HarmonyPatch("Awake")]
    class SlotAwakePatch
    {
        static bool Prefix(ItemManager __instance)
        {
            __instance.maxItem = BaldiSlotReducer.Slots - 1;
            return true;
        }
    }

    [HarmonyPatch(typeof(ItemManager))]
    [HarmonyPatch("UpdateSelect")]
    class PreventSelectPatch
    {
        static bool Prefix(ItemManager __instance)
        {
            if (__instance.selectedItem > BaldiSlotReducer.Slots - 1)
            {
                __instance.selectedItem = BaldiSlotReducer.Slots - 1;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ElevatorScreen))]
    [HarmonyPatch("QueueShop")]
    class NoShopForYou
    {
        static bool Prefix()
        {
            return false;
        }
    }


    /*[HarmonyPatch(typeof(StoreScreen))] //fix this later, for now just disable john man's shop
    [HarmonyPatch("BuyItem")]
    class OverrideBuyItemTheLazyWay
    {
        //the below code is copied from DNSpy and when I'm feeling like it I'll figure out ILCode
        static bool Prefix(StoreScreen __instance, int val, ref bool[] ___itemPurchased, ref int ___ytps, ref ItemObject[] ___itemForSale, ref int ___pointsSpent, ref TMP_Text ___totalPoints, ref Image[] ___forSaleImage, ref GameObject[] ___forSaleHotSpots, ref TMP_Text[] ___itemPrice, ref ItemObject[] ___inventory, ref Image[] ___inventoryImage, ref GameObject[] ___counterHotSpots, ref bool ___purchaseMade, ref AudioManager ___audMan, ref SoundObject[] ___audBuy, ref SoundObject[] ___audUnafforable)
        {
            if (val <= 5)
            {
                if (!___itemPurchased[val] && ___ytps >= ___itemForSale[val].price)
                {
                    ___itemPurchased[val] = true;
                    ___ytps += ___itemForSale[val].price;
                    ___pointsSpent += ___itemForSale[val].price;
                    ___totalPoints.text = ___ytps.ToString();
                    ___forSaleImage[val].gameObject.SetActive(false);
                    ___forSaleHotSpots[val].SetActive(false);
                    ___itemPrice[val].text = "SOLD";
                    ___itemPrice[val].color = Color.red;
                    int i = 0;
                    while (i < ___inventory.Length)
                    {
                        if (i != BaldiSlotReducer.Slots && ___inventory[i].itemType == Items.None)
                        {
                            ___inventory[i] = ___itemForSale[val];
                            ___inventoryImage[i].sprite = ___inventory[i].itemSpriteSmall;
                            if (i > BaldiSlotReducer.Slots)
                            {
                                ___counterHotSpots[(i - (BaldiSlotReducer.Slots + 1))].SetActive(true);
                                ___inventoryImage[i].gameObject.SetActive(true);
                                break;
                            }
                            break;
                        }
                        else
                        {
                            UnityEngine.Debug.Log("add to i");
                            i++;
                        }
                    }
                    ___purchaseMade = true;
                    if (!___audMan.QueuedUp)
                    {
                        ___audMan.QueueRandomAudio(___audBuy);
                    }
                    __instance.StandardDescription();
                    return false;
                }
                if (!___audMan.QueuedUp)
                {
                    ___audMan.QueueRandomAudio(___audUnafforable);
                    return false;
                }
                return false;
            }
            return true;
        }
    }
*/
    [HarmonyPatch(typeof(HudManager))]
    [HarmonyPatch("SetItemSelect")]
    class UpdateSetItemGraphics
    {
        static void Postfix(HudManager __instance, ref RawImage[] ___itemBackgrounds, ref Image[] ___itemSprites)
        {
            if (BaldiSlotReducer.Slots == 5)
            {
                return;
            }
            for (int i = BaldiSlotReducer.Slots; i < 5; i++)
            {
                ___itemBackgrounds[i].color = Color.black;
                ___itemSprites[i].sprite = BaldiSlotReducer.CrossSprite;
            }
        }
    }
}
