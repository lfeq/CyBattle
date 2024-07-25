using UnityEngine;
using UnityEngine.EventSystems;

public class Descriptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private GameObject dropDown;
    
    private void Start() {
        dropDown.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData t_eventData) {
        dropDown.SetActive(true);
    }

    public void OnPointerExit(PointerEventData t_eventData) {
        dropDown.SetActive(false);
    }
}