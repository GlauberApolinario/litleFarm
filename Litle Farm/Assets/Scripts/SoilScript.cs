using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilScript : MonoBehaviour
{
    public bool isWet;
    public float timeToDry = 120;
    public Material materialDry;
    public Material materialWet;
    private MeshRenderer thisMeshRenderer;
    public float dryCoolDown = 0;

    void Awake(){
        thisMeshRenderer = GetComponent<MeshRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        dryCoolDown = timeToDry;
    }

    // Update is called once per frame
    void Update()
    {
        thisMeshRenderer.material = isWet ? materialWet : materialDry;
        if(isWet){
            dryCoolDown-=Time.deltaTime;
            if(dryCoolDown<= 0){
                isWet = false;
            }
        }
    }

    public void Water(){
        isWet = true;
        dryCoolDown = timeToDry;
    }
}
