using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlePlayer : MonoBehaviour
{
    [SerializeField]
    float aceleracao,forcaPulo,velocidadeMaxima;
    [SerializeField]
    LayerMask mascaraDeLayers;
    [SerializeField]
    Image barraPulo;
    bool noChao = false;
    bool jumping=false;
    Rigidbody2D rb;//referencia para o componente do Rigidbody2D
   [SerializeField] Text pontos;

    InputAction move;
    InputAction jump;
    int ponto;
    private void Start()
    {
        pontos.text = "Itens: " + ponto.ToString("000");
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(BarraEncher());
    }
    private void Update()
    {
        if (jump.WasPressedThisFrame() && noChao && barraPulo.fillAmount>0)
        {
            jumping = true;
          barraPulo.fillAmount -= 0.1f;
        }
    }
    private void FixedUpdate()
    {
        Vector2 direcao = move.ReadValue<Vector2>();
        if (direcao != Vector2.zero)
        {
            rb.AddForce(Vector2.right * direcao.x * aceleracao, ForceMode2D.Force);
            if (rb.linearVelocity.magnitude > velocidadeMaxima)
            {
                rb.linearVelocityX = velocidadeMaxima * direcao.x;
            }
        }
        else
        {
            rb.AddForce(new Vector2(rb.linearVelocityX * -aceleracao, 0), ForceMode2D.Force);
        }
        if (jumping)
        {
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            jumping=false;
        }
       Vector2 baseObjeto = new Vector2(transform.position.x, 
            GetComponent<BoxCollider2D>().bounds.min.y);

        noChao = Physics2D.OverlapCircle(baseObjeto, 0.1f, mascaraDeLayers);
    }
    IEnumerator BarraEncher()
    {
        while (true)
        {
            if (barraPulo.fillAmount < 1)
            {
                barraPulo.fillAmount += 0.001f;
            }
       
        yield return new WaitForSeconds(1.0f); }
       
    }
    //Essa não é a melhor maneira de se checar 
    //se o Player está no chão
    private void OnCollisionStay2D(Collision2D collision)
    {
     //   noChao = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
     //   noChao = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            ponto++;
            pontos.text = "Itens: "+ ponto.ToString("000") ;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("respawn"))
        {
            transform.position = new Vector2(-4.6f, 0.5f);
        }
    }
}