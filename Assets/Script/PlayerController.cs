using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D coll;
    public float speed;
    public float jumpforce=5000f;
    public Animator animat;
    public LayerMask ground;
    public Collider2D colliderrelay;
    private Rigidbody2D rigidbodyrelay;
    public bool hanging = false;
    private Transform transformrelay;
    public Transform player;
    private float relaydirection = 2f, movex = 1.6f, movey = 1.6f;
    // Update is called once per frame
    void Update()
    {
        Movement();
        SwitchAnim();
        RelayControl();
    }
    void Movement()
    {
        float horizontalmove = Input.GetAxis("Horizontal1");
        float facedircetion = -Input.GetAxisRaw("Horizontal1");
        //人物移动
        if (horizontalmove != 0)
        {
            rb.velocity = new Vector2(horizontalmove * speed, rb.velocity.y);
            animat.SetFloat("running", Mathf.Abs(facedircetion));
        }
        if(facedircetion != 0)
        {
            transform.localScale = new Vector3(facedircetion, 1, 1);
            relaydirection = facedircetion;
        }
        //人物跳跃
        float verticaly = Input.GetAxisRaw("Vertical1");
        if (verticaly != 0)
        {
            transform.Translate(Vector3.up * verticaly * speed * Time.deltaTime, Space.World);
            animat.SetBool("jumping", true);
        }
    }
    void SwitchAnim()
    {
        if (animat.GetBool("jumping"))
        {
            if(rb.velocity.y <= 0.0f)
            {
                animat.SetBool("jumping", false);
                animat.SetBool("falling", true);
            }
        }else if (coll.IsTouchingLayers(ground))
        {
            animat.SetBool("falling", false);
            animat.SetBool("Idle", true);
        }
    }
    private void RelayControl()
    {
        bool isdown = Input.GetKey(KeyCode.S);
        if(isdown && hanging)
        {
            colliderrelay.isTrigger = false;
            rigidbodyrelay.gravityScale = 5;
            hanging = false;
        }
        else if(isdown && Checkrelay(5f, 1.2f))
        {
            rigidbodyrelay = colliderrelay.gameObject.GetComponent<Rigidbody2D>();
            rigidbodyrelay.gravityScale = 0;
            transformrelay = colliderrelay.gameObject.GetComponent<Transform>();
            colliderrelay.isTrigger = true;
            hanging = true;
        }
        if (hanging) RelayFollow();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "trap")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if(collision.collider.tag == "pass")
        {

        }
    }
    void SwitchRelay()
    {
        if(colliderrelay.isTrigger == true)
        {
            colliderrelay.isTrigger = false;
            rigidbodyrelay.gravityScale = 5.0f;
        }
        else
        {
            colliderrelay.isTrigger = true;
            rigidbodyrelay.gravityScale = 0.0f;
        }
    }
    private bool Checkrelay(float r , float sn)
    {
        colliderrelay = null;
        rigidbodyrelay = null;
        transformrelay = null;
        Collider2D[] cols = Physics2D.OverlapCircleAll(player.transform.position, r);
        int length = cols.Length;
        if(length > 0)
        {
            for(int i = 0; i < length; ++i)
            {
                Vector2 Dir = cols[i].transform.position - player.position;
                Vector2 DirPlayer;
                DirPlayer.x = -relaydirection;
                DirPlayer.y = 0;
                Vector3 forw = Dir;
                Vector3 forw1 = DirPlayer;
                if (Vector2.Angle(Dir, player.transform.localEulerAngles) < sn && cols[i].isTrigger == false && cols[i].gameObject.tag == "relay")
                {
                    colliderrelay = cols[i];
                    return true;
                }
            }
        }
        return false;
    }
    private void RelayFollow()
    {
        Vector3 end = new Vector3(transform.localPosition.x - relaydirection * movex, transform.localPosition.y + movey, transform.localPosition.z);
        transformrelay.localPosition = Vector3.MoveTowards(transformrelay.localPosition, end, speed * 5.0f * Time.deltaTime);
    }
}
