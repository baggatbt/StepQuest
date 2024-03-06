using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pixel_Backgrounds
{
    public class CameraController : MonoBehaviour
    {
        public float camSpeed = 5f;
        public float minX = 0f;
        public float maxX = 15f;
        public Toggle Automove;

        private float camPos;


        void Start()
        {
            camPos = transform.position.x;
            Automove.isOn = false;
        }
        void Update()
        {
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && transform.position.x < maxX)
            {
                transform.Translate(new Vector3(camSpeed * Time.deltaTime, 0, 0));
                Automove.isOn = false;
            }
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && transform.position.x > minX)
            {
                transform.Translate(new Vector3(-camSpeed * Time.deltaTime, 0, 0));
                Automove.isOn = false;
            }
            if (Automove.isOn && transform.position.x < maxX)
            {
                transform.Translate(new Vector3(camSpeed * Time.deltaTime, 0, 0));
            }
        }
        public void AutomoverCheck()
        {
            if (Automove.isOn == false)
            {
                Automove.isOn = true;
            }
            else
            {
                Automove.isOn = false;
            }
        }
    }
}

