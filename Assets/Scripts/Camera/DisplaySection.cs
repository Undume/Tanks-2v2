using System;
using UnityEngine;

public class DisplaySection
{
    public Camera m_Camera;
    public int position;
    public Boolean hasPlayer;

    public DisplaySection(Camera c, int p, Boolean hp)
    {
        m_Camera = c;
        position = p;
        hasPlayer = hp;
    }
}
