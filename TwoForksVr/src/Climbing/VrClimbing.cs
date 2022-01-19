using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Climbing
{
    public class VrClimbing: MonoBehaviour
    {
		private Transform playerTransform;
		private CharacterController characterController;
		private VrClimbing otherHand;
		private Vector3? grabPosition;
	    private Transform attatchedTransform;
	    private SteamVR_Input_Sources inputSource;

	    public static VrClimbing Create(VrHand hand)
	    {
		    return hand.gameObject.AddComponent<VrClimbing>();
	    }

	    public void SetUp(Transform player, VrClimbing otherClimbingController)
	    {
		    otherHand = otherClimbingController;
		    playerTransform = player;
		    characterController = player ? player.GetComponent<CharacterController>() : null;
		    if (!characterController)
		    {
			    Logs.LogError("no character controller, whaaaaaaaaaaaaaaaaaaat?");
		    }
	    }
	    
		private void Awake ()
		{
			inputSource = GetComponent<SteamVR_Behaviour_Pose>().inputSource;
		}
		
		private void Update ()
		{
	        UpdateCollider();
	        UpdatePlayerPosition();
		}
		
		private void UpdatePlayerPosition()
	    {
	        if (grabPosition == null || attatchedTransform == null || playerTransform == null)
	        {
	            return;
	        }

	        playerTransform.position += attatchedTransform.TransformPoint((Vector3) (grabPosition)) - transform.position;
	    }
		
	    private void OnTriggerStay(Collider otherCollider)
	    {
	        if (otherCollider.isTrigger) return;


	        if (grabPosition != null || !IsPressingClimb() || IsOtherHandGrabbing() || !IsRockClimb(otherCollider)) return;

	        Logs.LogInfo($"########### holding on to dear life to ${otherCollider.name}");
	        
	        attatchedTransform = otherCollider.transform;
	        grabPosition = attatchedTransform.InverseTransformPoint(transform.position);
	        if (characterController)
	        {
		        characterController.enabled = false;
	        }
	    }

	    private bool IsRockClimb(Component otherCollider)
	    {
		    return otherCollider.transform.parent && otherCollider.transform.parent.GetComponentInChildren<vgRockClimbEdge>();
	    }

	    private void UpdateCollider()
	    {
	        if (!IsPressingClimb())
	        {
		        Logs.LogInfo("Release");
	            Release();
	        }
	    }

	    private bool IsOtherHandGrabbing()
	    {
		    return otherHand && otherHand.grabPosition != null;
	    }
	    
	    private void Release()
	    {
		    grabPosition = null;
		    if (characterController && !IsOtherHandGrabbing())
		    {
				characterController.enabled = true;
		    }
	    }

	    private bool IsPressingClimb()
	    {
		    return SteamVR_Actions.default_Grip.GetState(inputSource);
	    }
    }
}