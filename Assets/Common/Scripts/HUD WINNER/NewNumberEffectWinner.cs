using System.Collections;
using UnityEngine;


public class NewNumberEffectWinner : MonoBehaviour
{
    [SerializeField] private Animator _fxAnimNumber;
    [SerializeField] private GameObject _numberContainer;

    //[SerializeField] private TextMeshProUGUI txtNumberNew;

    public void fxNewNumber(int seg, int num)
    {
        Activate(true);
        StartCoroutine(effectNewNumber(seg, num));
    }

    IEnumerator effectNewNumber(int seg, int num)
    {
        GameObject goNum = Instantiate(findNumberGo(num));
        goNum.transform.SetParent(GameObject.Find("Effect").transform);
        goNum.SetActive(true);
        if(num == 0)
        {
            goNum.transform.localPosition = new Vector3(-0.09f, -0.04f, 0f);
            goNum.transform.localScale = new Vector3(0.7044711f, 0.7263284f, 0f);
        } else
        {
            goNum.transform.localPosition = new Vector3(0.0268016f, 0.02796667f, 0f);
            goNum.transform.localScale = new Vector3(0.7044711f, 0.7263284f, 0f);
        }
        fxNumberAnimatorTrigger();

        yield return new WaitForSeconds(seg);
        Activate(false);
        Destroy(goNum);
    }

    private void Activate(bool on)
    {
        this.gameObject.SetActive(on);
    }

    private void fxNumberAnimatorTrigger()
    {
        _fxAnimNumber.SetTrigger("Effect");
    }

    private bool onRed(int num)
    {
        int[] auxRed = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
        bool aux = false;
        foreach(int a in auxRed)
        {
            if(num == a)
            {
                aux = true;
                break;
            }
        }
        return aux;
    }

    private GameObject findNumberGo(int num)
    {
        GameObject auxGo = _numberContainer.transform.GetChild(num).gameObject;
        return auxGo;
    }
}
