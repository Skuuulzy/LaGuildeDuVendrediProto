using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Island;
using VComponent.Multiplayer;
using VComponent.Multiplayer.Deliveries;
using VComponent.Tools.Singletons;
using VComponent.Tools.Timer;

public class DeliveryRequester : NetworkSingleton<DeliveryRequester>
{
    [Header("Parameters")]
    [SerializeField] private AnimationCurve _maxDeliveriesCountOverTime;
    [SerializeField] private AnimationCurve _minDeliveriesCountOverTime;
    
    [SerializeField] private int _deliveryGenerationInterval;

    private CountdownTimer _deliveryRequestTimer;
    
    private float GameProgress => MultiplayerGameplayManager.Instance.GameClock.Progress;

    private List<MultiplayerFactionIslandController> _factionIslands;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            Destroy(gameObject);
            return;
        }
        
        _factionIslands = FindObjectsOfType<MultiplayerFactionIslandController>().ToList();
    }

    private void Update()
    {
        _deliveryRequestTimer.Tick(Time.deltaTime);
    }

    public void StartDeliveriesGeneration()
    {
        // Generate the first deliveries.
        GenerateDeliveries();
        
        // Start the clock for the next deliveries.
        _deliveryRequestTimer = new CountdownTimer(_deliveryGenerationInterval);
        _deliveryRequestTimer.Start();
        
        // When the timer stop we try to generate deliveries and we relaunch the timer.
        _deliveryRequestTimer.OnTimerStop += () =>
        {
            GenerateDeliveries();
            _deliveryRequestTimer.Start();
        };
    }
    
    private void GenerateDeliveries()
    {
        int deliveryToGenerateCount = DeliveryToGenerateCount();
        if (deliveryToGenerateCount == 0)
        {
            return;
        }

        // We get the potential island by selecting those wo don't already have a delivery requested.
        var potentialIsland = _factionIslands.Where(island => !island.DeliveryRequested).ToList();

        // There is no potential islands.
        if (!potentialIsland.Any())
        {
            return;
        }

        potentialIsland = potentialIsland.OrderBy(island => island.DeliveriesRequestedCount).ToList();

        for (int i = 0; i < deliveryToGenerateCount; i++)
        {
            // There is not enough potential islands to generate all deliveries.
            if (i >= potentialIsland.Count)
            {
                break;
            }
            
            potentialIsland[i].RequestDelivery();
        }
    }

    /// <summary>
    /// Determine how many deliveries can be requested at the current game progress.
    /// </summary>
    private int DeliveryToGenerateCount()
    {
        int minDeliveryCount = Mathf.RoundToInt(_maxDeliveriesCountOverTime.Evaluate(GameProgress));
        int maxDeliveryCount = Mathf.RoundToInt(_minDeliveriesCountOverTime.Evaluate(GameProgress));

        if (minDeliveryCount > maxDeliveryCount || maxDeliveryCount < minDeliveryCount)
        {
            Debug.LogError($"Please check the generation curve for progress: {GameProgress:0.00} the data are not logic.");
            minDeliveryCount = maxDeliveryCount;
        }
        
        // We generate a random delivery count (+1 because Random.Range is exclusive on the upper value)
        int randomDeliveryCount = UnityEngine.Random.Range(minDeliveryCount, maxDeliveryCount + 1);
        
        int currentDeliveryCount = DeliveryManager.Instance.ActiveDeliveriesCount;

        // If there is enough deliveries return 0, otherwise the random count.
        return currentDeliveryCount >= randomDeliveryCount ? 0 : randomDeliveryCount;
    }
}