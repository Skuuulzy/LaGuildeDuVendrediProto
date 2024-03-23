using QFSW.QC.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CannonShot : MonoBehaviour
{
    [SerializeField] private VisualEffect[] right_cannons_effect;
    [SerializeField] private VisualEffect[] left_cannons_effect;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CannonsShot(false, true);
        }
    }

    private void CannonsShot(bool right_shot, bool left_shot)
    {
        if (right_shot)
        {
            foreach (VisualEffect effect in right_cannons_effect)
            {
                effect.Play();
            }
        }

        if (left_shot)
        {
            foreach (VisualEffect effect in left_cannons_effect)
            {
                effect.Play();
            }
        }
    }
}
