using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{
        
        public Transform target;
        public Vector3 offset;

		// Use this for initialization
		void Start() { }

		// Update is called once per frame
		void LateUpdate()
		{
			// Early out if we don't have a target
			if (!target)
				return;
            //new Vector3(Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime), Mathf.Lerp(transform.position.y, target.position.y, Time.deltaTime), -10);
            transform.position = Vector3.MoveTowards(
                new Vector3(transform.position.x, transform.position.y, -10), //a point
                new Vector3(target.position.x, target.position.y, -10), //b point
                Time.deltaTime * Mathf.Abs(transform.position.magnitude-target.position.magnitude)); //speed
			// Always look at the target
			transform.LookAt(target);
		}
	}
}