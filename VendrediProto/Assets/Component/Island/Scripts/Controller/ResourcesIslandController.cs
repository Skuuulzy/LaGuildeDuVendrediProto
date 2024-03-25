using UnityEngine;

public class ResourcesIslandController : MonoBehaviour
{
    [SerializeField] private RessourcesIslandSO _islandData;
	[SerializeField] private RessourcesIslandView _view;

    public RessourcesIslandSO IslandData => _islandData;

    private void Start()
    {
	    _view.Init(_islandData);
    }
}