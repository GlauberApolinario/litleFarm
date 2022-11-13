using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float movementSpeed = 10;
    public int itemIndex = 0;
    public float itemOffset = 2;
    private Rigidbody thisRigidbody;
    private GameObject holdenObject;
    void Awake() {
        thisRigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update (){
        if(holdenObject != null){
        holdenObject.transform.position = transform.position + new Vector3(0, itemOffset, 0);
        }
    }
    // Update is called once per frame
    void FixedUpdate() {
        // Create input vector
        bool isUp = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isDown = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool isLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool isRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        float inputX = isRight ? 1 : isLeft ? -1 : 0;
        float inputY = isUp ? 1 : isDown ? -1 : 0;
        Vector2 movementVector = new Vector2(inputX, inputY);

        // Get forward
        Camera camera = Camera.main;
        float eulerY = camera.transform.eulerAngles.y;
        Quaternion forward = Quaternion.Euler(0, eulerY, 0);

       // Create vector
        Vector3 walkVector = new Vector3(movementVector.x, 0, movementVector.y);
        walkVector = forward * walkVector;
        walkVector *= movementSpeed;

        // Apply input to character
        thisRigidbody.AddForce(walkVector, ForceMode.Force);
    }

    void OnTriggerEnter(Collider other) {
        GameObject otherObject = other.gameObject;
        //Sensor
        if(otherObject.CompareTag("Sensor")){
            var sensorScript = otherObject.GetComponent<SensorScript>();
            var index = sensorScript.itemIndex;
            UpdateIndex(index);
            Debug.Log("Colidiu com um sensor");
        }
        //Soil
        if(otherObject.CompareTag("Soil")){
            var SoilScript = otherObject.GetComponent<SoilScript>();
            
            //With Watering can
            if(itemIndex == 3) {
                SoilScript.Water();
            }

            //With seeds
            if(itemIndex == 1 || itemIndex == 2){
                if(SoilScript.IsEmpty()){
                    SoilScript.Seed(itemIndex);
                }
            }

            //With air
            if(itemIndex == 0){
                if(SoilScript.IsFinished()){
                    var seedIndex = SoilScript.seedIndex;
                    SoilScript.RemoveCrop();
                    UpdateIndex(seedIndex+3);

                }
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        var otherObject = other.gameObject;

        //SellBox
        if(otherObject.CompareTag("SellBox")){
            if(itemIndex == 4 || itemIndex ==5){

                //Teleport fruit
                var fruitObject = holdenObject;
                holdenObject = null;
                var offset = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), Random.Range(-1f,1f));
                var destination = GameManager.Instance.depositBoxTransform.position + offset;
                fruitObject.transform.position = destination;

                //Enable Gravity
                var fruitRigidBody = fruitObject.GetComponent<Rigidbody>();
                if(fruitRigidBody != null){
                    fruitRigidBody.useGravity = true;
                }

                //Reset Index
                UpdateIndex(0);

                //Give money
                GameManager.Instance.coins += GameManager.Instance.coinsPerFruit;
                Debug.Log("Player coins: "+GameManager.Instance.coins);

            }
        }
    }
    private void UpdateIndex(int index){
        this.itemIndex = index;

        //Destroy previous object
        if(holdenObject !=null){
            Destroy(holdenObject);
            holdenObject = null;
        }

        //Create new object
        GameObject newObjectPrefab = GameManager.Instance.itemObject[index];
        if(newObjectPrefab != null){
            var position = transform.position;
            var rotation = newObjectPrefab.transform.rotation;
            holdenObject = Instantiate(newObjectPrefab, position, rotation);
        }
    }
}
