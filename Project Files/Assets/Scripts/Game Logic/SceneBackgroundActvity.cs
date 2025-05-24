using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SceneBackgroundActvity : MonoBehaviour
{
    public GameObject sceneCamera;
    public GameObject sceneLight;
    public GameObject[] floatingPlayers;          //array of game objects to store all 10 floating players

    private float sceneRotation;                  //stores te current y-rotation value of the light and camera
    
    private float[] xRotation = new float[10];    //float array to x-rotation of all 10 floating players respectively
    private float[] yRotation = new float[10];    //float array to y-rotation of all 10 floating players respectively
    private float[] zRotation = new float[10];    //float array to z-rotation of all 10 floating players respectively
    
    private Vector3[] axis = new Vector3[10];    //A vector 3 that created to decide which axis a player should be rotated about, this axis is decided randomly.
    
    private float[] rotations = new float[10];    //float array which stores 10 random values which are updated to the rotation of all 10 floating players respectively

    private void Start()
    {
        sceneRotation = Random.Range(0.0f, 360.0f);    //setting a random camera and light angle at the starting of scene
        
        for (int i = 0; i < 10; i++)
        {
            //if x-axis needs to be rotated, we update x value of Vector 3. We do the same thing with y and z.
            int index = Random.Range(0, 3);
            if (index == 0)
                axis[i].x = 1.0f;
            if (index == 1)
                axis[i].y = 1.0f;
            if (index == 2)
                axis[i].z = 1.0f;
            
            rotations[i] = Random.Range(-1.0f, 1.0f);    //assigning random update values for 10 floating players
        }
    }

    // Update is called once per frame
    private void Update()
    {
        sceneRotation += 0.15f * Time.deltaTime;                                //updating value for camera and light rotation
        
        sceneCamera.transform.localRotation = quaternion.Euler(0.0f, sceneRotation, 0.0f);   //rotation the scene camera
        sceneLight.transform.localRotation = quaternion.Euler(0.0f, sceneRotation, 0.0f);    //rotating the scene light

        for (int i = 0; i < 10; i++)
        {
            //updating rotation of 10 players along randomly chosen axes respectively 
            if (axis[i].x == 1.0f)
            {
                xRotation[i] += rotations[i] * Time.deltaTime;
                floatingPlayers[i].transform.localRotation = quaternion.Euler(xRotation[i], yRotation[i], zRotation[i]);
            }
            if (axis[i].y == 1.0f)
            {
                yRotation[i] += rotations[i] * Time.deltaTime;
                floatingPlayers[i].transform.localRotation = quaternion.Euler(xRotation[i], yRotation[i], zRotation[i]);
            }
            if (axis[i].z == 1.0f)
            {
                zRotation[i] += rotations[i] * Time.deltaTime;
                floatingPlayers[i].transform.localRotation = quaternion.Euler(xRotation[i], yRotation[i], zRotation[i]);
            }
        }
    }
}