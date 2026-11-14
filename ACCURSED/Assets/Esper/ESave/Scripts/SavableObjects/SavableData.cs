//***************************************************************************************
// Writer: Esper
// Contact: https://www.espergames.com/contact
//***************************************************************************************

namespace Esper.ESave.SavableObjects
{
    [System.Serializable]
    public class SavableData<T> : SavableObject
    {
        public T value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="value">The value.</param>
        public SavableData(string id, T value)
        {
            this.id = id;
            this.value = value;
        }
    }
}