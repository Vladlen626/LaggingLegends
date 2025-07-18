using DG.Tweening;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform _blackScreen;
    [SerializeField] private Transform _winScreen;
    [SerializeField] private Menu _menu;
    [SerializeField] private FmodSoundSettings _fmodSoundSettings;
    [SerializeField] private Transform _worldCanvasTransform;
    [SerializeField] private PickupMarker _pickupMarkerPrefab;
    
    public Menu Menu => _menu;
    
    public void Initialize()
    {
        _blackScreen.localScale = Vector3.one;
        _fmodSoundSettings.Initialize();
        _menu.Initialize();
    }
    
    public void Win()
    {
        _winScreen.gameObject.SetActive(true);
        _winScreen.localScale = Vector3.zero;
        _winScreen.DOScale(1f, 0.15f);
    }

    public void ShowBlackScreen()
    {
        _blackScreen.localScale = Vector3.one;
    }

    public void HideBlackScreen()
    {
        _blackScreen.DOScale(0f, 0.12f);
        _menu.Restart();
    }
    
    public PickupMarker CreatePickupMarker()
    {
        var marker = Instantiate(_pickupMarkerPrefab, _worldCanvasTransform);
        return marker;
    }
}