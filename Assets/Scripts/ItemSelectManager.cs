using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemSelectManager : MonoBehaviour
{
    public Camera uiCamera;
    public GameObject findedItem;

    // クリックしたアイテムの情報を表示する
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisplayItemSlotUI(false);
        }
    }

    public void DisplayItemSlotUI(bool begin)
    {
        //CleanItemSlotUI();
        // 最初のアイテムindexを呼び出すのかを決める
        if (!begin)
        {
            uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            // クリックした位置に物体があったら
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                // クリックしたアイテムスロットに情報(アイテムネーム)がいれば取り出す
                if (!hit.transform.gameObject.name.Equals("preventClickScreen") && !GameObject.Find(hit.transform.gameObject.name).transform.Find("itemName").GetComponent<Text>().text.Equals(""))
                {
                    findedItem = GameObject.Find(hit.transform.gameObject.name);

                    CleanItemSlotUI();

                    // 選択されたアイテムのindex
                    Debug.Log("SELECTED ITEM INDEX: " + Convert.ToInt32(findedItem.name.Substring(4)));
                    ItemCheckManager.itemSelectIndex = Convert.ToInt32(findedItem.name.Substring(4));

                    //選択したアイテムがキーアイテムじゃないならボタンをいかす
                    //if (findedItem.transform.Find("keyItem").GetComponent<Text>().text.Equals("N")) ItemUseAndDropButtonInteractable(true);
                    //else ItemUseAndDropButtonInteractable(false);

                    // クリックしたアイテムの情報を読み出す
                    GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
                    "[ " + "<color=#93DAFF>" + findedItem.transform.Find("itemName").GetComponent<Text>().text + "</color>" + "(" + "x" + findedItem.transform.Find("itemQty").GetComponent<Text>().text + ")" + " ]" +
                    "\n" +
                    findedItem.transform.Find("itemDesc").GetComponent<Text>().text;

                    // set item outline
                    GameObject.Find("itemSlotCanvas").transform.Find(hit.transform.gameObject.name).GetComponent<Outline>().effectDistance = new Vector2(10, 10);

                    // 選択されたアイテムがキーアイテムじゃないなら情報を保管
                    if (findedItem.transform.Find("keyItem").GetComponent<Text>().text.Equals("N"))
                    {
                        GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemName").GetComponent<Text>().text = findedItem.transform.Find("itemName").GetComponent<Text>().text;
                        GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemQty").GetComponent<Text>().text = findedItem.transform.Find("itemQty").GetComponent<Text>().text;
                        GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("keyItem").GetComponent<Text>().text = findedItem.transform.Find("keyItem").GetComponent<Text>().text;
                    }

                }
                // クリックしたアイテムスロットに情報がなかったら
                else if((!hit.transform.gameObject.name.Equals("preventClickScreen")))
                {
                    CleanItemSlotUI();
                    //ItemUseAndDropButtonInteractable(false);
                }
            }
            // クリックした位置に物体がなかったら
            else
            {
                //CleanItemSlotUI();
                //ItemUseAndDropButtonInteractable(false);
            }
        }
        // 最初のアイテム
        else
        {
            CleanItemSlotUI();

            //キーアイテムじゃないなら
            if (GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("keyItem").GetComponent<Text>().text.Equals("N"))
            {
                // ボタンをいかす
                ItemUseAndDropButtonInteractable(true);
                // 情報を保管
                GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemName").GetComponent<Text>().text = GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("itemName").GetComponent<Text>().text;
                GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemQty").GetComponent<Text>().text = GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("itemQty").GetComponent<Text>().text;
                GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("keyItem").GetComponent<Text>().text = GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("keyItem").GetComponent<Text>().text;
            }

                

            // 最初のアイテムの情報を読み出す
            GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
            "[ " + "<color=#93DAFF>" + GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("itemName").GetComponent<Text>().text + "</color>" + "(" + "x" + GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("itemQty").GetComponent<Text>().text + ")" + " ]" +
            "\n" +
            GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.Find("itemDesc").GetComponent<Text>().text;

            // set item outline
            GameObject.Find("itemSlotCanvas").transform.Find(GameObject.Find("itemSlotCanvas").transform.Find("item0").transform.gameObject.name).GetComponent<Outline>().effectDistance = new Vector2(10, 10);
        }

    }

    public void ItemUseAndDropButtonInteractable(bool sw)
    {
        GameObject.Find("Canvas").transform.Find("itemUseButton").GetComponent<Button>().interactable = sw;
        GameObject.Find("Canvas").transform.Find("itemDropButton").GetComponent<Button>().interactable = sw;
    }

    public void CleanItemSlotUI()
    {
        Debug.Log("CALL CleanItemSlotUI");

        // UI Clear(outline)
        for (int i = 0; i < 6; i++)
        {
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).GetComponent<Outline>().effectDistance = Vector2.zero;
        }
        // UI Clear(panel)
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text = null;

        // 選択されたアイテムのindex
        ItemCheckManager.itemSelectIndex = 0;

        // 選択されたアイテムの情報を初期化
        GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemName").GetComponent<Text>().text = "";
        GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("itemQty").GetComponent<Text>().text = "";
        GameObject.Find("Canvas").transform.Find("selectedItem").transform.Find("keyItem").GetComponent<Text>().text = "";
    }
}
