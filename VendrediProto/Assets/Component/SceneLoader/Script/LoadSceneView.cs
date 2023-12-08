using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using VTools;
using UnityEngine.Events;

public class LoadSceneView : MonoBehaviour
{
    [Header("Fade to black")]
    [SerializeField] private GameObject _fadeToBlackGO;
    [SerializeField] private Image _fadeToBlackImage;

    [Header("Static screen")]
    [SerializeField] private GameObject _staticScreenGO;
    [SerializeField] private Image _staticScreenImage;

    [Header("Animated screen")]
    [SerializeField] private GameObject _animatedScreenGO;
    [SerializeField] private RectTransform _animatedScreenParent;

    public static UnityAction<float> OnFadeToBlack;
    public static UnityAction<float> OnFadeToClear;

    public static UnityAction<Sprite> OnDisplayStaticScreen;
    public static UnityAction OnHideStaticScreen;

    private void Awake()
    {
        LoadSceneManager.LoadScenesEvent += StartLoadSequence;
        OnFadeToBlack += StartFadeToBlack;
        OnFadeToClear += StartFadeToClear;
        OnDisplayStaticScreen += OnDisplayStaticScreen;
        OnHideStaticScreen += HideStaticScreen;
    }

    private void OnDestroy()
    {
        LoadSceneManager.LoadScenesEvent -= StartLoadSequence;
        OnFadeToBlack -= StartFadeToBlack;
        OnFadeToClear -= StartFadeToClear;
        OnDisplayStaticScreen -= OnDisplayStaticScreen;
        OnHideStaticScreen -= HideStaticScreen;
    }

    private async void StartLoadSequence(LoadSceneDataSO loadSceneData)
    {
        while (LoadSceneManager.IsLoading)
        {
            await Task.Delay(30);
        }

        if(loadSceneData.LoadType is LoadTypeSONoScreen)
        {
            await LoadSceneManager.LoadScenesAsync(loadSceneData);
        }
        if (loadSceneData.LoadType is LoadTypeSOFadeToBlack fadeToBlack)
        {
            await FadeToBlack(fadeToBlack.FadeInTime);

            await LoadSceneManager.LoadScenesAsync(loadSceneData);

            await FadeToClear(fadeToBlack.FadeOutTime);

            if (fadeToBlack != null)
            {
            _fadeToBlackGO.SetActive(false);
        }
        }
        if (loadSceneData.LoadType is LoadTypeSOStaticScreen staticScreen)
        {
            _staticScreenGO.SetActive(true);
            _staticScreenImage.sprite = staticScreen.SpriteToDisplay;

            await Task.Delay(30);
            await LoadSceneManager.LoadScenesAsync(loadSceneData);

            _staticScreenGO.SetActive(false);
        }
        if (loadSceneData.LoadType is LoadTypeSOAnimatedScreen animatedScreen)
        {
            _animatedScreenGO.SetActive(true);
            Animator animator = Instantiate(animatedScreen.AnimationPrefab);
            animator.Play("StartLoading");

            await LoadSceneManager.LoadScenesAsync(loadSceneData);

            _animatedScreenGO.SetActive(false);
        }
    }

    private void StartFadeToBlack(float fadeTime)
    {
        Task _ = FadeToBlack(fadeTime);
    }

    private void StartFadeToClear(float fadeTime)
    {
        Task _ = FadeToClear(fadeTime);
    }

    private async Task FadeToBlack(float fadeTime)
    {
        _fadeToBlackGO.SetActive(true);
        if (fadeTime > 0f)
        {
            _fadeToBlackImage.SetAlpha(0f);
            float startTime = Time.time;
            float timer = 0f;
            while (timer < fadeTime)
            {
                await Task.Delay(30);
                timer = Time.time - startTime;
                _fadeToBlackImage.SetAlpha(timer / fadeTime);
            }
        }
        else
        {
            _fadeToBlackImage.SetAlpha(1f);
        }
    }

    private async Task FadeToClear(float fadeTime)
    {
        if (fadeTime > 0f)
        {
            _fadeToBlackGO.SetActive(true);
            _fadeToBlackImage.SetAlpha(1f);
            float startTime = Time.time;
            float timer = 0f;
            while (timer < fadeTime)
            {
                await Task.Delay(30);
                timer = Time.time - startTime;
                _fadeToBlackImage.SetAlpha(1f - (timer / fadeTime));
            }
            _fadeToBlackGO.SetActive(false);
        }
        else
        {
            _fadeToBlackImage.SetAlpha(0f);
            _fadeToBlackGO.SetActive(false);
        }
    }

    private void ShowStaticScreen(Sprite imageToShow)
    {
        _staticScreenImage.sprite = imageToShow;
        _staticScreenGO.SetActive(true);
    }

    private void HideStaticScreen()
    {
        _staticScreenGO.SetActive(false);
    }
}
