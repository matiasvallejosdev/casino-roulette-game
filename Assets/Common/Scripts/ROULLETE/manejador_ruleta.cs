using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class manejador_ruleta : Singlenton<manejador_ruleta>
{
    [SerializeField]private static GameObject sphere = null;
    private GameObject girar = null;
    private GameObject _girarBall = null;

    private GameObject cameraGo = null;
    private Animator cameraAnim = null;

    public manejador_ball sc_ball;
    [SerializeField][Range(0,1000)]private float speed = 0;

    private bool rotate_r = false;
    public Animator _roulleteFounded = null;
    // HUD
    private GameObject _fichasHud = null;
    private GameObject _backNumberHUD = null;
    private GameObject _saldosHUD = null;
    // UI
    [SerializeField] private GameObject _canvasUI = null;
    // HUD WINNER
    [SerializeField] private NewNumberEffectWinner _scFxNewNumber = null;

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
        rotate_r = true;
    }
    /// <summary>
    /// Init the rounded with random number
    /// </summary>
    /// <param name="numero"></param>
    public void start_giro(int numero)
    {
        int countFichas = GameObject.FindGameObjectsWithTag("Fichas").Length;
        if(countFichas > 0)
        {
            // Sound
            SoundContoller.Instance.fx_sound(5);
            // Intialize the rounded 
            RoundController.Instance.OnRoundIntialize();
            // Initialize the coroutine
            StartCoroutine(start_r(numero));
        }
        else
        {
            SoundContoller.Instance.fx_sound(3);
        }
    }
    /// <summary>
    /// Init the roullete and all components.
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
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
        SoundContoller.Instance.fx_sound(0);
        yield return new WaitForSeconds(0.5f);
        speed = 240f;
        yield return new WaitForSeconds(1.2f);
        speed = 245f;
        yield return new WaitForSeconds(2.0f);
        speed = 265;
        yield return new WaitForSeconds(3.8f);
        speed = 245;
        yield return new WaitForSeconds(1.5f);
        speed = 240f;
        sphere.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        speed = 145;

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
        PaymentController.Instance.roundFinished();
    }
    
    /// <summary>
    /// Set the focus vision in the roullete
    /// </summary>
    /// <param name="focusOn"></param>
    public void fx_focus(bool focusOn)
    {
        if(focusOn)
        {
            _roulleteFounded.gameObject.SetActive(true);
        } else 
        {
            _roulleteFounded.SetTrigger("off");
        }
    }
    
    /// <summary>
    /// Activate or desactivate the UI and HUD
    /// </summary>
    /// <param name="on"></param>
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
    void FixedUpdate()
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
