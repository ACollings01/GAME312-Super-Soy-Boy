﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save Level"))
        {
            Level level = (Level)target;
            level.transform.position = Vector3.zero;
            level.transform.rotation = Quaternion.identity;

            var levelRoot = GameObject.Find("Level");

			var ldr = new LevelDataRepresentation();
			var levelItems = new List<LevelItemRepresentation>();

			foreach (Transform t in levelRoot.transform)
			{
				var hm = t.GetComponent<HazardMovement>();
				var sr = t.GetComponent<SpriteRenderer>();
				var li = new LevelItemRepresentation()
				{
					position = t.position,
					rotation = t.rotation.eulerAngles,
					scale = t.localScale
				};

				if (t.name.Contains(" "))
				{
					li.prefabName = t.name.Substring(0, t.name.IndexOf(""));
				}
				else
				{
					li.prefabName = t.name;
				}

				if (sr != null)
				{
					li.spriteLayer = sr.sortingLayerName;
					li.spriteColour = sr.color;
					li.spriteOrder = sr.sortingOrder;
				}

				if (hm != null)
				{
					li.xDistance = hm.xDistance;
					li.xSpeed = hm.xSpeed;
					li.yDistance = hm.yDistance;
					li.ySpeed = hm.ySpeed;
				}

				levelItems.Add(li);
			}

			ldr.levelItems = levelItems.ToArray();
			ldr.playerStartPosition = GameObject.Find("SoyBoy").transform.position;

			var currentCamSettings = FindObjectOfType<CameraLerpToTransform>();

			if (currentCamSettings != null)
			{
				ldr.cameraSettings = new CameraSettingsRepresentation()
				{
					cameraTrackTarget = currentCamSettings.camTarget.name,
					cameraZDepth = currentCamSettings.cameraZDepth,
					minX = currentCamSettings.minX,
					maxX = currentCamSettings.maxX,
					minY = currentCamSettings.minY,
					maxY = currentCamSettings.maxY,
					trackingSpeed = currentCamSettings.trackingSpeed
				};
			}

			var levelDataToJson = JsonUtility.ToJson(ldr);
			var savePath = System.IO.Path.Combine(Application.dataPath, level.levelName + ".json");
			System.IO.File.WriteAllText(savePath, levelDataToJson);
			Debug.Log("Level saved to " + savePath);

        }
    }
}
