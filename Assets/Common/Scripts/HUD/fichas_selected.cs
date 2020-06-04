using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fichas_selected : MonoBehaviour
{
    private manejador_fichas _scManejadorFichas;
    public Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _scManejadorFichas = GameObject.Find("Fichas_Container").GetComponent<manejador_fichas>();
        _animator = GetComponent<Animator>();
    }
    public void selectedFicha_UI()
    {
        _animator.SetTrigger("select");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name != "MagnetFichas")
        {
            SoundContoller.Instance.fx_sound(7);
            selectedFicha_UI();
            string fichaNum;
            int aux = 0;
            if (other.gameObject.name != "all")
            {
                fichaNum = other.gameObject.name.ToString();
                aux = int.Parse(fichaNum);
            }
            _scManejadorFichas.num_ficha(aux);
        }
    }
}
