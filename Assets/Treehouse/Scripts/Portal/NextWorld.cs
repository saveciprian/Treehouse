using UnityEngine;
using UnityEngine.SceneManagement;

public class NextWorld : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("Mohammad");
    }

    public void Outline()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
