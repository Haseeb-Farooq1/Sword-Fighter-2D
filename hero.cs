using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class hero : MonoBehaviour
{
    public Text coinText;
    public int coincount = 0;
    public int maxhealth = 100;
    public Text health;
    public Animator animator;
    public float jumpHeight = 5f;
    public bool isGround = true;     
    private float movement;
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public Transform attackpoint;
    public float attackradius = 1f;
    public LayerMask attackLayer;
    public AudioSource coinsound;
    public AudioSource losingsound;

    public float gameOverY = -6f;  

    void Update()
    {
        if (maxhealth <= 0)
        {
            die();
        }

        if (transform.position.y < gameOverY) 
        {
            die(); 
        }

        coinText.text = coincount.ToString();
        health.text = maxhealth.ToString();

        movement = Input.GetAxis("Horizontal");

        if (movement > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); 
        }
        else if (movement < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
            isGround = false;
            animator.SetBool("jump", true);
        }

        animator.SetFloat("Run", Mathf.Abs(movement));

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("attack");
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movement * moveSpeed, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGround = true;
            animator.SetBool("jump", false);
        }
    }

    public void takedamage(int damage)
    {
        if (maxhealth <= 0) return;  

        maxhealth -= damage;
        Debug.Log("Player took damage! maxhealth: " + maxhealth);

        if (maxhealth <= 0)
        {
            die();
        }
    }

    public void attack()
    {
        Collider2D colInfo = Physics2D.OverlapCircle(attackpoint.position, attackradius, attackLayer);
        if (colInfo != null)
        { 

            Enemy player = colInfo.gameObject.GetComponent<Enemy>();
            if (player != null)
            {
                player.takedamage(10);  
            }
            else
            {
                Debug.Log("Attack hit something else, but it's not the player!");
            }
        }
        else
        {
            Debug.Log("Attack missed!");
        }
        Debug.Log("Attack Layer Mask: " + attackLayer.value);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "coin")
        {
            coincount++;
            coinsound.Play();
            other.gameObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("collected");
            Destroy(other.gameObject, 1f);
        }
        if (other.gameObject.tag == "goal")
        {
            SceneManager.LoadScene("win");
        }
    }

    void die()
    {
        // Play losing sound before handling game over logic
        if (losingsound != null)
        {
            losingsound.Play();  // Play the losing sound when the player dies
        }
        else
        {
            Debug.LogWarning("Losing sound is not assigned!");
        }

        // Game Over logic
        FindObjectOfType<GameManager>().isGameActive = false;
        Destroy(this.gameObject);  // Destroy player object
        FindObjectOfType<GameManager>().GameOver();  // Call Game Over function
    }
}
