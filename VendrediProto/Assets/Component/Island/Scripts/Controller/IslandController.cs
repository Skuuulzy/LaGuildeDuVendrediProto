using System.Linq;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    [SerializeField] private IslandView _islandView;

    [SerializeField] private IslandSO _islandSO;

    private MerchandiseType _currentMerchandiseAsked;
    private int _currentMerchandiseAskedValue;
	private bool _isAskingForAMerchandise;

	private MerchandiseType _currentMerchandiseToSell;
	private int _currentMerchandiseToSellValue;
	private bool _isGivingAMerchandise;

	private void Start()
	{
		InitIslandStat();
		
	}

	private void InitIslandStat()
	{
		if(_islandSO.MerchandisesRequested != null && _islandSO.MerchandisesRequested.ToDictionary().Count != 0)
		{
			// Call the fonction AskForMerchandise every 3 minutes, en commen�ant apr�s une seconde d'attente.
			InvokeRepeating(nameof(ChangeMerchandiseToSell), 1f, _islandSO.MerchandiseToSellTimeInterval);
		}
		if (_islandSO.MerchandisesToSell != null && _islandSO.MerchandisesToSell.ToDictionary().Count != 0)
		{
			InvokeRepeating(nameof(GenerateMerchandiseToAsk), 1f, _islandSO.MerchandiseRequestedTimeInterval);
		}
	}

	#region MERCHANDISES MANAGEMENT
	/// <summary>
	/// Ask for a new merchandise type and display this with the value to receive 
	/// </summary>
	public void GenerateMerchandiseToAsk()
    {
		//Choose a random merchandise to ask from the availables
        int randomNumber = Random.Range(0, _islandSO.MerchandisesRequested.ToDictionary().Count);
        _currentMerchandiseAsked = _islandSO.MerchandisesRequested.ToDictionary().ElementAt(randomNumber).Key;
		_currentMerchandiseAskedValue = _islandSO.MerchandisesRequested.ToDictionary().ElementAt(randomNumber).Value;

        //UpdateTheView
        _islandView.DisplayMerchandiseAsked(_currentMerchandiseAsked, _currentMerchandiseAskedValue);
	}
	/// <summary>
	/// Receive a merchandise from a player (or another one ?)
	/// </summary>
	private void ReceiveMerchandise(int numberOfMerchandiseReceived)
	{
		//Substrack the number of received merchandise from the current merchandise asked
		_currentMerchandiseAskedValue -= numberOfMerchandiseReceived;
		_currentMerchandiseAskedValue = Mathf.Max(0, _currentMerchandiseAskedValue);

		//Give reward for successfullMerchandise

		//UpdateTheView
		_islandView.DisplayMerchandiseAsked(_currentMerchandiseAsked, _currentMerchandiseAskedValue);
	}
	/// <summary>
	/// Check if the merchandise is the same as the available
	/// </summary>
	public void CheckReceivedMerchandise(MerchandiseType merchandiseType, int numberOfMerchandiseReceived)
	{
		if(merchandiseType == _currentMerchandiseAsked)
		{
			ReceiveMerchandise(numberOfMerchandiseReceived);
		}
		else
		{
			Debug.LogError("Not the same merchandise as the current asked");
		}
	}

	/// <summary>
	/// Change the merchandise to sell to players
	/// </summary>
	public void ChangeMerchandiseToSell()
    {
		//Choose a random merchandise to sell from the availables
		int randomNumber = Random.Range(0, _islandSO.MerchandisesToSell.ToDictionary().Count);
		_currentMerchandiseToSell = _islandSO.MerchandisesToSell.ToDictionary().ElementAt(randomNumber).Key;
		_currentMerchandiseToSellValue = _islandSO.MerchandisesToSell.ToDictionary().ElementAt(randomNumber).Value;

		//UpdateTheView
		_islandView.DisplayMerchandiseToSell(_currentMerchandiseToSell, _currentMerchandiseToSellValue);
	}
	/// <summary>
	/// Sell a merchandise to a player (or another one ?)
	/// </summary>
	public void SellMerchandise(int numberOfMerchandiseToSell)
	{
		//Substrack the number of sold merchandise from the current merchandise to sell
		_currentMerchandiseToSellValue -= numberOfMerchandiseToSell;
		_currentMerchandiseToSellValue = Mathf.Max(0, _currentMerchandiseToSellValue);

		//UpdateTheView
		_islandView.DisplayMerchandiseToSell(_currentMerchandiseToSell, _currentMerchandiseToSellValue);
	}
	#endregion MERCHANDISES MANAGEMENT
}