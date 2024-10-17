using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemeMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public int nextMove;

    SpriteRenderer flip;

    CapsuleCollider2D capsuleCollider;
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        flip = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think",5);
    }

   /*  void Update() {
        if()
    } */

    void FixedUpdate() {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check 
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f,rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if(rayHit.collider == null) {
                Turn();
            }
    }

    //재귀함수
    void Think() {

        //Set Next Active
        nextMove = Random.Range(-1, 2);

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
        flip.flipX = nextMove == 1;

        //Recursive(재귀함수)
         float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think",nextThinkTime);
    }
    //
    void Turn() {
        nextMove = nextMove * -1;
        flip.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think",2);
    }

    public void OnDamaged() {
        //Sprite Alpha
        flip.color = new Color(1,1,1, 0.4f);
        //Sprite Flip Y
        flip.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 5);
    }

    void DeActive() {
        gameObject.SetActive(false);
    }
}
