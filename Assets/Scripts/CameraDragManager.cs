using UnityEngine;
using System.Collections;

public class CameraDragManager : MonoBehaviour {

	private Vector3 origin;
	private Vector3 difference;
	private bool isDragging=false;
	

	void LateUpdate () {
		Camera cam = Camera.allCameras[0];
		if (Input.GetMouseButton (2)) {
			difference = (cam.ScreenToWorldPoint (Input.mousePosition)) - cam.transform.position;
			if (!isDragging){
				isDragging = true;
				origin = cam.ScreenToWorldPoint (Input.mousePosition);
			}
		} else {
			isDragging = false;
		}
		if (isDragging){
			cam.transform.position = origin-difference;
		}
	}
}