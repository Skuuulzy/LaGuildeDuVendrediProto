using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;

public class ShipResourcesCarriedView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentResourceCarriedNumber;
    [SerializeField] private Image _currentResourceCarriedImage;

    private ResourceType _currentResourceType = ResourceType.NONE;
    public ResourceType CurrentResourceType => _currentResourceType;

    public void Init(int numberOfResourceCarried, RessourcesSO resourcesSO)
    {
        UpdateNumberOfResourceCarried(numberOfResourceCarried);
        _currentResourceType = resourcesSO.Type;
		_currentResourceCarriedImage.sprite = resourcesSO.Sprite;
        gameObject.SetActive(true);
	}

    public void UpdateNumberOfResourceCarried(int numberOfResourceCarried)
    {
		_currentResourceCarriedNumber.text = numberOfResourceCarried.ToString();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}