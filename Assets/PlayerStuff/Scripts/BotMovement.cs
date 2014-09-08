﻿using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {
	
	
	
	public float moveSpeed = 10.0f;
	public float jumpSpeed = 5.0f;
	public float jumpDelay = 0.5f;
	public float drag = 9.81f;
	public float friction = 100f;
	//public float airControl = 0.5f;
	
	Vector3 direction = Vector3.zero;  //forward/back left/right
	Vector3 hitDirectionXZ = Vector3.zero;
	Vector3 horizontalHitVelocity =  Vector3.zero;
	float verticalVelocity = 0;
	float verticalHitVelocity = 0;
	
	
	bool canJumpAir = true;
	bool falling = true; //falling off a ldge without jumping 
	//	float jumpTime = 0.0f;
	float airTime = 0.0f;
	
	CharacterController cc;
	Animator anim;
	
	
	// Use this for initialization
	void Start () {
		cc=GetComponent<CharacterController>();
		anim=GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
//		direction =  ( transform.rotation * new Vector3 ( Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical") ));
		
		if( direction.magnitude > 1){
			direction = direction.normalized; 
		}
		
		anim.SetFloat ("Speed", direction.magnitude);
		
		if ( cc.isGrounded ) {
			airTime=0;
			canJumpAir=true;
			falling=true;
			
			if (horizontalHitVelocity.magnitude > 4) {
				horizontalHitVelocity -= horizontalHitVelocity.normalized * friction * Time.deltaTime;
			}else {
				horizontalHitVelocity = Vector3.zero;
			}
			
//			if ( Input.GetButtonDown ("Jump") ) {
//				verticalVelocity = jumpSpeed;
//				falling=false;
//			}
//			else{
				verticalVelocity += Physics.gravity.y * Time.deltaTime;
//			}
		}
		else{
			airTime += Time.deltaTime;
			
//			if ( Input.GetButtonDown ("Jump") && canJumpAir && (falling || airTime > jumpDelay)) {
//				canJumpAir=false;
//				verticalVelocity = jumpSpeed;
//			}
			
			if (horizontalHitVelocity.magnitude > 4) {
				horizontalHitVelocity -= horizontalHitVelocity.normalized * drag * Time.deltaTime;
			}else {
				horizontalHitVelocity = Vector3.zero;
			}
		}
		
		
		
	}
	
	// FixedUpdate is called once per physics loop
	// Do all movement and physics here
	void FixedUpdate(){
		
		Vector3 walkDist = direction * moveSpeed  * Time.deltaTime;
		Vector3 hitDist = horizontalHitVelocity * Time.deltaTime; 
		if (horizontalHitVelocity.magnitude > 0) {
			Debug.Log (horizontalHitVelocity);
		}
		Vector3 dist = walkDist + hitDist; 
		
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
		
		if (verticalVelocity < 0) {
			anim.SetBool ("Falling", true);
			
			if ( cc.isGrounded) {
				anim.SetBool("Grounded", true);
				anim.SetBool ("Jump", false);
				verticalVelocity = Physics.gravity.y * Time.deltaTime;
			} else {
				anim.SetBool("Grounded", false);
				
			}
		} else {
			if ( verticalVelocity > jumpSpeed*0.5f ){
				anim.SetBool ("Jump", true);
			}
			anim.SetBool ("Falling", false);
		}
		
		//		if ( cc.isGrounded && verticalVelocity < 0 ) {
		//			anim.SetBool("Grounded", true);
		//			anim.SetBool ("Jump", false);
		//			verticalVelocity = Physics.gravity.y * Time.deltaTime;
		//		} else {
		//
		//			if ( verticalVelocity > jumpSpeed*0.75f ){
		//				anim.SetBool ("Jump", true);
		//			}
		//			else if  ( verticalVelocity < 0 ){
		//				anim.SetBool ("Falling", true);
		//			}
		//				
		//		}
		//		Debug.Log (verticalVelocity);
		//		Debug.Log (cc.isGrounded);
		
		dist.y = verticalVelocity * Time.deltaTime;
		
		cc.Move ( dist );
		
	}
	
	
	
	[RPC]
	public void Hit(Vector3 hitDir, float hitForce){
		//Vector3 hitDir = hitDir.normalized;
		hitForce= hitForce * ( 1 + GetComponent<Health>().damagePercent/100);
		Vector3 hitVelocity = hitForce * hitDir.normalized;
		//Vector3 hitVelocity = transform.rotation * hitVelocityModifier;// new Vector3 (hitVelocityModifier.x,hitVelocityModifier.y,hitVelocityModifier.z ) ;
		verticalHitVelocity = hitVelocity.y;
		horizontalHitVelocity = new Vector3(hitVelocity.x, 0 , hitVelocity.z); 
		//
		verticalVelocity = verticalHitVelocity;
		//		verticalVelocity += hitDirection.y * hitForce  * Time.deltaTime;
		//		hitDirectionXZ = new Vector3 (hitDirection.x, 0, hitDirection.z); 
		//		hitSpeedXZ = hitForce;
	}
}
