using UnityEngine;

namespace RibertaGames
{
    public abstract class PresenterBase : MonoBehaviour
    {
        public virtual void Init()
        {
            _SetupModel();
            _SetupView();
            _Subscribe();
        }

        protected abstract void _SetupModel();

        protected abstract void _SetupView();

        protected abstract void _Subscribe();
    }
}