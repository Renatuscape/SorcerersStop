using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedsPrefab : MonoBehaviour
{
    public Item weedsObject;
    public planterScript planterParent;
    void Start()
    {
        weedsObject = Items.FindByID("PLA000");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (TransientDataScript.CameraView == CameraView.Garden)
        {
            weedsObject.AddToPlayer();

            planterParent.currentWeeds--;
            Destroy(gameObject);
        }
    }
}
