using System;
using UnityEngine;

namespace Behaviours
{
    public class LoaderBehaviour : MonoBehaviour
    {
        protected void Awake()
        {
            throw new NotImplementedException();
        }

        [SerializeField] private GameObject _segmentPrefab = default;
    }
}