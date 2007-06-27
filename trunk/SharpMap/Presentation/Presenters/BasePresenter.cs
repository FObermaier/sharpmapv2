using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Presentation
{
    /// <summary>
    /// The base presenter for map views.
    /// </summary>
    /// <typeparam name="TView">Type of view to manage.</typeparam>
    public abstract class BasePresenter<TView> : IDisposable
        where TView : class
    {
        private readonly SharpMap.Map _map;
        private readonly TView _view;
        private bool _disposed = false;

        #region Object Construction/Destruction
        /// <summary>
        /// Constructs a new presenter with the given map and view.
        /// </summary>
        /// <param name="map">The map model.</param>
        /// <param name="view">The view to keep synchronized with the model and to accept input from.</param>
        protected BasePresenter(SharpMap.Map map, TView view)
        {
            _map = map;
            _view = view;
        }

        ~BasePresenter()
        {
            Dispose(false);
        }

        #region Dispose Pattern
        #region IDisposable Members
        /// <summary>
        /// Releases all resources deterministically.
        /// </summary>
        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        /// <summary>
        /// Releases all resources, and removes from finalization queue if <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">True if being called deterministically, false if being called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
            }
        }

        /// <summary>
        /// Gets whether this presenter is disposed, and no longer accessible.
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }
        #endregion
        #endregion

        /// <summary>
        /// The map to present.
        /// </summary>
        /// <remarks>
        /// This is the model which is kept synchronized to the view, 
        /// and which input on the view modifies through this presenter.
        /// </remarks>
        public SharpMap.Map Map
        {
            get 
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(typeof(ToolsPresenter).ToString());
                }

                return _map; 
            }
        }

        /// <summary>
        /// The view used to accept input and keep synchronized with the <see cref="Map">model</see>.
        /// </summary>
        public TView View
        {
            get
            {
                if (Disposed)
                {
                    throw new ObjectDisposedException(typeof(ToolsPresenter).ToString());
                }

                return _view; 
            }
        }
    }
}