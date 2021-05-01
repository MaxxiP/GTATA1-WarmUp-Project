using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Namespace existiert nur im Editor, nicht im Build deshalb muss es zum builden exkludiert werden
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
public class DynamicButtonCallBackAttachment : MonoBehaviour
{

    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Button>().onClick.AddReloadableListener(PlaySound);
        //GameObject.Find("Next").GetComponent<Button>().onClick.AddReloadableListener(PlaySound);
        GameObject.Find("Next").GetComponent<Button>().onClick.AddReloadableListener(SpriteNext);

        GameObject.Find("Previous").GetComponent<Button>().onClick.AddReloadableListener(SpritePrevious);

    }

    private void PlaySound()
    {
        //Debug.Log("Button pressed");
        
        // Ändern des spieler charakters irgendwie so..
        //characterSprite.sprite.texture ... .
    }

    private void SpritePrevious()
    {
        audioSource.Play();
        Debug.Log("Select previous Sprite");
        
    }
    
    private void SpriteNext()
    {
        audioSource.Play();
        Debug.Log("Select next Sprite");
    }
}

public static class UnityEventExtension
{
    public static void AddReloadableListener(this UnityEvent eventHost, UnityAction callback)
    {
#if UNITY_EDITOR
        UnityEventTools.AddPersistentListener(eventHost, callback);
#else
        eventHost.AddListener(callback);
#endif
    }
}
