using System.Collections;
//allow us to creat a list of objects
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class scrChicken : MonoBehaviour
{

    bool isJumping = false;
    bool isJumpingFoward = false;
    bool isJumpingBackward = false;
    bool isJumpingLeft = false;
    bool isJumpingRight = false;

    //help keep track of strips and next strips
    public GameObject strip1;
    public GameObject strip2;
    public GameObject strip3;
    public GameObject strip4;
    public GameObject strip5;
    public GameObject strip6;
    public GameObject strip7;
    public GameObject strip8;
    public GameObject strip9;
    public GameObject strip10;
    public GameObject strip11;
    public GameObject strip12;
    public GameObject strip13;
    public GameObject strip14;
    public GameObject strip15;

    public GameObject defaultChicken;


    public GameObject[] poolOfStripsPrefabs;

    private List<GameObject> strips;
    const int MAXNUMROADS = 4;
    const int MAXNUMGRASS = 2;
    
    int indexCurrentStrip;
    int indexEndOfBareStrips = 2;
    int indexOfTopRoad    = 5;
    int indexOfMiddleRoad = 4;
    int indexOfBottomRoad = 3;
    int indexOfSingleRoad = 6;
    int indexOfGrassOccupiedStart = 7;

    float stripWidth;
    float deltaSidewaysDistance = 0.0f;
    float deltaDistance = 0.0f;
    float deltaMidwayDistance = 0.0f;

    bool isPlayingDeathAni = false;
    static int indexOfPreviousGrassStrip = 0;
    static int indexOfPreviousRoadStrip = 6;
    static bool isAGrassStripToBeDisplayed = false;//otherwise a road strip is to be displayed
    static bool wasARoadPreviouslyDisplayed = false;
    static int numOfRoadStripsToDisplay = 1;
    static int numOfGrassStripsToDisplay = 1;
    static int indexOfPreviousStrip = 0;


    public Vector3 jumpTargetLocation;
    public float movingSpeed = 100f;
    private float midWayPointZ;
    public float jumpHeightIncr = 10.0f;

    

    // Start is called before the first frame update
    void Start()
    {
        isJumping = false;

        strips = new List<GameObject>();
        strips.Add(strip1);
        strips.Add(strip2);
        strips.Add(strip3);
        strips.Add(strip4);
        strips.Add(strip5);
        strips.Add(strip6);
        strips.Add(strip7);
        strips.Add(strip8);
        strips.Add(strip9);
        strips.Add(strip10);
        strips.Add(strip11);
        strips.Add(strip12);
        strips.Add(strip13);
        strips.Add(strip14);
        strips.Add(strip15);


        indexCurrentStrip = -1;

        numOfRoadStripsToDisplay = Random.Range(1, MAXNUMROADS);
        numOfGrassStripsToDisplay = Random.Range(1, MAXNUMGRASS);

        GameObject itemToGetWidth = poolOfStripsPrefabs[1];

        Transform parentSrtip = itemToGetWidth.transform.GetChild(0) as Transform;
        Transform childStrip = parentSrtip.GetChild(0) as Transform;
        stripWidth = childStrip.gameObject.GetComponent<Renderer>().bounds.size.z;
        //how far foward to move the strip

        //determine the distance from one strip to thr next strip that will serve as the sideways distance
        //as well as the foward and backward distance the chicken needs to travel
        deltaSidewaysDistance = strips[2].transform.position.z - strips[1].transform.position.z;
        deltaDistance = strip2.transform.position.z - strip1.transform.position.z;
        deltaMidwayDistance = deltaDistance / 2.0f;//distance of chicken,halfway,destination

    }

    // Update is called once per frame
    void Update()
    {
        //mouse click jump
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down");
            if (isJumping == false)
            {
                isJumping = true;
                jump();
            }
        }
        //key boARD  press jump
        if(Input.GetKeyDown(KeyCode.LeftArrow) && !isJumping)
        {
            if (!isJumpingLeft)
            {
                isJumpingLeft = true;
                isJumping = true;
                //call jumpLeftSetUp()
                jumpLeftSetUp();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
        {
            if (!isJumpingFoward)
            {
                isJumpingFoward = true;
                isJumping = true;
                //call jumpLeftSetUp()
                jumpFowardSetUp();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && !isJumping)
        {
            if (!isJumpingRight)
            {
                isJumpingRight = true;
                isJumping = true;
                //call jumpLeftSetUp()
                jumpRightSetUp();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isJumping)
        {
            if (!isJumpingBackward && indexCurrentStrip != 0 )
            {
                isJumpingBackward = true;
                isJumping = true;
                //call jumpLeftSetUp()
                jumpBackwardSetUp();
            }
        }

        //move chicken based on direction
        if (isJumpingFoward)
        {
            //rotate the chicken to point to the north
            defaultChicken.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

            if (this.transform.position.z < deltaMidwayDistance)
            {//move foward a little and move up
                this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y
                    + jumpHeightIncr * Time.deltaTime,this.transform.position.z + (movingSpeed * Time.deltaTime));
                
            }
            else if (this.transform.position.z <= jumpTargetLocation.z)
            {//falling down still moving foward
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y
                    - jumpHeightIncr * Time.deltaTime, this.transform.position.z + (movingSpeed * Time.deltaTime));
            }
            else
            {//make chicken flat on the ground and at jumpTargetLocation
                this.transform.position = jumpTargetLocation;
                isJumpingFoward = false;
                isJumping = isJumpingBackward && isJumpingFoward && isJumpingLeft && isJumpingRight;
            }
            
        }//if(isJumpingFoward)
        else if (isJumpingBackward) {
            //rotate the chicken to point to the south
            defaultChicken.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

            if (this.transform.position.z > deltaMidwayDistance)
            {//move backward a little and move up
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y
                    + jumpHeightIncr * Time.deltaTime, this.transform.position.z - (movingSpeed * Time.deltaTime));

            }
            else if (this.transform.position.z >= jumpTargetLocation.z)//MOD: CHANGE TO >=
            {//falling down still moving backward
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y
                    - jumpHeightIncr * Time.deltaTime, this.transform.position.z - (movingSpeed * Time.deltaTime));
            }
            else
            {//make chicken flat on the ground and at jumpTargetLocation
                this.transform.position = new Vector3(this.transform.position.x, strips[indexCurrentStrip].transform.position.y,
                    jumpTargetLocation.z);
                isJumpingBackward = false;
                isJumping = isJumpingBackward && isJumpingFoward && isJumpingLeft && isJumpingRight;
            }
        }//else if(isjumpingbackwards)
        else if (isJumpingLeft)
        {
            //rotate the chicken to point to the west
            defaultChicken.transform.localEulerAngles = new Vector3(0f, -90f, 0f);

            if (this.transform.position.x > deltaMidwayDistance)
            {//jumping up and moving left a little
                this.transform.position = new Vector3(this.transform.position.x - (movingSpeed * Time.deltaTime), this.transform.position.y
                    + jumpHeightIncr * Time.deltaTime, this.transform.position.z);

            }
            else if (this.transform.position.x >= jumpTargetLocation.x)
            {//falling down still moving left
                this.transform.position = new Vector3(this.transform.position.x - (movingSpeed * Time.deltaTime), this.transform.position.y
                    - jumpHeightIncr * Time.deltaTime, this.transform.position.z);
            }
            else
            {//make chicken flat on the ground and at jumpTargetLocation
                this.transform.position = jumpTargetLocation;
                isJumpingLeft = false;
                isJumping = isJumpingBackward && isJumpingFoward && isJumpingLeft && isJumpingRight;
            }
        }//else if (isJumpingLeft)
        else if(isJumpingRight){
            
            //rotate the chicken to point to the east
            defaultChicken.transform.localEulerAngles = new Vector3(0f, 90f, 0f);

            if (this.transform.position.x < deltaMidwayDistance)
            {//jumping up and moving right a little
                this.transform.position = new Vector3(this.transform.position.x + (movingSpeed * Time.deltaTime), this.transform.position.y
                    + jumpHeightIncr * Time.deltaTime, this.transform.position.z);

            }
            else if (this.transform.position.x <= jumpTargetLocation.x)
            {//falling down still moving right
                this.transform.position = new Vector3(this.transform.position.x + (movingSpeed * Time.deltaTime), this.transform.position.y
                    - jumpHeightIncr * Time.deltaTime, this.transform.position.z);
            }
            else
            {//make chicken flat on the ground and at jumpTargetLocation
                this.transform.position = jumpTargetLocation;
               
                isJumpingRight = false;
                isJumping = isJumpingBackward && isJumpingFoward && isJumpingLeft && isJumpingRight;
                
            }
        }

/*
        if (isJumping)
        {
            //jump up and move foward a little
            if (this.transform.position.z < midWayPointZ)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y
                    + jumpHeightIncr * Time.deltaTime, this.transform.position.z
                    + movingSpeed * Time.deltaTime);
            }

            //falling down still moving foward
            else if (this.transform.position.z <= jumpTargetLocation.z)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y
                    - jumpHeightIncr * Time.deltaTime, this.transform.position.z
                    + movingSpeed * Time.deltaTime);
            }
            //make the chicken flat on the ground and at jumptarget
            else // rewatch
            {
                this.transform.position = new Vector3(this.transform.position.x, strips[indexCurrentStrip].transform.position.y
                    , jumpTargetLocation.z);
                isJumping = false;
            }
        }*/
    }

    void jump()
    {//we want to move the chcken in the next strip
     // this.transform.position = new Vector3( transform.position.x,transform.position.y,strip5.transform.position.z);

        //move the strip current index by one 
        indexCurrentStrip += 1;
        
        //get the strip at the index withen the strips list
        GameObject nextStrip = strips[indexCurrentStrip] as GameObject;
       
        //get the z position of this new strip and apply it to the chicken
        jumpTargetLocation = new Vector3(transform.position.x,transform.position.y,
            nextStrip.transform.position.z);

        midWayPointZ = (this.transform.position.z + jumpTargetLocation.z) /2;

        spawnNewStrip();
        
    }

    void jumpLeftSetUp()
    {

        //calculate the destination landing point of the chicken
        jumpTargetLocation = new Vector3(this.transform.position.x - deltaDistance,
            strips[indexCurrentStrip].transform.position.y, this.transform.position.z);
        //calculaye the halfway point of the chicken fpr moving up and then down
        deltaMidwayDistance = (this.transform.position.x + jumpTargetLocation.x) / 2.0f;
        
    }
    void jumpRightSetUp()
    {
        //calulate the destination landing point of the chicken tpo the right
        jumpTargetLocation = new Vector3(this.transform.position.x + deltaDistance,
            strips[indexCurrentStrip].transform.position.y, this.transform.position.z);
        //calculate the halfway point of the chicken for moving up then down
        deltaMidwayDistance = (this.transform.position.x + jumpTargetLocation.x) / 2.0f;
    }

    void jumpFowardSetUp()
    {
        //move the strips current index by 1
        indexCurrentStrip += 1;
        //get the z position of the new strip and apply it to chicken
        jumpTargetLocation = new Vector3(this.transform.position.x, strips[indexCurrentStrip + 1].transform.position.y,
            this.transform.position.z + deltaDistance);
        deltaMidwayDistance = (this.transform.position.z + jumpTargetLocation.z) / 2.0f;
        //instantiate a new strip right after the last strip
        spawnNewStrip() ;
    }

    void jumpBackwardSetUp()
    {
        //note, as we jump backward we need to get the previous strips height except
        //when we jump back into the big green patch
        if (indexCurrentStrip > 1)
        {
            //decrment the current strips idea
            indexCurrentStrip -= 1;

            jumpTargetLocation = new Vector3(this.transform.position.x, strips[indexCurrentStrip - 1].transform.position.y,
                this.transform.position.z - deltaDistance);
            deltaMidwayDistance = this.transform.position.z - (deltaDistance / 2.0f);
        }
        else if(indexCurrentStrip == 1)//we are jumping back to the orifinal chicken standing gteen patch
        {
            //decrement the current strips
            indexCurrentStrip -= 1;

            jumpTargetLocation = new Vector3(this.transform.position.x, strips[0].transform.position.y,
                strips[0].transform.position.z);
            deltaMidwayDistance = this.transform.position.z - (deltaDistance / 2.0f);
        }
    }
    void spawnNewStrip()
    {
        //we take astrip from the pool of prefabs strips randomly
        int stripsPrefabCount = poolOfStripsPrefabs.Length;
        int randomNumber = Random.Range(0, stripsPrefabCount);

        //alternate between roads and grass strips. note there may be a continuation of grass road strips
        if(isAGrassStripToBeDisplayed)
        {
            //alternate the random selection of grass strips from  bare to not bare
            if(indexOfPreviousGrassStrip <= indexEndOfBareStrips)//previous strip was bare
            {
                randomNumber = Random.Range(indexOfGrassOccupiedStart, stripsPrefabCount - 1);//choose a strip 
            //that is not bare (7-12)
            }
            else//previous grass strip was NOT bare
            {
                randomNumber = Random.Range(0, indexEndOfBareStrips);
            }
            indexOfPreviousGrassStrip = randomNumber;//preparation for the next possible iteration
            numOfGrassStripsToDisplay -= 1;
            if(numOfGrassStripsToDisplay == 0)
            {
                isAGrassStripToBeDisplayed = false; //i.e., next strip to be displayed to be displayed will be a road strip
                numOfGrassStripsToDisplay = Random.Range(1, MAXNUMGRASS);
            }
            else
            {
                isAGrassStripToBeDisplayed = true;//display another grass strip
            }
        }
        else //otherwise drawn road strips
        {
            //alternate the random slecetion of rad strips based on the previous road strip
            //i.e. if the previous road strip was a botom road strip then the next road strip 
            //can be a top or middle depending on the numver of roads to be displayed
            //i.e. if the previous road strip was a middle road strip then the next road strip 
            //must be another middle or top depending on the number of roads to be dispalyed
            //for example, say we have 3 roads to be displayed , then the iteration must be, middle,middle, top
            //for example, say we have 3 roads to be displayed, then the iteration must be
            //bottom, middle,top 

            if (numOfRoadStripsToDisplay > 1 && wasARoadPreviouslyDisplayed)
            {
                //arriving here meas two or three roads must be drawn and the bottom road has already been displayed
                //therefore we must display the middle road so that the next road will be a top 
                //i.e. 2 roads to display or another middle road (i.e. 3 roads to display)
                randomNumber = indexOfMiddleRoad;
                indexOfPreviousRoadStrip = indexOfMiddleRoad;
            }
            else if(numOfRoadStripsToDisplay > 1 && !wasARoadPreviouslyDisplayed)
            {
                //arriving here means we have not begun the proccess of displaying multible road strips
                //therefor we must display the bottom road to begin the process
                randomNumber = indexOfBottomRoad;
                indexOfPreviousRoadStrip = indexOfBottomRoad;
                wasARoadPreviouslyDisplayed = true;//let the next iteration know a new road has been displayed
            }
            else if(numOfRoadStripsToDisplay == 1 && !wasARoadPreviouslyDisplayed)//only draw a single road
            {
                randomNumber = indexOfSingleRoad;
                indexOfPreviousRoadStrip = indexOfSingleRoad;
            }
            else if(numOfRoadStripsToDisplay == 1 && wasARoadPreviouslyDisplayed) //draw a top road
            {
                //since we are only draw a single road, and a road wa spreviouly diplsayed then\
                //the only conclusion is that the previous road was either a bottom or middle road, 
                //therefor thes next road to be drawn  must be a top road
                randomNumber = indexOfTopRoad;
                indexOfPreviousRoadStrip = indexOfTopRoad;
                wasARoadPreviouslyDisplayed = false; //last road strip was drawn
            }

            numOfRoadStripsToDisplay -= 1;
            if (numOfRoadStripsToDisplay == 0)
            {
                isAGrassStripToBeDisplayed = true;//i.e. next strip to be displayed will be a grass strip
                numOfRoadStripsToDisplay = Random.Range(1, MAXNUMROADS);
                //if(numOfRoadStripsToDisplay == 4)
               // {
               //     print("numofroadstripstodisplay equals 4");
               // }
            }
            else
            {
                isAGrassStripToBeDisplayed = false;//there are more roads to display
            }
        }//else/otherwise drawn road strips

        //create  the new random strip based on the above criteria
        GameObject item = poolOfStripsPrefabs[randomNumber] as GameObject;

        //get the last strip item so that we can find its location
        GameObject lastStrip = strips[strips.Count - 1] as GameObject;

        //instantaite a new strip to be placed at the end, but first position on top of the last strip
        GameObject newStrip = Instantiate(item, lastStrip.transform.position, lastStrip.transform.rotation);
        //right on top of last strip

        //now move the newstrip and to its new position after the last strip
        newStrip.transform.position = new Vector3(newStrip.transform.position.x,newStrip.transform.position.y,
            newStrip.transform.position.z + stripWidth);

        //add the new strip to the list
        strips.Add(newStrip);

        //update the indexOfPreviousStrip for the next iteration 
        indexOfPreviousStrip = randomNumber;

    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            Debug.Log("Collision");
            updateDeathAnimation();
        }
    }

    void updateDeathAnimation()
    {
        transform.localScale = new Vector3(1f, 0.14f,1f);
    }
}
