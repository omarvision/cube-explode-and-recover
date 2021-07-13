using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //for Text

public class CubeMover : MonoBehaviour
{
    public enum enumState
    {
        wait,
        explode,
        recover
    }

    public float power = 50.0f;
    public float radius = 5.0f;
    public float time = 2.0f;

    private float deltaPos;
    private float deltaRot;
    private GameObject CubeTarget = null;
    private GameObject Sphere = null;
    private Text Txt = null;
    private Rigidbody rb = null;
    private enumState state = enumState.wait;

    private void Start()
    {
        CubeTarget = GameObject.Find("CubeTarget");
        Sphere = GameObject.Find("Sphere");
        Txt = GameObject.Find("Txt").GetComponent<Text>();
        rb = this.GetComponent<Rigidbody>();

        if (CubeTarget == null || Sphere == null || Txt == null || rb == null)
        {
            Debug.Log("Error: one of the cube mover objects is null");
        }
    }
    private void Update()
    {
        switch (state)
        {
            case enumState.wait:
                Txt.text = state.ToString() + ": press left mouse button";
                if (Input.GetMouseButton(0) == true)
                {
                    doExplosion();
                }
                break;
            case enumState.explode:
                Txt.text = state.ToString() + ": velocity = " + rb.velocity.magnitude.ToString("0.00") + ", angularVelocity = " + rb.angularVelocity.magnitude.ToString("0.00");
                if (rb.velocity.magnitude < 0.1f)
                {
                    state = enumState.recover;
                    rb.isKinematic = true;
                    initRecover();
                }
                break;
            case enumState.recover:
                Txt.text = state.ToString() + ": position = " + this.transform.position.ToString() + ", rotation = " + this.transform.rotation.ToString();
                doRecover();
                break;
        }

    }
    private void doExplosion()
    {
        rb.isKinematic = false;
        rb.AddExplosionForce(power, Sphere.transform.position, radius);
        this.GetComponent<AudioSource>().Play();
        state = enumState.explode;
    }
    private void initRecover()
    {
        deltaPos = Vector3.Distance(this.transform.position, CubeTarget.transform.position) / time;
        deltaRot = Quaternion.Angle(this.transform.rotation, CubeTarget.transform.rotation) / time;
    }
    private void doRecover()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, CubeTarget.transform.position, deltaPos * Time.deltaTime);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, CubeTarget.transform.rotation, deltaRot * Time.deltaTime);

        if (Vector3.Distance(this.transform.position, CubeTarget.transform.position) < 0.01f)
        {
            state = enumState.wait;
        }
    }
}