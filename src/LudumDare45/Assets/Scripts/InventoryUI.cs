﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    private PlayerManager playerManager; 

    public Transform itemsParent;
    public bool isOpen;
    private Vector3 posRight;
    private Vector3 posLeft;

    InventorySlot[] slots;
    private bool userCanCangeSatus = true;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.Instance;
        playerManager.onShopInteractionCallback += UpdateUiForShop;

        inventory = Inventory.Instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        posRight = transform.localPosition;
        posLeft = new Vector3(-posRight.x, posRight.y, 0);

        isOpen = false;
        changeUIVisible();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            isOpen = !isOpen;
            changeUIVisible();
        }

        if (isOpen)
        {
            if (playerManager.facingRight())
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, posLeft,Time.deltaTime * 4f);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, posRight, Time.deltaTime * 4f);
            }
        }
    }

    void changeUIVisible()
    {
        if (playerManager.facingRight())
        {
            transform.localPosition = posLeft;
        }
        else
        {
            transform.localPosition = posRight ;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isOpen);
        }
    }

    public void UpdateUiForShop()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.getSize())
            {
                slots[i].setShopOpen(true);
            }
            else
            {
                slots[i].setShopOpen(false);
            }
        }
        changeUIVisible();
    }

    void UpdateUI() {
        Debug.Log("Update UI");
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.getSize())
            {
                slots[i].addItem(inventory.invItems[i],i);
            }
            else
            {
                slots[i].clearSlot();
            }
        }
        StartCoroutine("AutoShowUI");
    }

    IEnumerator AutoShowUI()
    {
        bool curStatus = isOpen;
        if (curStatus)
            yield break;

        userCanCangeSatus = false;
        isOpen = true;
        changeUIVisible();
        yield return new WaitForSeconds(2f);
        isOpen = false;
        changeUIVisible();
        userCanCangeSatus = true;
    }

}
