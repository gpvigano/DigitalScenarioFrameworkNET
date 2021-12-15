using System;

namespace DiScenFw
{
    /// <summary>
    /// Result of an action.
    /// </summary>
    public enum Result
    {
        /// <summary> Task still in progress. </summary>
        InProgress = 0,
        /// <summary> Failure, something went wrong. </summary>
        Failed, 
        /// <summary> Success, task completed. </summary>
        Succeeded,
        /// <summary> Deadlock, task came to a deadlock. </summary>
		Deadlock,
        /// <summary> Action denied, unable to take the action. </summary>
		Denied
    };


    /// <summary>
    /// Entity condition that can be logically connected to other conditions.
    /// </summary>
    [System.Serializable]
    public struct EntityCondition
    {
        /// <summary> Special tag for any entity. </summary>
		const string ANY = "___ANY_ENTITY___"; 

        /// <summary> Special tag for all the entities. </summary>
		const string ALL = "___ALL_ENTITIES___";

        /// <summary> Identifier of the entity on which the condition is evaluated </summary>
        /// <remarks>
        /// It can be also EntityCondition.ANY or EntityCondition.ALL.
        /// </remarks>
        public string EntityId;

        /// <summary> Name/identifier of the property to be evaluated. </summary>
        public string PropertyId;

        /// <summary> Reference value to be evaluated. </summary>
        public string PropertyValue;
    }


    /// <summary>
    /// Action definition.
    /// </summary>
    public struct ActionData
    {
        /// <summary> Action type encoded as a string (e.g. command). </summary>
        public string ActionId;

        /// <summary> Action parameters encoded as strings. </summary>
        public string[] Params;
    };


    /// <summary>
    /// Generic relationships represented by entity identifiers and relationship identifiers.
    /// </summary>
    public struct RelationshipData
    {
        /// <summary> Identifier of the relationship. </summary>
        public string RelationshipId;

        /// <summary> Identifier of the related relationship. </summary>
        public string RelatedEntityId;

        /// <summary> Identifier of the end point (e.g. an anchor) for the related relationship. </summary>
        public string RelatedEndPoint;
    };


    /// <summary>
    /// Generic properties represented as strings.
    /// </summary>
    public struct PropertyData
    {
        /// <summary> Identifier of the property. </summary>
        public string PropertyId;

        /// <summary> Value of the property. </summary>
        public string PropertyValue;
    };
}
