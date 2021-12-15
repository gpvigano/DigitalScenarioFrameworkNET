using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DiScenFw
{

    public class DiScenXp
    {
        private const string pluginName = "DiScenFw";


        private enum Bool
        {
            False = 0,
            True = 1,
        };


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

        // Straight From the c++ Dll (unmanaged)

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NewEpisode();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearCurrentExperience();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearAllExperiences();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetGoals")]
        private static extern int _GetGoals([In, Out] IntPtr[] stringArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetGoalsCount();
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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetCurrentGoal")]
        private static extern IntPtr _GetCurrentGoal();
        public static string GetCurrentGoal()
        {
            IntPtr ptr = _GetCurrentGoal();
            string goalName = Marshal.PtrToStringAnsi(ptr);
            return goalName;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "RemoveGoal")]
        private static extern Bool _RemoveGoal(IntPtr goalName);
        public static bool RemoveGoal(string goalName)
        {
            IntPtr goalNamePtr = Marshal.StringToHGlobalAnsi(goalName);
            Bool result = _RemoveGoal(goalNamePtr);
            Marshal.FreeHGlobal(goalNamePtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetCurrentGoal")]
        private static extern Bool _SetCurrentGoal(IntPtr goalName);
        public static bool SetCurrentGoal(string goalName)
        {
            IntPtr goalNamePtr = Marshal.StringToHGlobalAnsi(goalName);
            Bool result = _SetCurrentGoal(goalNamePtr);
            Marshal.FreeHGlobal(goalNamePtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LoadDigitalSystem")]
        private static extern Bool _LoadDigitalSystem(IntPtr goalName);
        public static bool LoadDigitalSystem(string digitalSystemPath)
        {
            IntPtr digitalSystemNamePtr = Marshal.StringToHGlobalAnsi(digitalSystemPath);
            Bool result = _LoadDigitalSystem(digitalSystemNamePtr);
            Marshal.FreeHGlobal(digitalSystemNamePtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddNewGoal")]
        private static extern Bool _AddNewGoal(IntPtr goalName);
        public static bool AddNewGoal(string goalName)
        {
            IntPtr goalNamePtr = Marshal.StringToHGlobalAnsi(goalName);
            Bool result = _AddNewGoal(goalNamePtr);
            Marshal.FreeHGlobal(goalNamePtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResetSuccessCondition();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddSuccessCondition")]
        private static extern void _AddSuccessCondition(IntPtr entityId, IntPtr propertyId, IntPtr propertyValue);
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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetSuccessConditionsCount();
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSuccessConditions")]
        private static extern Bool _GetSuccessConditions([In, Out] CondPtr[] successConditionsData, ref int size);
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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LoadCurrentExperience")]
        private static extern Bool _LoadCurrentExperience(IntPtr filePath);
        public static bool LoadCurrentExperience(string filePath)
        {
            IntPtr filePathPtr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _LoadCurrentExperience(filePathPtr);
            Marshal.FreeHGlobal(filePathPtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SaveCurrentExperience")]
        private static extern Bool _SaveCurrentExperience(IntPtr filePath);
        public static bool SaveCurrentExperience(string filePath)
        {
            IntPtr filePathPtr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _SaveCurrentExperience(filePathPtr);
            Marshal.FreeHGlobal(filePathPtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Result LastResult();

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DoAction")]
        private static extern Result _DoAction(IntPtr actionId, IntPtr[] parameters, int paramsCount, Bool updateXp);
        public static Result DoAction(string actionId, string[] parameters, bool updateXp)
        {
            IntPtr[] paramPtr = new IntPtr[parameters.Length];
            IntPtr actionPtr = Marshal.StringToHGlobalAnsi(actionId);
            for (int i = 0; i < paramPtr.Length; i++)
                paramPtr[i] = Marshal.StringToHGlobalAnsi(parameters[i]);
            Result result = _DoAction(actionPtr, paramPtr, parameters.Length, updateXp ? Bool.True : Bool.False);

            Marshal.FreeHGlobal(actionPtr);
            foreach (IntPtr par in paramPtr)
                Marshal.FreeHGlobal(par);
            return result;
        }

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Do")]
        private static extern Result _Do(IntPtr action, Bool updateXp);
        public static Result Do(ActionData actionData, bool updateXp)
        {
            ActionDataPtr action;
            ConvertActionToPtr(actionData, out action);
            IntPtr actionPtr = Marshal.AllocHGlobal(Marshal.SizeOf(action));
            Result result = Result.UNACCOMPLISHED;
            try
            {
                Marshal.StructureToPtr(action, actionPtr, false);
                result = _Do(actionPtr, updateXp ? Bool.True : Bool.False);
            }
            finally
            {
                // Free the unmanaged memory.
                FreeActionPtr(ref action);
                Marshal.FreeHGlobal(actionPtr);
            }
            return result;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetAvailableActions")]
        private static extern int _GetAvailableActions([In, Out] IntPtr[] ptrArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetAvailableActionsCount();

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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetForbiddenActions")]
        private static extern int _GetForbiddenActions([In, Out] IntPtr[] ptrArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetForbiddenActionsCount();

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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSuggestedActions")]
        private static extern int _GetSuggestedActions([In, Out] IntPtr[] ptrArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetSuggestedActionsCount();

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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetScenarioEntities")]
        private static extern int _GetScenarioEntities([In, Out] IntPtr[] stringArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetScenarioEntitiesCount();
        public static string[] GetScenarioEntities()
        {
            int count = GetScenarioEntitiesCount();
            IntPtr[] entityIds = new IntPtr[count];
            _GetScenarioEntities(entityIds, ref count);
            string[] entities = new string[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = Marshal.PtrToStringAnsi(entityIds[i]);
            }
            return entities;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetChangedEntities")]
        private static extern int _GetChangedEntities([In, Out] IntPtr[] stringArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetChangedEntitiesCount();
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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityProperties")]
        private static extern int _GetEntityProperties(IntPtr entityId, [In, Out] PropPtr[] propsArray, ref int size);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetEntityPropertiesCount(IntPtr entityId);

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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetEntityRelationshipsCount(IntPtr entityId);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityRelationships")]
        private static extern int _GetEntityRelationships(IntPtr entityId, [In, Out] RelPtr[] propsArray, ref int size);

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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityProperty")]
        private static extern IntPtr _GetEntityProperty(IntPtr entityId, IntPtr propertyId);
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

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "CheckEntityProperty")]
        private static extern Bool _CheckEntityProperty(IntPtr entityId, IntPtr propertyId, IntPtr propertyValue);
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


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "EntityHasRelationship")]
        private static extern Bool _EntityHasRelationship(IntPtr entityId, IntPtr relationshipId);
        public static bool EntityHasRelationship(string entityId, string relationshipId)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr relationshipIdPtr = Marshal.StringToHGlobalAnsi(relationshipId);
            Bool result = _EntityHasRelationship(entityIdPtr, relationshipIdPtr);
            Marshal.FreeHGlobal(entityIdPtr);
            Marshal.FreeHGlobal(relationshipIdPtr);
            return result == Bool.True;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetInfo")]
        private static extern IntPtr _GetInfo(IntPtr infoId);
        public static string GetInfo(string infoId)
        {
            IntPtr infoIdPtr = Marshal.StringToHGlobalAnsi(infoId);
            IntPtr ptr = _GetInfo(infoIdPtr);
            string info = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(infoIdPtr);
            return info;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetConfiguration")]
        private static extern IntPtr _GetConfiguration();
        public static string GetConfiguration()
        {
            IntPtr ptr = _GetConfiguration();
            string info = Marshal.PtrToStringAnsi(ptr);
            return info;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetConfiguration")]
        private static extern Bool _SetConfiguration(IntPtr configString);
        public static bool SetConfiguration(string configString)
        {
            IntPtr configStringPtr = Marshal.StringToHGlobalAnsi(configString);
            bool result = _SetConfiguration(configStringPtr) == Bool.True;
            Marshal.FreeHGlobal(configStringPtr);
            return result;
        }


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

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TrainAssistant")]
        private static extern Result _TrainAssistant(Bool updateXp, Bool agentLearning);
        public static Result TrainAssistant(bool updateXp, bool agentLearning)
        {
            return _TrainAssistant(updateXp ? Bool.True : Bool.False, agentLearning ? Bool.True : Bool.False);
        }

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetDeadlockDetection")]
        private static extern void _SetDeadlockDetection(Bool enabled);
        public static void SetDeadlockDetection(bool enabled)
        {
            _SetDeadlockDetection(enabled ? Bool.True : Bool.False);
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntityType")]
        private static extern IntPtr _GetEntityType(IntPtr entityId);
        public static string GetEntityType(string entityId)
        {
            IntPtr entityIdPtr = Marshal.StringToHGlobalAnsi(entityId);
            IntPtr ptr = _GetEntityType(entityIdPtr);
            string entityTypeId = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(entityIdPtr);
            if (string.IsNullOrEmpty(entityTypeId))
            {
                //DiScenApi.Log("GetEntityType(null)", LogLevel.Error);
                return "";
            }
            //DiScenApi.Log(entityId + " is a " + entityTypeId);
            return entityTypeId;
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPossiblePropertiesCount(IntPtr entityTypeId);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPossibleProperties")]
        private static extern Bool _GetPossibleProperties(IntPtr entityTypeId, [In, Out] IntPtr[] stringArray, ref int size);

        public static string[] GetPossibleProperties(string entityTypeId)
        {
            IntPtr entityTypeIdPtr = Marshal.StringToHGlobalAnsi(entityTypeId);
            int count = GetPossiblePropertiesCount(entityTypeIdPtr);
            string[] properties = new string[count];
            if (count == 0)
            {
                // TODO: provide a common messaging framework (see DiScenAPI)
                //Debug.LogWarningFormat("No property found for entity type {0}.", entityTypeId);
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

        public static string[] GetPossibleEntityProperties(string entityId)
        {
            string entityTypeId = GetEntityType(entityId);
            if (string.IsNullOrEmpty(entityTypeId))
            {
                return new string[0];
            }
            return GetPossibleProperties(entityTypeId);
        }



        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetPossiblePropertyValuesCount(IntPtr entityTypeId, IntPtr propertyId);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetPossiblePropertyValues")]
        private static extern Bool _GetPossiblePropertyValues(IntPtr entityTypeId, IntPtr propertyId, [In, Out] IntPtr[] stringArray, ref int size);

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

        public static string[] GetPossibleEntityPropertyValues(string entityId, string propertyId)
        {
            return GetPossiblePropertyValues(GetEntityType(entityId), propertyId);
        }



    }
}
