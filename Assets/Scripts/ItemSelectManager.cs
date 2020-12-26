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
            uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                // クリックしたアイテムスロットに情報(アイテムネーム)がいれば取り出す
                if (!GameObject.Find(hit.transform.gameObject.name).transform.Find("itemName").GetComponent<Text>().text.Equals(""))
                {
                    findedItem = GameObject.Find(hit.transform.gameObject.name);

                    CleanItemSlotUI();

                    // クリックしたアイテムの情報を読み出す
                    GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
                    "[ " + "<color=#93DAFF>" + findedItem.transform.Find("itemName").GetComponent<Text>().text + "</color>" + "(" + "x" + findedItem.transform.Find("itemQty").GetComponent<Text>().text + ")" + " ]" +
                    "\n" +
                    findedItem.transform.Find("itemDesc").GetComponent<Text>().text;

                    // set item outline
                    GameObject.Find("itemSlotCanvas").transform.Find(hit.transform.gameObject.name).GetComponent<Outline>().effectDistance = new Vector2(10, 10);
                }
            }
            // ないなら
            else
            {
                Debug.Log("ddd");
                CleanItemSlotUI();
            }
        }
    }

    public void DisplayItemSlotUI(bool begin)
    {
        // 最初のアイテムindexを呼び出すのかを決める
        if (!begin)
        {
            uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                // クリックしたアイテムスロットに情報(アイテムネーム)がいれば取り出す
                if (!GameObject.Find(hit.transform.gameObject.name).transform.Find("itemName").GetComponent<Text>().text.Equals(""))
                {
                    findedItem = GameObject.Find(hit.transform.gameObject.name);

                    CleanItemSlotUI();

                    // クリックしたアイテムの情報を読み出す
                    GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
                    "[ " + "<color=#93DAFF>" + findedItem.transform.Find("itemName").GetComponent<Text>().text + "</color>" + "(" + "x" + findedItem.transform.Find("itemQty").GetComponent<Text>().text + ")" + " ]" +
                    "\n" +
                    findedItem.transform.Find("itemDesc").GetComponent<Text>().text;

                    // set item outline
                    GameObject.Find("itemSlotCanvas").transform.Find(hit.transform.gameObject.name).GetComponent<Outline>().effectDistance = new Vector2(10, 10);
                }
            }
            // ないなら
            else
            {
                Debug.Log("ddd");
                CleanItemSlotUI();
            }
        }
        else
        {
            CleanItemSlotUI();

            // 最初のアイテムの情報を読み出す
            GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text =
            "[ " + "<color=#93DAFF>" + GameObject.Find("item0").transform.Find("itemName").GetComponent<Text>().text + "</color>" + "(" + "x" + GameObject.Find("item0").transform.Find("itemQty").GetComponent<Text>().text + ")" + " ]" +
            "\n" +
            GameObject.Find("item0").transform.Find("itemDesc").GetComponent<Text>().text;

            // set item outline
            GameObject.Find("itemSlotCanvas").transform.Find(GameObject.Find("item0").transform.gameObject.name).GetComponent<Outline>().effectDistance = new Vector2(10, 10);
        }

    }

    public void CleanItemSlotUI()
    {
        // UI Clear(outline)
        for (int i = 0; i < 6; i++)
        {
            GameObject.Find("itemSlotCanvas").transform.Find("item" + i).GetComponent<Outline>().effectDistance = Vector2.zero;
        }
        // UI Clear(panel)
        GameObject.Find("Panel").transform.Find("Text").GetComponent<Text>().text = null;
    }
}
