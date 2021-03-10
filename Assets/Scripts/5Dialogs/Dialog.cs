using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using HeroLeft.Misc;
namespace HeroLeft.Dialog
{
    public class Dialog : MonoBehaviour
    {
        public Text dialogText;
        public DialogSystem dialogSystem;
        public int dialogStep;

        [Space()]
        public float characterPause = 0.1f;
        public bool startWriting = true;

        [Space()]
        public Text ButtonText;
        public GameObject[] panels;
        int activedPn = 0;

        public DialogSystem.phrase availPhrase => dialogSystem.Phrases[dialogStep];

        Coroutine enumerator;

        public void WriteText(string content)
        {
            if (enumerator != null)
                StopCoroutine(enumerator);
            dialogText.text = "";
            enumerator = StartCoroutine(CharacterAdd(content));
        }


        public IEnumerator CharacterAdd(string content)
        {
            int i = 0;
            while (i != content.Length)
            {
                dialogText.text += content[i];
                i++;
                yield return new WaitForSeconds(characterPause);
            }
        }

        public void PhrasePlus()
        {
            dialogStep++;
        }

        public void NextPhrase()
        {
            if (dialogStep < dialogSystem.Phrases.Length - 1)
            {
                dialogStep++;
                ButtonText.text = availPhrase.buttonText;
                if (availPhrase.panelActive != -1)
                {

                    if (availPhrase.type == 0)
                        panels[activedPn].SetActive(false);
                    if (activedPn != availPhrase.panelActive)
                    {
                        activedPn = availPhrase.panelActive;
                        panels[availPhrase.panelActive].SetActive(!panels[availPhrase.panelActive].activeSelf);
                    }

                    if(availPhrase.type == 1)
                    {
                        panels[0].SetActive(false);
                        GetComponent<Traing>().NextStep();
                    }
                }
                else
                {
                    ReloadPhrase();
                }
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
        }

        public void ActiveDialog()
        {
            panels[availPhrase.panelActive].SetActive(true);
            ReloadPhrase();
        }

        public void MainDialogActive()
        {
            panels[0].SetActive(true);
            ReloadPhrase();
        }

        public void ReloadPhrase()
        {
            WriteText(availPhrase.text);
        }

        void Start()
        {
            if (panels.Length > 0)
                panels[0].SetActive(true);
            if (startWriting)
                ReloadPhrase();
        }
    }
}