using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Gamekit2D
{
    public class AutoCameraSetup : MonoBehaviour
    {
        public bool autoSetupCameraFollow = true;
        public string cameraFollowGameObjectName = "Ellen";

        void Awake ()
        {
            if(!autoSetupCameraFollow)
                return;

            CinemachineCamera cam = GetComponent<CinemachineCamera> ();
        
            if(cam == null)
                throw new UnityException("Cinemachine Camera was not found, default follow cannot be assigned.");

            // Search only inside this scene so preview objects are ignored. The recursive
            // lookup also supports levels that organise the player below hierarchy groups.
            var rootObj = gameObject.scene.GetRootGameObjects();
            GameObject cameraFollowGameObject = null;
            foreach (var go in rootObj)
            {
                if (go.name == cameraFollowGameObjectName)
                    cameraFollowGameObject = go;
                else
                {
                    var transforms = go.GetComponentsInChildren<Transform>(true);
                    foreach (var child in transforms)
                    {
                        if (child.name != cameraFollowGameObjectName)
                            continue;

                        cameraFollowGameObject = child.gameObject;
                        break;
                    }
                }

                if (cameraFollowGameObject != null) break;
            }
        
            if(cameraFollowGameObject == null)
                throw new UnityException("GameObject called " + cameraFollowGameObjectName + " was not found, default follow cannot be assigned.");

            if (cam.Follow == null)
            {
                cam.Follow = cameraFollowGameObject.transform;
            }
        }
    }
}
