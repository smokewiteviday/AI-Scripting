using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public interface IState
    {
        void OnEnter();
        void Update();
        void FixedUpdate();
        void OnExit();
    }
    public abstract class BaseState : IState
    {
        public void OnEnter()
        {
            
        }
        public void Update()
        {
            
        }
        public void FixedUpdate()
        {

        }
        public void OnExit()
        {

        }
    }
}