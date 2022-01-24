using System;
using System.Runtime.InteropServices;

namespace Unity.Media
{
    /// <summary>
    /// A class that handles the allocation and disposal of an object. All the different encoders use it.
    /// </summary>
    /// <typeparam name="T">The type of object that the handle refers to.</typeparam>
    public class RefHandle<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// Specifies whether the handle has been allocated or not.
        /// </summary>
        public bool IsCreated { get { return m_Handle.IsAllocated; } }

        /// <summary>
        /// The target object of the handle.
        /// </summary>
        public T Target
        {
            get
            {
                if (!IsCreated)
                    return null;

                return m_Handle.Target as T;
            }

            set
            {
                if (IsCreated)
                    m_Handle.Free();

                if (value != null)
                    m_Handle = GCHandle.Alloc(value, GCHandleType.Normal);
            }
        }

        GCHandle m_Handle;
        private bool Disposed = false;

        /// <summary>
        /// The constructor of the handle.
        /// </summary>
        public RefHandle()
        {
        }

        /// <summary>
        /// The constructor of the handle.
        /// </summary>
        /// <param name="target">The object to use as the target of the handle.</param>
        public RefHandle(T target)
        {
            m_Handle = new GCHandle();
            Target = target;
        }

        /// <summary>
        /// Cleans up the handle's resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up the handle's resources.
        /// </summary>
        /// <param name="disposing">If this is True, the method has been called by a user's code. Otherwise, it
        /// has been called by the runtime and only unmanaged resources can be disposed.</param>
        public void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            if (IsCreated)
                m_Handle.Free();

            Disposed = true;
        }

        /// <summary>
        /// The finalizer of the class.
        /// </summary>
        ~RefHandle()
        {
            Dispose(false);
        }
    }
}
