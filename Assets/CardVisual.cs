using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardVisual : MonoBehaviour
{
    private Card cardData;
    private Image image;
    private Sprite backSprite;
    public bool isFaceDown = true;

    public void Setup(Card card, Image img, Sprite back)
    {
        cardData = card;
        image = img;
        backSprite = back;
        image.sprite = backSprite;
    }

    public void Flip()
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        float duration = 0.25f;
        float t = 0;
        Vector3 startScale = transform.localScale;
        Vector3 midScale = new Vector3(0, startScale.y, startScale.z);

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(startScale, midScale, t);
            yield return null;
        }

        isFaceDown = !isFaceDown;
        image.sprite = isFaceDown ? backSprite : cardData.sprite;

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(midScale, startScale, t);
            yield return null;
        }
    }
}

