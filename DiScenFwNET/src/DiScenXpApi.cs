using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DiScenFw
{

    /// <summary>
    /// Binding to the experience API of Digital Scenario Framework .
    /// </summary>
    /// <remarks>
    /// Please note that entities here are conceptual abstraction,
    /// while in DiScenApi entities and elements are components of the scenario.
    /// </remarks>
    public class DiScenXpApi
    {
        /// <summary>
        /// Start a new episode.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NewEpisode();


        /// <summary>
        /// Clear the experience related to the current goal.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearCurrentExperience();


        /// <summary>
        /// Clear all the experiences.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearAllExperiences();


        /// <summary>
        /// Get a list of goals.
        /// </summary>
        /// <returns>The list of the names of stored goals.</returns>
        public static string[] GetGoals()
        {
            int count = GetGoalsCount();
            IntPtr[] names = new IntPtr[count];
            _GetGoals(names, ref count);
            string[] goalNames = new string[count];
            for (int i = 0; i < count; i++)
            {
                goalNames[i] = Marshal.PtrToStringAnsi(names[i]);
            }
            return goalNames;
        }


        /// <summary>
        /// Get the current goal name.
        /// </summary>
        /// <returns>The current goal name.</returns>
        public static string GetCurrentGoal()
        {
            IntPtr ptr = _GetCurrentGoal();
            string goalName = Marshal.PtrToStringAnsi(ptr);
            return goalName;
        }


        /// <summary>
        /// Remove the experience related to the goal with the given name.
        /// </summary>
        /// <param name="goalName">Name of the goal to be removed.</param>
        /// <returns>true if the goal was removed, false if the named goal does not exist.</returns>
        public static bool RemoveGoal(string goalName)
        {
            IntPtr goalNamePtr = Marshal.StringToHGlobalAnsi(goalName);
            Bool result = _RemoveGoal(goalNamePtr);
            Marshal.FreeHGlobal(goalNamePtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Set the named goal and related experience as the current one.
        /// </summary>
        /// <param name="goalName">Name of the goal to select.</param>
        /// <returns>true if the goal was selected, false if the named goal does not exist.</returns>
        public static bool SetCurrentGoal(string goalName)
        {
            IntPtr goalNamePtr = Marshal.StringToHGlobalAnsi(goalName);
            Bool result = _SetCurrentGoal(goalNamePtr);
            Marshal.FreeHGlobal(goalNamePtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Load a digital system implementation.
        /// </summary>
        /// <param name="cyberSystemPath">Path to the digital system implementation (DLL) without extension.</param>
        /// <returns>true if the digital system implementation was successfully loaded, false otherwise.</returns>
        public static bool LoadCyberSystem(string cyberSystemPath)
        {
            IntPtr cyberSystemNamePtr = Marshal.StringToHGlobalAnsi(cyberSystemPath);
            Bool result = _LoadCyberSystem(cyberSystemNamePtr);
            Marshal.FreeHGlobal(cyberSystemNamePtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Check if a digital system implementation was loaded.
        /// </summary>
        /// <returns>true if a digital system implementation was successfully loaded, false otherwise.</returns>
        public static bool IsCyberSystemLoaded()
        {
            return _IsCyberSystemLoaded() == Bool.True;
        }


        /// <summary>
        /// Add a new goal (the related experience is created).
        /// </summary>
        /// <param name="goalName">Name of the goal to be added.</param>
        /// <returns>true if the goal is adde, false if the goal name is empty or already exists.</returns>
        public static bool AddNewGoal(string goalName)
        {
            IntPtr goalNamePtr = Marshal.StringToHGlobalAnsi(goalName);
            Bool result = _AddNewGoal(goalNamePtr);
            Marshal.FreeHGlobal(goalNamePtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Reset the success condition for the current experience.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResetSuccessCondition();


        /// <summary>
        /// Add a new success condition to the current experience.
        /// </summary>
        /// <param name="entityId">Identifier of the entity to be evaluated.</param>
        /// <param name="propertyId">Identifier of the entity property to be evaluated.</param>
        /// <param name="propertyValue">Value of the entity property to be compared.</param>
        public static void AddSuccessCondition(string entityId, string propertyId, string propertyValue)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr propertyIdPtr = Marshal.StringToHGlobalAnsi(propertyId);
            IntPtr valuePtr = Marshal.StringToHGlobalAnsi(propertyValue);
            _AddSuccessCondition(entityIdPtr, propertyIdPtr, valuePtr);
            Marshal.FreeHGlobal(entityIdPtr);
            Marshal.FreeHGlobal(propertyIdPtr);
            Marshal.FreeHGlobal(valuePtr);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static EntityCondition[] GetSuccessConditions()
        {
            int size = GetSuccessConditionsCount();
            CondPtr[] condArray = new CondPtr[size];
            Bool result = _GetSuccessConditions(condArray, ref size);
            EntityCondition[] successConditions = new EntityCondition[size];
            if (result > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    successConditions[i] = new EntityCondition();
                    successConditions[i].EntityId = Marshal.PtrToStringAnsi(condArray[i].Id);
                    successConditions[i].PropertyId = Marshal.PtrToStringAnsi(condArray[i].PropId);
                    successConditions[i].PropertyValue = Marshal.PtrToStringAnsi(condArray[i].Val);
                }
            }
            return successConditions;
        }


        /// <summary>
        /// Load the current experience from a JSON text file.
        /// </summary>
        /// <param name="filePath">Path to the JSON text file.</param>
        /// <returns>true if successfully loaded, false otherwise.</returns>
        public static bool LoadCurrentExperience(string filePath)
        {
            IntPtr filePathPtr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _LoadCurrentExperience(filePathPtr);
            Marshal.FreeHGlobal(filePathPtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Save the current experience to a JSON text file.
        /// </summary>
        /// <param name="filePath">Path to the JSON text file.</param>
        /// <returns>true if successfully saved, false otherwise.</returns>
        public static bool SaveCurrentExperience(string filePath)
        {
            IntPtr filePathPtr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _SaveCurrentExperience(filePathPtr);
            Marshal.FreeHGlobal(filePathPtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Get the result of the last action.
        /// </summary>
        /// <returns>The result of the last action.</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result LastResult();


        /// <summary>
        /// Take an action, optionally updating the current experience.
        /// </summary>
        /// <param name="actionId">Action identifier.</param>
        /// <param name="parameters">Action parameters encoded as strings.</param>
        /// <param name="updateXp">Update the current experience.</param>
        /// <returns>The result of the executed action.</returns>
        public static Result TakeAction(string actionId, string[] parameters, bool updateXp)
        {
            IntPtr[] paramPtr = new IntPtr[parameters.Length];
            IntPtr actionIdPtr = Marshal.StringToHGlobalAnsi(actionId);
            for (int i = 0; i < paramPtr.Length; i++)
                paramPtr[i] = Marshal.StringToHGlobalAnsi(parameters[i]);
            Result result = _TakeAction(actionIdPtr, paramPtr, parameters.Length, updateXp ? Bool.True : Bool.False);

            Marshal.FreeHGlobal(actionIdPtr);
            foreach (IntPtr par in paramPtr)
                Marshal.FreeHGlobal(par);
            return result;
        }


        /// <summary>
        /// Take an action, optionally updating the current experience.
        /// </summary>
        /// <param name="actionData">Action data structure.</param>
        /// <param name="updateXp">Update the current experience.</param>
        /// <returns>The result of the executed action.</returns>
        public static Result TakeAction(ActionData actionData, bool updateXp)
        {
            return TakeAction(actionData.ActionId, actionData.Params, updateXp);
        }


        /// <summary>
        /// Get a list of entity names defined in the scenario.
        /// </summary>
        /// <returns>The list of entity names defined in the scenario.</returns>
        public static string[] GetScenarioEntities()
        {
            int count = GetScenarioEntitiesCount();
            if (count == 0)
            {
                return new string[0];
            }
            IntPtr[] entityIds = new IntPtr[count];
            _GetScenarioEntities(entityIds, ref count);
            string[] entities = new string[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = Marshal.PtrToStringAnsi(entityIds[i]);
            }
            return entities;
        }


        /// <summary>
        /// Get a list of names of the changed entities in the scenario.
        /// </summary>
        /// <returns>The list of names of the changed entities in the scenario.</returns>
        public static string[] GetChangedEntities()
        {
            int count = GetChangedEntitiesCount();
            IntPtr[] entityIds = new IntPtr[count];
            _GetChangedEntities(entityIds, ref count);
            string[] entities = new string[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = Marshal.PtrToStringAnsi(entityIds[i]);
            }
            return entities;
        }


        /// <summary>
        /// Get the list of properties of the entity with the given identifier.
        /// </summary>
        /// <param name="entityId">Identifier of the entity.</param>
        /// <returns>The properties of the entity.</returns>
        public static PropertyData[] GetEntityProperties(string entityId)
        {
            IntPtr idPtr = Marshal.StringToHGlobalAnsi(entityId);
            int size = GetEntityPropertiesCount(idPtr);
            PropPtr[] propsArray = new PropPtr[size];
            int result = _GetEntityProperties(idPtr, propsArray, ref size);
            PropertyData[] props = new PropertyData[size];
            if (result > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    props[i] = new PropertyData();
                    props[i].PropertyId = Marshal.PtrToStringAnsi(propsArray[i].Id);
                    props[i].PropertyValue = Marshal.PtrToStringAnsi(propsArray[i].Value);
                }
            }
            Marshal.FreeHGlobal(idPtr);
            return props;
        }


        /// <summary>
        /// Get a list of relationships of the entity with the given identifier.
        /// </summary>
        /// <param name="entityId">Identifier of the entity.</param>
        /// <returns>The relationships of the entity.</returns>
        public static RelationshipData[] GetEntityRelationships(string entityId)
        {
            IntPtr idPtr = Marshal.StringToHGlobalAnsi(entityId);
            int size = GetEntityRelationshipsCount(idPtr);
            RelPtr[] relArray = new RelPtr[size];
            int result = _GetEntityRelationships(idPtr, relArray, ref size);
            RelationshipData[] relationships = new RelationshipData[size];
            if (result > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    relationships[i] = new RelationshipData();
                    relationships[i].RelationshipId = Marshal.PtrToStringAnsi(relArray[i].Id);
                    relationships[i].RelatedEntityId = Marshal.PtrToStringAnsi(relArray[i].EntityId);
                    relationships[i].RelatedEndPoint = Marshal.PtrToStringAnsi(relArray[i].EndPoint);
                }
            }
            return relationships;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public static string GetEntityProperty(string entityId, string propertyId)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr propertyIdPtr = Marshal.StringToHGlobalAnsi(propertyId);
            IntPtr propertyValuePtr = _GetEntityProperty(entityIdPtr, propertyIdPtr);
            string propertyValue = Marshal.PtrToStringAnsi(propertyValuePtr);
            Marshal.FreeHGlobal(entityIdPtr);
            Marshal.FreeHGlobal(propertyIdPtr);
            return propertyValue;
        }


        /// <summary>
        /// Check if the given property of the given entity is set to the given value.
        /// </summary>
        /// <param name="entityId">Identifier of the entity.</param>
        /// <param name="propertyId">Identifier of the property.</param>
        /// <param name="propertyValue">Value to be compared.</param>
        /// <returns>true if the given property of the given entity is set to the given value, false otherwise.</returns>
        public static bool CheckEntityProperty(string entityId, string propertyId, string propertyValue)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr propertyIdPtr = Marshal.StringToHGlobalAnsi(propertyId);
            IntPtr valueIdPtr = Marshal.StringToHGlobalAnsi(propertyValue);
            Bool result = _CheckEntityProperty(entityIdPtr, propertyIdPtr, valueIdPtr);
            Marshal.FreeHGlobal(entityIdPtr);
            Marshal.FreeHGlobal(propertyIdPtr);
            Marshal.FreeHGlobal(valueIdPtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Check if the given entity has the given relationship.
        /// </summary>
        /// <param name="entityId">Identifier of the entity.</param>
        /// <param name="relationshipId">Identifier of the relationship.</param>
        /// <returns>true if the given entity has the given relationship, false otherwise.</returns>
        public static bool EntityHasRelationship(string entityId, string relationshipId)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr relationshipIdPtr = Marshal.StringToHGlobalAnsi(relationshipId);
            Bool result = _EntityHasRelationship(entityIdPtr, relationshipIdPtr);
            Marshal.FreeHGlobal(entityIdPtr);
            Marshal.FreeHGlobal(relationshipIdPtr);
            return result == Bool.True;
        }


        /// <summary>
        /// Get the digital system information.
        /// </summary>
        /// <param name="infoId">Optional identifier of the information needed.</param>
        /// <returns>The required digital system information (or an empty string if not available).</returns>
        public static string GetInfo(string infoId = "")
        {
            IntPtr infoIdPtr = Marshal.StringToHGlobalAnsi(infoId);
            IntPtr ptr = _GetInfo(infoIdPtr);
            string info = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(infoIdPtr);
            return info;
        }


        /// <summary>
        /// Get the digital system configuration encoded as a string.
        /// </summary>
        /// <returns>The digital system configuration encoded as a string (or an empty string if not available).</returns>
        public static string GetConfiguration()
        {
            IntPtr ptr = _GetConfiguration();
            string info = Marshal.PtrToStringAnsi(ptr);
            return info;
        }


        /// <summary>
        /// Set the digital system configuration encoded as a string.
        /// </summary>
        /// <param name="configString">The digital system configuration encoded as a string.</param>
        /// <returns>true if the configuration was successful, false otherwise.</returns>
        public static bool SetConfiguration(string configString)
        {
            IntPtr configStringPtr = Marshal.StringToHGlobalAnsi(configString);
            bool result = _SetConfiguration(configStringPtr) == Bool.True;
            Marshal.FreeHGlobal(configStringPtr);
            return result;
        }

        /// <summary>
        /// Get the configuration of an entity in the digital system configuration encoded as a string.
        /// </summary>
        /// <param name="entityId">Identifier of the entity.</param>
        /// <returns>The entity configuration encoded as a string (or an empty string if not available).</returns>
        public static string GetEntityConfiguration(string entityId)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr ptr = _GetEntityConfiguration(entityIdPtr);
            string info = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(entityIdPtr);
            return info;
        }


        /// <summary>
        /// Set the configuration of an entity in the digital system encoded as a string.
        /// </summary>
        /// <param name="entityId">Identifier of the entity to be configured.</param>
        /// <param name="configString">The entity configuration encoded as a string.</param>
        /// <returns>true if the configuration was successful, false otherwise.</returns>
        public static bool SetEntityConfiguration(string entityId, string configString)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr configStringPtr = Marshal.StringToHGlobalAnsi(configString);
            bool result = _SetEntityConfiguration(entityIdPtr, configStringPtr) == Bool.True;
            Marshal.FreeHGlobal(configStringPtr);
            Marshal.FreeHGlobal(entityIdPtr);
            return result;
        }


        /// <summary>
        /// Do a single training step for the current assistant.
        /// </summary>
        /// <param name="updateXp">Update the current experience.</param>
        /// <param name="agentLearning">Enable the internal agent to learn from its actions.</param>
        /// <returns>The result of the last action.</returns>
        public static Result TrainAssistant(bool updateXp, bool agentLearning)
        {
            return _TrainAgent(updateXp ? Bool.True : Bool.False, agentLearning ? Bool.True : Bool.False);
        }


        /// <summary>
        /// Enable deadlock detection, comparing recent states to detect loops.
        /// </summary>
        /// <param name="enabled">Enable deadlock detection.</param>
        public static void SetDeadlockDetection(bool enabled)
        {
            _SetDeadlockDetection(enabled ? Bool.True : Bool.False);
        }


        /// <summary>
        /// Get the type of the given entity.
        /// </summary>
        /// <param name="entityId">Entity identifier.</param>
        /// <returns>Type of the given entity (or empty string if not available).</returns>
        public static string GetEntityStateType(string entityId)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr ptr = _GetEntityStateType(entityIdPtr);
            string entityTypeId = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(entityIdPtr);
            if (string.IsNullOrEmpty(entityTypeId))
            {
                //DiScenApi.Log("GetEntityStateType(null)", LogLevel.Error);
                return "";
            }
            //DiScenApi.Log(entityId + " is a " + entityTypeId);
            return entityTypeId;
        }


        /// <summary>
        /// Get a list of properties that can be related to the given entity type.
        /// </summary>
        /// <param name="entityTypeId">Identifier of the entity type to inspect.</param>
        /// <returns>The list of properties that can be related to the given entity type.</returns>
        public static string[] GetPossibleProperties(string entityTypeId)
        {
            IntPtr entityTypeIdPtr = Marshal.StringToHGlobalAnsi(entityTypeId);
            int count = GetPossiblePropertiesCount(entityTypeIdPtr);
            string[] properties = new string[count];
            if (count == 0)
            {
                // TODO: debug message here? (see DiScenApi)
                //DiScenApi.Log("No property found for entity type " + entityTypeId, WARNING);
                return properties; // empty
            }
            IntPtr[] propertyIds = new IntPtr[count];
            _GetPossibleProperties(entityTypeIdPtr, propertyIds, ref count);
            for (int i = 0; i < count; i++)
            {
                properties[i] = Marshal.PtrToStringAnsi(propertyIds[i]);
            }
            Marshal.FreeHGlobal(entityTypeIdPtr);
            return properties;
        }


        /// <summary>
        /// Get a list of properties that can be related to the given entity.
        /// </summary>
        /// <param name="entityId">Identifier of the entity to inspect.</param>
        /// <returns>The list of properties that can be related to the given entity.</returns>
        public static string[] GetPossibleEntityProperties(string entityId)
        {
            string entityTypeId = GetEntityStateType(entityId);
            if (string.IsNullOrEmpty(entityTypeId))
            {
                return new string[0];
            }
            return GetPossibleProperties(entityTypeId);
        }


        /// <summary>
        /// Get a list of property values that can be related to the given property of the given entity type.
        /// </summary>
        /// <param name="entityTypeId">Identifier of the entity type to inspect.</param>
        /// <param name="propertyId">Identifier of the property to inspect.</param>
        /// <returns>The list of property values that can be related to the given property of the given entity type (empty list if not defined).</returns>
        public static string[] GetPossiblePropertyValues(string entityTypeId, string propertyId)
        {
            IntPtr entityTypeIdPtr = Marshal.StringToHGlobalAnsi(entityTypeId);
            IntPtr propertyIdPtr = Marshal.StringToHGlobalAnsi(propertyId);
            int count = GetPossiblePropertyValuesCount(entityTypeIdPtr, propertyIdPtr);
            string[] values = new string[count];
            if (count == 0)
            {
                // TODO: provide a common messaging framework (see DiScenAPI)
                //Debug.LogWarningFormat("No property value found for property {0} in entity type {1}.", propertyId, entityTypeId);
                return values; // empty
            }
            IntPtr[] valuesPtr = new IntPtr[count];
            _GetPossiblePropertyValues(entityTypeIdPtr, propertyIdPtr, valuesPtr, ref count);
            for (int i = 0; i < count; i++)
            {
                values[i] = Marshal.PtrToStringAnsi(valuesPtr[i]);
            }
            Marshal.FreeHGlobal(entityTypeIdPtr);
            Marshal.FreeHGlobal(propertyIdPtr);
            return values;
        }


        /// <summary>
        /// Get a list of property values that can be related to the given property of the given entity.
        /// </summary>
        /// <param name="entityId">Identifier of the entity to inspect.</param>
        /// <param name="propertyId">Identifier of the property to inspect.</param>
        /// <returns>The list of property values that can be related to the given property of the given entity (empty list if not defined).</returns>
        public static string[] GetPossibleEntityPropertyValues(string entityId, string propertyId)
        {
            return GetPossiblePropertyValues(GetEntityStateType(entityId), propertyId);
        }


        /// <summary>
        /// Get a list of suggested actions with the current scenario state.
        /// </summary>
        /// <returns>The list of suggested actions (empty if no experience or no suggestion).</returns>
        public static ActionData[] GetSuggestedActions()
        {
            int count = GetSuggestedActionsCount();
            IntPtr[] ptrArray = new IntPtr[count];
            _GetSuggestedActions(ptrArray, ref count);

            ActionData[] actions = new ActionData[count];
            for (int i = 0; i < count; i++)
            {
                actions[i] = ConvertPtrToAction(ptrArray[i]);
            }
            return actions;
        }


        /// <summary>
        /// Get a list of forbidden actions with the current scenario state.
        /// </summary>
        /// <returns>The list of forbidden actions (empty if no experience or no forbidden action).</returns>
        public static ActionData[] GetForbiddenActions()
        {
            int count = GetForbiddenActionsCount();
            IntPtr[] ptrArray = new IntPtr[count];
            _GetForbiddenActions(ptrArray, ref count);

            ActionData[] actions = new ActionData[count];
            for (int i = 0; i < count; i++)
            {
                actions[i] = ConvertPtrToAction(ptrArray[i]);
            }
            return actions;
        }


        /// <summary>
        /// Get a list of available actions with the current scenario state.
        /// </summary>
        /// <returns>The list of available actions (empty if no available action).</returns>
        public static ActionData[] GetAvailableActions()
        {
            int count = GetAvailableActionsCount();
            IntPtr[] ptrArray = new IntPtr[count];
            _GetAvailableActions(ptrArray, ref count);

            ActionData[] actions = new ActionData[count];
            for (int i = 0; i < count; i++)
            {
                actions[i] = ConvertPtrToAction(ptrArray[i]);
            }
            return actions;
        }


        /// <summary>
        /// Reset the API.
        /// </summary>
        /// <remarks>
        /// This method must be called if the environment is reset without unloading
        /// the DLL (e.g. in Unity, when play mode is stopped).
        /// </remarks>
        public static void Deinitialize()
        {
            _Deinitialize();
        }




#if WIN64
        private const string pluginName = "DiScenFw-x64";
#else
        private const string pluginName = "DiScenFw-x86";
#endif

        private enum Bool
        {
            False = 0,
            True = 1,
        };

#pragma warning disable 0649

        private struct ActionDataPtr
        {
            public IntPtr ActionId;
            public IntPtr[] Params;
            public int ParamCount;
        }


        private struct PropPtr
        {
            public IntPtr Id;
            public IntPtr Value;
        };


        private struct RelPtr
        {
            public IntPtr Id;
            public IntPtr EntityId;
            public IntPtr EndPoint;
        };


        private struct CondPtr
        {
            public IntPtr Id;
            public IntPtr PropId;
            public IntPtr Val;
        };

#pragma warning restore 0649


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetGoals")]
        private static extern int _GetGoals([In, Out] IntPtr[] stringArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetGoalsCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetCurrentGoal")]
        private static extern IntPtr _GetCurrentGoal();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "RemoveGoal")]
        private static extern Bool _RemoveGoal(IntPtr goalName);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetCurrentGoal")]
        private static extern Bool _SetCurrentGoal(IntPtr goalName);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LoadCyberSystem")]
        private static extern Bool _LoadCyberSystem(IntPtr goalName);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsCyberSystemLoaded")]
        private static extern Bool _IsCyberSystemLoaded();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddNewGoal")]
        private static extern Bool _AddNewGoal(IntPtr goalName);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddSuccessCondition")]
        private static extern void _AddSuccessCondition(IntPtr entityId, IntPtr propertyId, IntPtr propertyValue);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetSuccessConditionsCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSuccessConditions")]
        private static extern Bool _GetSuccessConditions([In, Out] CondPtr[] successConditionsData, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LoadCurrentExperience")]
        private static extern Bool _LoadCurrentExperience(IntPtr filePath);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SaveCurrentExperience")]
        private static extern Bool _SaveCurrentExperience(IntPtr filePath);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TakeAction")]
        private static extern Result _TakeAction(IntPtr actionId, IntPtr[] parameters, int paramsCount, Bool updateXp);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetAvailableActions")]
        private static extern int _GetAvailableActions([In, Out] IntPtr[] ptrArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetAvailableActionsCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetForbiddenActions")]
        private static extern int _GetForbiddenActions([In, Out] IntPtr[] ptrArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetForbiddenActionsCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSuggestedActions")]
        private static extern int _GetSuggestedActions([In, Out] IntPtr[] ptrArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetSuggestedActionsCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetScenarioEntities")]
        private static extern int _GetScenarioEntities([In, Out] IntPtr[] stringArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetScenarioEntitiesCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetChangedEntities")]
        private static extern int _GetChangedEntities([In, Out] IntPtr[] stringArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetChangedEntitiesCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityProperties")]
        private static extern int _GetEntityProperties(IntPtr entityId, [In, Out] PropPtr[] propsArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetEntityPropertiesCount(IntPtr entityId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetEntityRelationshipsCount(IntPtr entityId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityRelationships")]
        private static extern int _GetEntityRelationships(IntPtr entityId, [In, Out] RelPtr[] propsArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityProperty")]
        private static extern IntPtr _GetEntityProperty(IntPtr entityId, IntPtr propertyId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "CheckEntityProperty")]
        private static extern Bool _CheckEntityProperty(IntPtr entityId, IntPtr propertyId, IntPtr propertyValue);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "EntityHasRelationship")]
        private static extern Bool _EntityHasRelationship(IntPtr entityId, IntPtr relationshipId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetInfo")]
        private static extern IntPtr _GetInfo(IntPtr infoId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetConfiguration")]
        private static extern IntPtr _GetConfiguration();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetConfiguration")]
        private static extern Bool _SetConfiguration(IntPtr configString);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityConfiguration")]
        private static extern IntPtr _GetEntityConfiguration(IntPtr entityId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetEntityConfiguration")]
        private static extern Bool _SetEntityConfiguration(IntPtr entityId, IntPtr configString);


        private static void ConvertActionToPtr(ActionData actionData, out ActionDataPtr action)
        {
            action.ActionId = Marshal.StringToHGlobalAnsi(actionData.ActionId);
            action.Params = new IntPtr[actionData.Params.Length];
            for (int i = 0; i < actionData.Params.Length; i++)
            {
                action.Params[i] = Marshal.StringToHGlobalAnsi(actionData.Params[i]);
            }
            action.ParamCount = actionData.Params.Length;
        }

        private static void FreeActionPtr(ref ActionDataPtr action)
        {
            Marshal.FreeHGlobal(action.ActionId);
            for (int i = 0; i < action.ParamCount; i++)
                Marshal.FreeHGlobal(action.Params[i]);
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetActionParamsCount(IntPtr actionPtr);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetAction")]
        private static extern void _GetAction(IntPtr actionPtr, ref IntPtr actionIdPtr, [In, Out] IntPtr[] ptrArray, ref int size);


        private static ActionData ConvertPtrToAction(IntPtr actionPtr)
        {
            int paramsCount = GetActionParamsCount(actionPtr);

            ActionDataPtr actionDataPtr = new ActionDataPtr();
            actionDataPtr.ActionId = new IntPtr(0);
            actionDataPtr.Params = new IntPtr[paramsCount];
            actionDataPtr.ParamCount = actionDataPtr.Params.Length;

            _GetAction(actionPtr, ref actionDataPtr.ActionId, actionDataPtr.Params, ref actionDataPtr.ParamCount);
            ActionData actionData = new ActionData();
            actionData.ActionId = Marshal.PtrToStringAnsi(actionDataPtr.ActionId);
            actionData.Params = new string[actionDataPtr.ParamCount];
            for (int i = 0; i < actionDataPtr.ParamCount; i++)
                actionData.Params[i] = Marshal.PtrToStringAnsi(actionDataPtr.Params[i]);
            return actionData;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TrainAgent")]
        private static extern Result _TrainAgent(Bool updateXp, Bool agentLearning);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetDeadlockDetection")]
        private static extern void _SetDeadlockDetection(Bool enabled);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityStateType")]
        private static extern IntPtr _GetEntityStateType(IntPtr entityId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPossiblePropertiesCount(IntPtr entityTypeId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPossibleProperties")]
        private static extern Bool _GetPossibleProperties(IntPtr entityTypeId, [In, Out] IntPtr[] stringArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPossiblePropertyValuesCount(IntPtr entityTypeId, IntPtr propertyId);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPossiblePropertyValues")]
        private static extern Bool _GetPossiblePropertyValues(IntPtr entityTypeId, IntPtr propertyId, [In, Out] IntPtr[] stringArray, ref int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DeinitializeXp")]
        private static extern void _Deinitialize();
    }
}
