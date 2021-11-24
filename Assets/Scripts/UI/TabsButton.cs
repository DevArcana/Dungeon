using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class TabsButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        public TabGroup tabGroup;

        [NonSerialized]public Image background;

        public RecipeType recipeType;
        // Start is called before the first frame update
        void Start()
        {
            background = GetComponent<Image>();
            tabGroup.Subscribe(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }
    }
}
