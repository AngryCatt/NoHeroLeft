using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeroLeft.Dialog
{
    public class Dialog : MonoBehaviour
    {
        public Text dialogText;
        public float characterPause = 0.1f;

        [Multiline(12)] public string content;
        Coroutine enumerator;

        [ContextMenu("write")]
        public void WriteText()
        {
            if (enumerator != null)
                StopCoroutine(enumerator);
            dialogText.text = "";
            enumerator = StartCoroutine(CharacterAdd());
        }


        public IEnumerator CharacterAdd()
        {
            int i = 0;
            while (i != content.Length-1)
            {
                dialogText.text += content[i];
                i++;
                yield return new WaitForSeconds(characterPause);
            }
        }

    }
}