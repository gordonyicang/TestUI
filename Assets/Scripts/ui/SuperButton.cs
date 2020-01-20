using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SuperButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    protected TouchPos _clickCB;
    protected TouchPos _downCB;
    protected TouchPos _upCB;
    protected bool _touchEnabled = true;
    public string _name;

    public delegate void TouchPos(string name,float x, float y);

    public void setLuaCallback(string name, TouchPos clickCB,TouchPos downCB = null, TouchPos upCB = null)
    {
        _name = name;
        _clickCB = clickCB;
        _downCB = downCB;
        _upCB = upCB;
    }

    public void setTouchEnabled(bool v)
    {
        _touchEnabled = v;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_touchEnabled && _clickCB != null)
        {
           _clickCB(_name, eventData.position.x,eventData.position.y);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_touchEnabled && _downCB != null)
        {
           _downCB(_name, eventData.position.x, eventData.position.y);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(_touchEnabled && _upCB != null)
        {
            _upCB(_name, eventData.position.x, eventData.position.y);
        }
    }
    public void OnDestory()
    {
        _clickCB = null;
        _downCB = null;
        _upCB = null;
    }
}
