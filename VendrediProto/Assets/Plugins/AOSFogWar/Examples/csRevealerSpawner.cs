/*
 * Created :    Spring 2023
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csRevealerSpawner.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */



using UnityEngine;          // Monobehaviour



namespace FischlWorks_FogWar
{



    public class csRevealerSpawner : MonoBehaviour
    {
        [SerializeField]
        private FogWar fogWar = null;

        [SerializeField]
        private GameObject exampleRevealer = null;



        private void Start()
        {
            // This part is meant to be modified following the project's scene structure later...
            try
            {
                fogWar = GameObject.Find("FogWar").GetComponent<FogWar>();
            }
            catch
            {
                Debug.LogErrorFormat("Failed to fetch csFogWar component. " +
                    "Please rename the gameobject that the module is attachted to as \"FogWar\", " +
                    "or change the implementation located in the csFogVisibilityAgent.cs script.");
            }
        }



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Vector3 randomPoint = new Vector3(
                    Random.Range(-fogWar.LevelData.levelDimensionX / 2.0f, fogWar.LevelData.levelDimensionX / 2.0f),
                    fogWar.LevelMidPoint.position.y + 0.5f,
                    Random.Range(-fogWar.LevelData.levelDimensionY / 2.0f, fogWar.LevelData.levelDimensionY / 2.0f));

                // Instantiating & fetching the revealer Transform
                Transform randomTransform = Instantiate(exampleRevealer, randomPoint, Quaternion.identity).GetComponent<Transform>();

                // Utilizing the constructor, setting updateOnlyOnMove to true will not update the fog texture immediately
                int index = fogWar.AddFogRevealer(new FogWar.FogRevealer(randomTransform, 3, false));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (fogWar.FogRevealers.Count > 2)
                {
                    fogWar.RemoveFogRevealer(fogWar.FogRevealers.Count - 1);
                }
            }
        }
    }



}