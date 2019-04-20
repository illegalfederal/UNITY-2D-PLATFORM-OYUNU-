using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{


    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float pointRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGround;
    private bool jump;
  

    [SerializeField]
    private float movementSpeed; //Hareket hızı için değişken yaptık

    [SerializeField]
    private float jumpForce;


    private bool isAttack;
    private bool isSlide;

    private Rigidbody2D myRigidBody2D; //Kodun bağlı olduğu objenin RigidBody'sine değişken yaptık
 
    private Animator myAnimator; //Kodun bağlı olduğu objenin Animator'una değişken yaptık


    private bool facingRight; //Karakterin sağa bakıp bakmadığına göre true yada false alır.

    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>(); //myRigidBody2D değişkenini Editörde bulunan Rigidbody'ye referans yaptık.
        //Yeni
        myAnimator = GetComponent<Animator>(); //myAnimator değişkenini Editörde bulunan Animator'e referans yaptık.
        facingRight = true;  //Oyun başladığında karakter sağa baktığı için true değeri veriyoruz.
    }


    private void Update()
    {
        HandleInputs();
    }

    void FixedUpdate()
    {

        float horizontal = Input.GetAxis("Horizontal");  //Horizontal sistemi için değişken yaptık.horizontal değişkeni a,d ve yön tuşlarına göre -1 vey 1 değeri alacak.

        isGround = isGrounded();
      
        HandleMovement(horizontal);
        Flip(horizontal);
        HandleAttack();
        ResetValues();
    }


    private void HandleMovement(float horizontal)
    {
        if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
            myRigidBody2D.velocity = new Vector2(horizontal * movementSpeed, myRigidBody2D.velocity.y);
        }

        //Zıplama hareketi
        if (isGround && jump)
        {
            isGround = false;
            myRigidBody2D.AddForce(new Vector2(0, jumpForce));
        }
    

        //Kayma hareketi başla
        if (isSlide && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Slide")) {
            myAnimator.SetBool("slide", true);
        }
        else if (!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Slide")) {
            myAnimator.SetBool("slide", false);
        }
        //Kayma hareketi bitiş
        myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void HandleAttack() {  //Saldırı Kontrol
        if (isAttack && !this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) {
            myAnimator.SetTrigger("attack");
            myRigidBody2D.velocity = Vector2.zero;
        }
    }

    private void HandleInputs() {  //Kalvye Girişleri
        if (Input.GetKeyDown(KeyCode.X))
        {
            isAttack = true;
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            isSlide = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void ResetValues() {
        isAttack = false;
        isSlide = false;
        jump = false;
    }

    private void Flip(float horizontal)
    { //Horizontal'a bağlı olarak karakterin yönünü değiştirir. 

        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 theScale = transform.localScale; //Karakter scale'ine referans yaptık.
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    //Karakter yerde mi değil mi?
    private bool isGrounded()
    {
        if (myRigidBody2D.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position,pointRadius, whatIsGround);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != this.gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}