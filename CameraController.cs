using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	/*
	 * Camera movement along the xy-plane &
	 * Camera rotation around a point in space on the boundaries of a sphere.
	 * For this script to work you'll need a gameobject besides the camera.
	 * Add the script to the gameobject and make your main camera the child of this gameobject.
	*/
	public float startRadius;
	public float zoomSpeed;
	public float minZoom;
	public float tiltSpeed;
	public float startTheta;
	public float maxTheta;
	public float scrollSpeed;
	public float rotationSpeed;
	public float rotationStepSize; // set this to a small value to rotate freely 
		// camera rotates slower for lower values, if rotationSpeed * time.deltaTime > rotationStepSize
	public float offset;

	public Camera camera;


	private int rotationDirection; // 0 if no rotation, -1 if clockwise, 1 if counter-clockwise (for rotations around z-axis)
	
	private float radius; // is changed by mouse-scrolling
	private float startingPhi;
	private float phi; // angle around the z-axis
	private float targetPhi; // target angle around the z-axis
	private float theta; // angle around the x-axis
	private float targetTheta; // target angle around the x-axis
	
	void Start()
	{
		SetValuesToDefault ();
	}
	
	private void SetValuesToDefault()
	{
		radius = startRadius;
		phi = offset;
		targetPhi = offset;
		theta = startTheta;
		
		SetCameraPositionAndRotation();
	}
	
	private void SetCameraPositionAndRotation()
	{
		Vector3 newPosition = new Vector3 (0f, 0f, 0f);
		
		newPosition.x = radius * Mathf.Cos (phi * Mathf.PI / 180) * Mathf.Sin (theta * Mathf.PI / 180);
		newPosition.z = radius * Mathf.Sin (phi* Mathf.PI / 180) * Mathf.Sin (theta * Mathf.PI / 180);
		newPosition.y = radius * Mathf.Cos (theta * Mathf.PI / 180);

		Quaternion newRotation = Quaternion.Euler (90 - theta, 270 - phi, 0);
		
		camera.transform.localPosition = newPosition;
		camera.transform.rotation = newRotation;
	}
	
	private void ComputeNewPhi()
	{
		int sign = rotationDirection;
		float phiDelta = sign * rotationSpeed * Time.deltaTime;
		
		if (Mathf.Abs (phiDelta) > rotationStepSize && Mathf.Abs (phiDelta) < 180)
		{
			phi = targetPhi;
		}
		else
		{
			phi = (phi + phiDelta + 360) % 360;
		}
	}
	
	private void CheckForRotateInput()
	{
		int spin = (int) Input.GetAxis ( "RotateCamera" );

		if (Mathf.Abs (spin) > float.Epsilon) 
		{
			targetPhi = (targetPhi + spin * rotationStepSize + 360) % 360;
			rotationDirection = spin;
		}
		else
		{
			phi = targetPhi;
			rotationDirection = 0;
		}
	}
	
	private void XZPlaneMovementInput()
	{
		Vector3 direction = new Vector3 (0f, 0f, 0f);
		Vector3 newPosition = new Vector3 (0f, 0f, 0f);
		Vector3 oldPosition = transform.position;
		Vector3 parentRotation = transform.rotation.eulerAngles;
		
		newPosition.y = oldPosition.y;

		float vMove = Input.GetAxis ("Vertical");
		float hMove = Input.GetAxis ("Horizontal");

		if (Mathf.Abs (hMove) > Mathf.Epsilon || Mathf.Abs (vMove) > Mathf.Epsilon) 
		{
			float norm = Mathf.Sqrt(Mathf.Pow (vMove, 2) + Mathf.Pow (hMove, 2));

			direction.x = (-vMove * Mathf.Cos (phi * Mathf.PI / 180) - hMove * Mathf.Sin (phi * Mathf.PI / 180))/norm;
			direction.z = (-vMove * Mathf.Sin (phi * Mathf.PI / 180) + hMove * Mathf.Cos (phi * Mathf.PI / 180))/norm;
		}

		newPosition.x = oldPosition.x + direction.x * scrollSpeed * Time.deltaTime;
		newPosition.z = oldPosition.z + direction.z * scrollSpeed * Time.deltaTime;
		
		transform.position = newPosition;
	}
	
	private void ZoomCamera()
	{

		if (Input.GetAxis ("Mouse ScrollWheel") != 0) 
		{
			float deltaRadius = Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

			if (deltaRadius <= radius - minZoom || deltaRadius > 0)
			{
				radius = radius + deltaRadius;
			}
		}
		else if (Input.GetAxis ("ZoomCamera") != 0) 
		{
			float deltaRadius = Input.GetAxis ("ZoomCamera") * zoomSpeed * Time.deltaTime;

			if (deltaRadius <= radius - minZoom || deltaRadius > 0)
			{
				radius = radius + deltaRadius;
			}
		}
	}

	private void TiltCamera()
	{
		if (Input.GetAxis ("TiltCamera") != 0) 
		{
			float deltaTheta = Input.GetAxis ("TiltCamera") * tiltSpeed * Time.deltaTime;

			if ((deltaTheta + theta < maxTheta || deltaTheta < 0) && (theta > 0 || deltaTheta > 0))
			{
				theta = theta + deltaTheta;

				if (theta > maxTheta)
				{
					theta = maxTheta;
				}
				else if (theta < 0)
				{
					theta = 0;
				}
			}
		}
	}
	
	void Update()
	{
		XZPlaneMovementInput ();
		ZoomCamera ();
		TiltCamera ();

		if (rotationDirection == 0) 
		{
			CheckForRotateInput();
		}
		
		if (rotationDirection == -1) 
		{
			ComputeNewPhi();

			// border case checks
			bool firstCheck = (targetPhi < rotationStepSize && (phi < targetPhi || phi > 360 - rotationStepSize));
			bool secondCheck = (targetPhi > 360 - rotationStepSize && phi > 360 - rotationStepSize && phi < targetPhi);
			bool thirdCheck = (phi <= targetPhi && targetPhi <= 360 - rotationStepSize);
			
			if (firstCheck || secondCheck || thirdCheck) 
			{
				CheckForRotateInput();
			}
		} 
		
		else if (rotationDirection == 1) 
		{
			ComputeNewPhi();

			bool firstCheck = (targetPhi > 360 && (phi > targetPhi || phi < rotationStepSize));
			bool secondCheck = (targetPhi < rotationStepSize && phi < rotationStepSize && phi > targetPhi);
			bool thirdCheck = (phi >= targetPhi && targetPhi >= rotationStepSize);
			
			if (firstCheck || secondCheck || thirdCheck) 
			{
				CheckForRotateInput();
			}
		}
		SetCameraPositionAndRotation();
	}
}
