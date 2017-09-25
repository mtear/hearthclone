using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragDrop : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	bool dragging = false;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			int mask = LayerMask.GetMask ("PlayerHand");
			if (Physics.Raycast (ray, out hit, 150.0f, mask)) {
				dragging = true;
			}
		} else if(dragging) {
			if (Input.GetMouseButtonUp (0)) {
				dragging = false;
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				int mask = LayerMask.GetMask ("PlayZone");
				if (Physics.Raycast (ray, out hit, 150.0f, mask)) {
					Debug.Log ("Playzone");
					Destroy (gameObject);
					GameObject zone = GameObject.Find ("creatureportion_0");
					GameObject prefab = Instantiate (Resources.Load ("oval") as GameObject);
					prefab.transform.parent = zone.transform;
				}
			}
			else if (Input.GetMouseButton (0)) {
				Vector3 v = transform.position;
				Vector3 m = Input.mousePosition;
				m.z = 100;
				m = Camera.main.ScreenToWorldPoint (m);
				//v.x = m.x; v.y = m.y;
				transform.position = m;
			}
		}
	}
}
