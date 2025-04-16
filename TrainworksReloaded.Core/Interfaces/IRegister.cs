using System.Diagnostics.CodeAnalysis;
using TrainworksReloaded.Core.Enum;

namespace TrainworksReloaded.Core.Interfaces
{
    /// <summary>
    /// A register is a Service for Looking up by either name or id certain game resources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRegister<T> : IRegisterableDictionary<T>
    {
        /// <summary>
        /// gets all stored identifiers of a given identifier type
        /// </summary>
        /// <param name="identifierType">The type of identifier to get</param>
        /// <returns>A list of all identifiers of the given type</returns>
        public List<string> GetAllIdentifiers(RegisterIdentifierType identifierType);

        /// <summary>
        /// attempts to get a stored lookup item by identifier
        /// </summary>
        /// <param name="identifier">The identifier of the item to lookup</param>
        /// <param name="identifierType">The type of identifier to lookup</param>
        /// <param name="lookup">The item if found</param>
        /// <param name="IsModded">Whether the item is modded</param>
        public bool TryLookupIdentifier(
            string identifier,
            RegisterIdentifierType identifierType,
            [NotNullWhen(true)] out T? lookup,
            [NotNullWhen(true)] out bool? IsModded
        );
    }
}
