using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilScript : MonoBehaviour
{
    public bool isWet;
    public float timeToDry = 120;
    public Material materialDry;
    public Material materialWet;
    public GameObject cropObject;
    public int seedIndex;
    private int oldCropStage;
    public int cropStage;
    private MeshRenderer thisMeshRenderer;
    public float dryCoolDown = 0;
    private float growInterval = 1;
    private float growCoolDown = 0;
    private float growChance = 0.025f;

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

        //Dry soil
        if(isWet){
            dryCoolDown-=Time.deltaTime;
            if(dryCoolDown<= 0){
                isWet = false;
            }
        }

        // Update crop
        if(oldCropStage != cropStage){

            //Destroy crops
            if(cropObject != null){
                Destroy(cropObject);
            }

            //Plant Crop
            if(cropStage>0){
                var gm = GameManager.Instance;
                var prefabs = seedIndex == 1 ? gm.pumpkinPrefabs : gm.beetPrefabs;
                var cropPrefab = prefabs[cropStage - 1];
                var position = transform.position;
                var rotation = cropPrefab.transform.rotation * Quaternion.Euler(Vector3.up * Random.Range(0, 360)); 
                cropObject = Instantiate(cropPrefab, position, rotation);
            }
        }
        oldCropStage = cropStage;

        //Grow Crops
        if(!IsEmpty() && !IsFinished()){
            if((growCoolDown -= Time.deltaTime) <= 0){
                growCoolDown = growInterval;
                var realChance = growChance;
                if (isWet){
                    realChance *= 2f;
                }
                if (Random.Range(0f, 1f)<realChance){
                    cropStage++;
                }
            }
        }
    }

    public void Water(){
        isWet = true;
        dryCoolDown = timeToDry;
    }

    public bool IsEmpty(){
        return cropStage == 0;
    }
    public bool IsFinished(){
        return cropStage == 5;
    }

    public void Seed(int index){
        if(!IsEmpty()) return;
        seedIndex = index;
        cropStage = 1;
    }
}
