using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class HandlerRoulleteWheel : Singlenton<HandlerRoulleteWheel>
{
    /*
    [Header("Wheel components")]
    public Animator _roulleteFounded = null;
    
    [Header("Wheel configuration")]
    public GameObject sphere = null;
    // UI
    [SerializeField] private GameObject _canvasUI = null;
    // HUD WINNER
    [SerializeField] private NewNumberEffectWinner _scFxNewNumber = null;
    [SerializeField][Range(0,1000)]private float speed = 0;

    [Header("Wheel references")]    
    public HandlerBall handlerBallScript;
    public CanvasHUD canvasHud;
    public GameObject ballRotator = null;
    public GameObject wheelRotator = null;

    private GameObject _cameraGame = null;
    private Animator _cameraAnimator = null;
    private bool _rotateWheelRoullete = false;

    // Start is called before the first frame update
    void Start()
    {
        _cameraGame = Camera.main.gameObject;
        _cameraAnimator = _cameraGame.GetComponent<Animator>();

        _rotateWheelRoullete = true;
    }

    public void StartRound(int numero)
    {
        int countFichas = GameObject.FindGameObjectsWithTag("Fichas").Length;
        if(countFichas > 0)
        {
            // Sound
            SoundContoller.Instance.PlayFxSound(6);
            // Intialize the rounded 
            RoundController.Instance.OnRoundIntialize();
            // Initialize the coroutine
            StartCoroutine(StartRotationRoullete(numero));
        }
        else
        {
            SoundContoller.Instance.PlayFxSound(4);
        }
    }
    IEnumerator StartRotationRoullete(int randomNumber)
    {
        RoundController.Instance.ActivateButtons(false);

        Destroy(handlerBallScript.RotatorBallSphere);
        sphere.SetActive(true);
        
        speed = 35f;
        
        FxFocusFoundedRoullete(true);
        _cameraAnimator.SetBool("Mover", true);
       
        FxCanvas(false);
        //fx_focus(true);

        yield return new WaitForSeconds(2.0f);
        speed = 75f;
        yield return new WaitForSeconds(1.0f);
        speed = 145f;
        SoundContoller.Instance.PlayFxSound(8);
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
        handlerBallScript.SetBallInWheel(randomNumber);
        yield return new WaitForSeconds(1.8f);
        FxFocusFoundedRoullete(false);
        speed = 75f;
        // Fx
        _scFxNewNumber.fxNewNumber(4,randomNumber);
        yield return new WaitForSeconds(5.0f);
        speed = 35f;
        _cameraAnimator.SetBool("Mover", false);
        FxCanvas(true);

        // Intialize the payment system and display the news values
        // Finished the rounded 

        // Active Buttons
        RoundController.Instance.ActivateButtons(true);
        PaymentController.Instance.roundFinished();
    }
    
    public void FxFocusFoundedRoullete(bool focusOn)
    {
        if(focusOn)
        {
            _roulleteFounded.gameObject.SetActive(true);
        } else 
        {
            _roulleteFounded.SetTrigger("off");
        }
    }

    private void FxCanvas(bool on)
    {
        if(on)
        {
            // HUD
            canvasHud.OnWheelRotate(true);
            // UI
            _canvasUI.SetActive(true);
        }
        else 
        {
            // HUD
            canvasHud.OnWheelRotate(false);
            // UI
            _canvasUI.SetActive(false);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        RotateWheelRoullete();
    }

    private void RotateWheelRoullete()
    {
        if(_rotateWheelRoullete == true)
        {
            wheelRotator.transform.Rotate(Vector3.forward * speed * Time.deltaTime);
            ballRotator.transform.Rotate(Vector3.back * speed * 3 * Time.deltaTime);  
        }
    }*/
}
