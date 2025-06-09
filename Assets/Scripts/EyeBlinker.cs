using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlinker : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshEyes;
    private int blendshapeIndexLeft;
    private int blendshapeIndexRight;

    [Header("Properties")]
    public float blinkInterval = 5.0f;  
    public float blinkEyeCloseDuration = 0.06f;
    public float blinkOpeningSeconds = 0.03f; 
    public float blinkClosingSeconds = 0.1f;  

    public Coroutine blinkCoroutine;

    private void Awake()
    {
        blendshapeIndexLeft = GetBlendshapeIndex("head_01.eyeBlinkLeft");
        blendshapeIndexRight = GetBlendshapeIndex("head_01.eyeBlinkRight");
    }

    private int GetBlendshapeIndex(string blendshapeName)
    {
        Mesh mesh = skinnedMeshEyes.sharedMesh;
        int blendshapeIndex = mesh.GetBlendShapeIndex(blendshapeName);
        return blendshapeIndex;
    }

    private IEnumerator BlinkRoutine()
    {
        //This is an infinite loop coroutine
        while (true)
        {
            //Wait until we need to blink
            yield return new WaitForSeconds(blinkInterval);

            //Close eyes
            var value = 0f;
            var closeSpeed = 1.0f / blinkClosingSeconds;
            while (value < 1)
            {
                skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexLeft, value * 100);
                skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexRight, value * 100);
                value += Time.deltaTime * closeSpeed;
                yield return null;
            }
            skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexLeft, 100);
            skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexRight, 100);


            //Wait to open our eyes
            yield return new WaitForSeconds(blinkEyeCloseDuration);

            //Open eyes
            value = 1f;
            var openSpeed = 1.0f / blinkOpeningSeconds;
            while (value > 0)
            {
                skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexLeft, value * 100);
                skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexRight, value * 100);

                value -= Time.deltaTime * openSpeed;
                yield return null;
            }
            skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexLeft, 0);
            skinnedMeshEyes.SetBlendShapeWeight(blendshapeIndexRight, 0);
        }
    }

    private void OnEnable()
    {
        blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }
}