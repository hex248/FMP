using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private List<float> selectableYPositions = new List<float>();
    private ScrollRect m_ScrollRect;

    bool selectedIsInList = false;
    bool hasBeenOpened;

    private Vector2 m_NextScrollPosition = Vector2.up;
    
    void OnEnable()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        
        ResetScroll();
    }
    
    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }
    
    void Start()
    {
        if (m_ScrollRect)
        {
            m_ScrollRect.content.GetComponentsInChildren(m_Selectables);
        }
        ResetScroll();
    }

    float GetNormalizedPosition(Selectable selected)
    {
        Selectable first = m_Selectables[0];
        Selectable last = m_Selectables[m_Selectables.Count - 1];
        float range = first.transform.position.y - last.transform.position.y;
        float fac = (first.transform.position.y - selected.transform.position.y) / range;
        return fac;
    }
    
    void Update()
    {
        // Scroll via input.
        InputScroll();
        if(selectedIsInList)
        {
            if (!mouseOver)
            {
                // Lerp scrolling code.
                m_ScrollRect.normalizedPosition = Vector2.Lerp(m_ScrollRect.normalizedPosition, m_NextScrollPosition, scrollSpeed * Time.deltaTime);
            }
            else
            {
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
        }
    }
    
    void InputScroll()
    {
        if (m_Selectables.Count > 0)
        {
            ScrollToSelected(false);
        }
    }
    
    void ScrollToSelected(bool quickScroll)
    {
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;
        selectedIsInList = (selectedElement != null) && m_Selectables.Contains(EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>());
        if (selectedIsInList)
        {

            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = new Vector2(0, 1 - GetNormalizedPosition(selectedElement));
                m_NextScrollPosition = m_ScrollRect.normalizedPosition;
            }
            else
            {
                m_NextScrollPosition = new Vector2(0, 1 - GetNormalizedPosition(selectedElement));
            }
        }
    }

    void ResetScroll()
    {
        m_ScrollRect.normalizedPosition = new Vector2(0, 1);
        m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        EventSystem.current.SetSelectedGameObject(m_Selectables[0].gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}