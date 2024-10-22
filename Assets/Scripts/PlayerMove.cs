using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    private int jumpCount = 0; // 현재 점프 횟수
    private int maxJumpCount = 1; // 최대 점프
    public float maxSpeed;
    public float jumpPower;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public AudioClip audioBgm;
    Rigidbody2D rigid;

    SpriteRenderer spriteRenderer;

    Animator anim;

    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;

    void Awake() 
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action) {
        switch (action) {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;  
            case "BGM":
                audioSource.clip = audioBgm;
                break; 
        }
    } 

    void Start() {
        PlaySound("BGM");
        audioSource.Play();
    }

    void Update() 
    {
        //Jump (단발적인 이동과 같은 경우에는 업데이트에 쓰는게 좋다)
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount) { //점프를 한번만 하게 함 max카운트 늘리면 더블점프
            rigid.velocity = new Vector2(rigid.velocity.x,0);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
            audioSource.Play();   
        }
        

        //Stop Speed 
        if(Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.00000001f, rigid.velocity.y); // 벡터값의 소수점을 작게 줘서 얼음판처럼 미끄러지는걸 방지
        }

        //Direction Sprite 
        if(Input.GetButton("Horizontal"))
        spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; 

        //Animation
        if(Mathf.Abs(rigid.velocity.x) < 0.3) 
            anim.SetBool("isWalking",false);
        else 
            anim.SetBool("isWalking",true);
    }
    void FixedUpdate()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed 
        if(rigid.velocity.x > maxSpeed) //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if(rigid.velocity.x < maxSpeed*(-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //Landing Platform
        if(rigid.velocity.y < 0) {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if(rayHit.collider != null) {
                if(rayHit.distance < 0.5f)
                anim.SetBool("isJumping", false);
                jumpCount = 0;
            }
        }
    }
    //충돌이벤트
    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Enemy") {
            //Attack 
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
                // Sound 
                PlaySound("ATTACK");
                audioSource.Play();   
            }
            else //Damaged
            OnDamaged(collision.transform.position);
            // Sound 
            PlaySound("DAMAGED");
            audioSource.Play();   
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Item") {
            // Point 
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if(isSilver)
                gameManager.stagePoint += 100;
            else if(isGold)
                gameManager.stagePoint += 300; 

            // Deactive Item
            collision.gameObject.SetActive(false);

            // Sound 
            PlaySound("ITEM");
            audioSource.Play();   
        }
        else if (collision.gameObject.tag == "Finish") {
            // Next Stage 
            gameManager.NextStage();
            // Sound 
            PlaySound("FINISH");
            audioSource.Play();   
        }
    }
    void OnAttack(Transform enemy) {
        // Point
        gameManager.stagePoint += 100;

        // Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        // Enemy Die
        EnemeMove enemyMove = enemy.GetComponent<EnemeMove>();
        enemyMove.OnDamaged();
    }


    //충돌시 튕겨나감+무적 함수 
    void OnDamaged(Vector2 targetPos) {

        // Health Down 
        gameManager.HealthDown();
        // Change Layer (Immortal Active)
        gameObject.layer = 9;

        // View Alpha
        spriteRenderer.color = new Color(1,1,1,0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc,1)*7,ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 1); // 무적시간 조절

    }
    //충돌후 무적에서 돌아오기
    void OffDamaged() {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1,1,1,1);
    }

    public void OnDie() {
        //Sprite Alpha
        spriteRenderer.color = new Color(1,1,1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        // Sound 
        PlaySound("DIE");
        audioSource.Play();   
    }

    public void VelocityZero() {
        rigid.velocity = Vector2.zero;
    }
}
