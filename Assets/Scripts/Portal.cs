using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading levels
using System.Collections;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;

    public CanvasGroup faderCanvas;
    public float fadeDuration = 1.0f;

    private bool isTransitioning = false;

    private void Start()
    {
        faderCanvas.alpha = 1f;
        StartCoroutine(FadeCanvas(faderCanvas, 1f, 0f)); 
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(TransitionToScene(collision));
        }
    }

    private IEnumerator TransitionToScene(Collider2D player)
    {
        isTransitioning = true;

        PlayerController controller = player.GetComponent<PlayerController>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        controller.enabled = false;
        rb.linearVelocity = Vector2.zero;

        yield return StartCoroutine(FadeCanvas(faderCanvas, 0f, 1f));

        yield return new WaitForSeconds(0.5f);
   
        SceneManager.LoadScene(sceneToLoad);
        
        
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float startAlpha, float endAlpha)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            yield return null;
        }
        cg.alpha = endAlpha;
    }


}