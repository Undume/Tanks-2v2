using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class CameraTank : MonoBehaviour
    {

        private Camera m_Camera;
        private float smooth = 0.5f;
        private float limitDist = 20.0f;

        private Boolean camFixed = false;
        private Boolean fix = false;

        private void FixedUpdate()
        {
            if (m_Camera == null)
            {
                m_Camera = GetComponentInChildren<Camera>();
                return;
            }

            Follow();
        }

        private void Follow()
        {
            float currentDist = Vector3.Distance(transform.position, m_Camera.transform.position);
            if (!fix && currentDist > limitDist)
            {
                m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, transform.position,
                    Time.deltaTime * smooth);
            }
            else if(!fix)
            {
                //TODO revisar aquesta part ja que si el tank té rotació inicial surt girat!
                m_Camera.transform.localPosition = new Vector3(-9, 13, -6);
                m_Camera.transform.eulerAngles = new Vector3(40, m_Camera.GetComponentInParent<Transform>().localEulerAngles.y, 0);
                //Debug.Log(m_Camera.GetComponentInParent<Transform>().localEulerAngles.y);
                fix = true;
            }
        }
    }
}