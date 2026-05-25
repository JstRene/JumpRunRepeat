using System.Collections;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup victoryCanvasGroup;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Optional")]
    [SerializeField] private Character playerCharacter;

    private bool collected = false;

    private void Start()
    {
        if (this.victoryCanvasGroup != null)
        {
            this.victoryCanvasGroup.alpha = 0.0f;
            this.victoryCanvasGroup.interactable = false;
            this.victoryCanvasGroup.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        this.collected = true;

        if (this.playerCharacter == null)
        {
            this.playerCharacter = other.GetComponent<Character>();

            if (this.playerCharacter == null)
            {
                this.playerCharacter = other.GetComponentInParent<Character>();
            }

            if (this.playerCharacter == null)
            {
                this.playerCharacter = other.GetComponentInChildren<Character>();
            }
        }

        if (this.playerCharacter != null)
        {
            this.playerCharacter.enabled = false;
        }

        Collider jewelCollider = this.GetComponent<Collider>();
        if (jewelCollider != null)
        {
            jewelCollider.enabled = false;
        }

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        StartCoroutine(FadeInVictoryScreen());
    }

    private IEnumerator FadeInVictoryScreen()
    {
        float timer = 0.0f;

        while (timer < this.fadeDuration)
        {
            float percent = timer / this.fadeDuration;
            this.victoryCanvasGroup.alpha = percent;

            yield return null;
            timer += Time.deltaTime;
        }

        this.victoryCanvasGroup.alpha = 1.0f;
        this.victoryCanvasGroup.interactable = true;
        this.victoryCanvasGroup.blocksRaycasts = true;
    }
}