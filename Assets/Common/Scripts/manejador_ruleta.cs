using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class manejador_ruleta : MonoBehaviour
{
    [SerializeField]private GameObject sphere;
    private GameObject girar;
    private GameObject _girarBall;

    private GameObject cameraGo;
    private Animator cameraAnim;

    public manejador_ball sc_ball;
    [SerializeField][Range(0,1000)]private float speed;
    [SerializeField][Range(0,1000)]private float _speedBall;
    private bool rotate_r;
    private Animator _wheelFounded;
    // HUD
    private GameObject _fichasHud;
    private GameObject _backNumberHUD;
    private GameObject _saldosHUD;
    // UI
    [SerializeField] private GameObject _canvasUI;
    // HUD WINNER
    [SerializeField] private NewNumberEffectWinner _scFxNewNumber;

    // Start is called before the first frame update
    void Start()
    {
        _fichasHud = GameObject.Find("FichasHUD");
        _backNumberHUD = GameObject.Find("BackNumberHUD");
        _saldosHUD = GameObject.Find("SaldosHUD");

        girar = GameObject.Find("Girar");
        _girarBall = GameObject.Find("Ball");

        cameraGo = GameObject.Find("Main Camera");
        cameraAnim = cameraGo.GetComponent<Animator>();

        sphere = GameObject.Find("Sphere");
        _wheelFounded = GameObject.Find("fx_shadow_roullete").GetComponent<Animator>();
        rotate_r = true;
    }

    public void start_giro(int numero)
    {
        int countFichas = GameObject.FindGameObjectsWithTag("Fichas").Length;
        if(countFichas > 0)
        {
            // Sound
            SoundContoller.Instance.fx_sound(5);
            // Intialize the rounded 
            RoundController.Instance.onRoundIntialize();
            // Initialize the coroutine
            StartCoroutine(start_r(numero));
        }
        else
        {
            SoundContoller.Instance.fx_sound(3);
        }
    }

    IEnumerator start_r(int num)
    {
        Destroy(sc_ball._newSphere);
        sphere.SetActive(true);
        speed = 35f;
        fx_focus(true);
        cameraAnim.SetBool("Mover", true);
        fx_canvas(false);
        //fx_focus(true);
        yield return new WaitForSeconds(2.0f);
        speed = 75f;
        yield return new WaitForSeconds(1.0f);
        speed = 145f;
        _speedBall = 45f;
        SoundContoller.Instance.fx_sound(0);
        yield return new WaitForSeconds(0.5f);
        speed = 240f;
        _speedBall = 145f;
        yield return new WaitForSeconds(1.2f);
        speed = 245f;
        _speedBall = 330f;
        yield return new WaitForSeconds(2.0f);
        speed = 265;
        _speedBall = 500;
        yield return new WaitForSeconds(3.8f);
        speed = 245;
        _speedBall = 330f;
        yield return new WaitForSeconds(1.5f);
        speed = 240f;
        _speedBall = 245f;
        sphere.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        speed = 145;
        _speedBall = 0f;
        // Ball position
        sc_ball.colocar_ball(num);
        yield return new WaitForSeconds(1.8f);
        fx_focus(false);
        speed = 75f;
        // Fx
        _scFxNewNumber.fxNewNumber(4,num);
        yield return new WaitForSeconds(5.0f);
        speed = 35f;
        cameraAnim.SetBool("Mover", false);
        fx_canvas(true);

        // Intialize the payment system and display the news values
        // Finished the rounded 

        // Active Buttons
        //RoundController.Instance.activeButtons(true);
        PaymentController.Instance.roundFinished(num);
    }

    public void fx_focus(bool focusOn)
    {
        if(focusOn)
        {
            _wheelFounded.SetTrigger("FoundIn");
        } else 
        {
            _wheelFounded.SetTrigger("FoundOut");
        }
    }
    private void fx_canvas(bool on)
    {
        if(on)
        {
            // HUD
            _fichasHud.SetActive(true);
            _backNumberHUD.SetActive(true);
            _saldosHUD.SetActive(true);
            // UI
            _canvasUI.SetActive(true);
        }
        else 
        {
            // HUD
            _fichasHud.SetActive(false);
            _backNumberHUD.SetActive(false);
            _saldosHUD.SetActive(false);
            // UI
            _canvasUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotate_roullete();
    }

    private void rotate_roullete()
    {
        if(rotate_r == true)
        {
            girar.transform.Rotate(Vector3.forward * speed * Time.deltaTime);
            _girarBall.transform.Rotate(Vector3.back * speed * 3 * Time.deltaTime);  
        }
    }
}
