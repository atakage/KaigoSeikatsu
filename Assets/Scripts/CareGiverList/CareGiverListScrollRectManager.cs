using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class CareGiverListScrollRectManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public bool completedFadeOut;
    public bool beingDrag;
    public CareGiverListSharingObjectManager careGiverListSharingObjectManager;


    private void Start()
    {
        careGiverListSharingObjectManager = GameObject.Find("CareGiverListSharingObjectManager").GetComponent<CareGiverListSharingObjectManager>();
    }

    public void OnEndDrag(PointerEventData data)
    {
        careGiverListSharingObjectManager.scrollHelpMessageBoxGameObj.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData data)
    {
        careGiverListSharingObjectManager.scrollHelpMessageBoxGameObj.SetActive(true);
    }

    

}
