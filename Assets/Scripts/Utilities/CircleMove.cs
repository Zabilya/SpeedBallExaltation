﻿using UnityEngine;

//TODO: check for ability to change gameobject transform in editor via script "online" 

namespace Utilities
{
    public class CircleMove : MonoBehaviour
    {
        public Vector3 centre = new Vector3(0.0f, 1.5f, 0.0f);
        public float rotateSpeed = 1f;
        public float angle = 90.0f;

        private const float Radius = 2f;
        
        private void Start()
        {
            angle *= Mathf.Deg2Rad;
            centre = new Vector3(0.0f, transform.position.y, 0.0f);
        }

        private void Update()
        {
            angle += rotateSpeed * Time.deltaTime;
            var offset = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle)) * Radius;
            transform.position = centre + offset;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(centre, 0.1f);
            Gizmos.DrawLine(centre, transform.position);
        }
    }
}
