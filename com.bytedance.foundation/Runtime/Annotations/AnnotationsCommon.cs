namespace ByteDance.Foundation.Annotation
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines the <see cref="IUpdatable" />.
    /// </summary>
    [ComVisible(true)]
    public interface IUpdatable
    {
        /// <summary>
        /// The Update.
        /// </summary>
        void Update();
    }

    /// <summary>
    /// Defines the <see cref="IAwakable" />.
    /// </summary>
    [ComVisible(true)]
    public interface IAwakable
    {
        /// <summary>
        /// The Awake.
        /// </summary>
        void Awake();
    }

    /// <summary>
    /// Defines the <see cref="IDisposable" />.
    /// </summary>
    [ComVisible(true)]
    public interface IDisposable
    {
        /// <summary>
        /// The Dispose.
        /// </summary>
        void Dispose();
    }
}
