using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public string levelToLoad;

    private bool gameStarted = false;

    void Start()
    {
        StartCoroutine(PlayIdleLoop());
    }

    // Coroutine to loop between Idle1 and Idle2 animations
    IEnumerator PlayIdleLoop()
    {
        while (!gameStarted)
        {
            animator.Play("Idle1");
            yield return new WaitForSecondsRealtime(GetAnimationLength("Idle1"));

            animator.Play("Idle2");
            yield return new WaitForSecondsRealtime(GetAnimationLength("Idle2"));
        }
    }

    // Method to get the length of the animation clip
    float GetAnimationLength(string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    // Call this function when the user presses "Start Game"
    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            StopCoroutine(PlayIdleLoop()); // Stop the idle loop
            StartCoroutine(PlayStartupAnimation());
        }
    }

    // Coroutine to play the startup animation and then load the level
    IEnumerator PlayStartupAnimation()
    {
        animator.Play("Startup");
        yield return new WaitForSecondsRealtime(GetAnimationLength("Startup") - 0.5f);

        Debug.Log("Animation finished!");
        // After startup animation finishes, load the level
        SceneManager.LoadScene(levelToLoad);
        Time.timeScale = 1;
    }
}
