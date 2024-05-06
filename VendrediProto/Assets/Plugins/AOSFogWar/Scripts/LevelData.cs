using System.Collections.Generic;
using UnityEngine;

namespace FischlWorks_FogWar
{
    public partial class FogWar
    {
        /// A class for storing the base level data.
        /// 
        /// This class is later serialized into Json format.\n
        /// Empty spaces are stored as 0, while the obstacles are stored as 1.\n
        /// If a level is loaded instead of being scanned, 
        /// the level dimension properties of csFogWar will be replaced by the level data.
        [System.Serializable]
        public class FogLevelData
        {
            public void AddColumn(LevelColumn levelColumn)
            {
                levelRow.Add(levelColumn);
            }

            // Indexer definition
            public LevelColumn this[int index]
            {
                get
                {
                    if (index >= 0 && index < levelRow.Count)
                    {
                        return levelRow[index];
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in x axis is out of range");

                        return null;
                    }
                }
                set
                {
                    if (index >= 0 && index < levelRow.Count)
                    {
                        levelRow[index] = value;
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in x axis is out of range");

                        return;
                    }
                }
            }

            // Adding private getter / setters are not allowed for serialization
            public int levelDimensionX = 0;
            public int levelDimensionY = 0;

            public float unitScale = 0;

            public float scanSpacingPerUnit = 0;

            [SerializeField] private List<LevelColumn> levelRow = new List<LevelColumn>();
        }
    }
}