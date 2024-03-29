using UnityEngine;

public class ResourcesIslandController : MonoBehaviour
{
    [SerializeField] private ResourcesIslandSO _islandData;
	[SerializeField] private RessourcesIslandView _view;

    public ResourcesIslandSO IslandData => _islandData;

    private void Start()
    {
	    _view.Init(_islandData);
    }
}