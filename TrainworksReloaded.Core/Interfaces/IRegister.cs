using System.Diagnostics.CodeAnalysis;

namespace TrainworksReloaded.Core.Interfaces
{
    /// <summary>
    /// A register is a Service for Looking up by either name or id certain game resources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRegister<T> : IRegisterableDictionary<T>
    {
        public bool TryLookupName(
            string name,
            [NotNullWhen(true)] out T? lookup,
            [NotNullWhen(true)] out bool? IsModded
        );
        public bool TryLookupId(
            string id,
            [NotNullWhen(true)] out T? lookup,
            [NotNullWhen(true)] out bool? IsModded
        );
    }
}
