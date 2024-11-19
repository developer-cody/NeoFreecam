using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkibidiFreecam
{
    public class CamMovement : MonoBehaviour
    {
        private const float horizontalMultiplier = 6f, verticalMultiplier = 4.5f;

        public float speed = 0.1f;


        public void Update()
        {
           

            Vector3 movefwd = Plugin.Intense.FlyCamera.transform.forward * verticalMultiplier / 30;
            Vector3 moveright = Plugin.Intense.FlyCamera.transform.right * horizontalMultiplier / 30;
            Vector3 moveleft = -Plugin.Intense.FlyCamera.transform.right * horizontalMultiplier / 30;
            Vector3 moveback = -Plugin.Intense.FlyCamera.transform.forward * verticalMultiplier / 30;
            if (Keyboard.current.wKey.isPressed)
            {
                Plugin.Intense.FlyCamera.transform.position += movefwd;
            }
           
            if (Keyboard.current.dKey.isPressed)
            {
                Plugin.Intense.FlyCamera.transform.position += moveright;
            }
            
            if (Keyboard.current.aKey.isPressed)
            {
                Plugin.Intense.FlyCamera.transform.position += moveleft;
            }
            
            if (Keyboard.current.sKey.isPressed)
            {
                Plugin.Intense.FlyCamera.transform.position += moveback;
            }

            if (Mouse.current.rightButton.isPressed)
            {
                Plugin.Intense.FlyCamera.transform.eulerAngles += speed * new Vector3(-Mouse.current.delta.y.ReadValue(), Mouse.current.delta.x.ReadValue(), 0);

                //credit to https://stackoverflow.com/questions/64327135/how-to-convert-input-getaxismouse-x-or-input-getaxismouse-y-to-the-new-i and https://www.youtube.com/watch?v=FIiKuP-9KuY
            }


        }
    }
}
