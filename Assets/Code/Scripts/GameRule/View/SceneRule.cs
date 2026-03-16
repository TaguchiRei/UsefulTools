using UnityEngine;

public class SceneRule : MonoBehaviour
{
    [SerializeReference, SubclassSelector] private IRule[] _rules;
    
    private void Update()
    {
        
    }
}