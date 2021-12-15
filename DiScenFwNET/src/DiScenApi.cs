using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DiScenFw
{
    /// <summary>
    /// Binding to the main Digital Scenario Framework API.
    /// </summary>
    public class DiScenApi
    {
        /// <summary>
        /// Path to the project directory.
        /// </summary>
        /// <remarks>
        /// If set it can be used by external modules to locate the project directory.
        /// </remarks>
        public static string ProjectDir;


        /// <summary>
        /// Event triggered each time the framework need to display a log message.
        /// </summary>
        /// <remarks>
        /// It is importand to implement a method to respont to this event,
        /// otherwise messages are redirected to the system console that can
        /// be not available nor visible.
        /// </remarks>
        public static event DisplayMessageAction DisplayMessageEvent;


        /// <summary>
        /// Event triggered each time a synchronization of the whole scenario from the Virtual Environment is needed.
        /// </summary>
        public static event Action SyncScenarioEvent;


        /// <summary>
        /// Event triggered each time a synchronization of the Virtual Environment from the whole scenario is needed.
        /// </summary>
        public static event Action SyncSceneEvent;


        /// <summary>
        /// Event triggered each time a synchronization of an element in the scenario from a scene object is needed.
        /// </summary>
        public static event SyncElementTransformAction SyncElementTransformEvent;


        /// <summary>
        /// Event triggered each time a synchronization of a scene object from an element in the scenario is needed.
        /// </summary>
        public static event SyncSceneObjectTransformAction SyncSceneObjectTransformEvent;


        /// <summary>
        /// Event triggered each time a linear interpolation bewtween two transformation of an element in the scenario is needed.
        /// </summary>
        public static event LerpElementTransformAction LerpElementTransformEvent;


        /// <summary>
        /// Event triggered each time a simulation history is loaded.
        /// </summary>
        public static event Action SimulationLoadedEvent;


        /// <summary>
        /// Event triggered each time a simulation is updated.
        /// </summary>
        public static event Action SimulationUpdatedEvent;


        /// <summary>
        /// Event triggered each time a simulation starts.
        /// </summary>
        public static event Action SimulationPlayEvent;


        /// <summary>
        /// Event triggered each time a simulation is paused.
        /// </summary>
        public static event Action SimulationPauseEvent;


        /// <summary>
        /// Event triggered each time a simulation is stopped.
        /// </summary>
        public static event Action SimulationStopEvent;


        /// <summary>
        /// Event triggered each time the simulation is changed.
        /// </summary>
        public static event Action<float> SimulationTimeChangedEvent;


        /// <summary>
        /// Check if this API was initialized.
        /// </summary>
        public static bool Initialized
        {
            get
            {
                return initialized;
            }
        }


        /// <summary>
        /// Initialize the scenario API only if it was not yet initialized.
        /// </summary>
        public static void Initialize()
        {
            if (!initialized)
            {
                RegisterProjectDirCallback(_ProjectDirCallback);
                //RegisterTakeScreenshotCallback(_TakeScreenshotCallback);
                RegisterDisplayMessageCallback(_DisplayMessageCallback);
                //RegisterLoadScenarioCallback(_LoadScenarioCallback);
                //RegisterSaveScenarioCallback(_SaveScenarioCallback);
                RegisterSyncScenarioCallback(_SyncScenarioCallback);
                RegisterSyncSceneCallback(_SyncSceneCallback);
                RegisterSyncElementTransformCallback(_SyncElementTransformCallback);
                RegisterSyncSceneObjectTransformCallback(_SyncSceneObjectTransformCallback);
                RegisterLerpElementTransformCallback(_LerpElementTransformCallback);

                initialized = true;
            }
        }


        /// <summary>
        /// Initialize the scenario and simulation API only if it was not yet initialized.
        /// </summary>
        public static void InitializeSimulation()
        {
            Initialize();
            if (!simulationInitialized)
            {
                RegisterSimulationLoadedCallback(_SimulationLoadedCallback);
                RegisterSimulationUpdatedCallback(_SimulationUpdatedCallback);
                RegisterSimulationPlayCallback(_SimulationPlayCallback);
                RegisterSimulationPauseCallback(_SimulationPauseCallback);
                RegisterSimulationStopCallback(_SimulationStopCallback);
                RegisterSimulationTimeChangedCallback(_SimulationTimeChangedCallback);

                simulationInitialized = true;
            }
        }


        /// <summary>
        /// Utility method to display log messages using the defined DisplayMessageEvent.
        /// </summary>
        /// <param name="message">Text of the message to be displayed.</param>
        /// <param name="severity">Severity level (see LogLevel).</param>
        /// <param name="category">Category name, used to identify the source of the message.</param>
        /// <param name="onConsole">Display the message on console (when available).</param>
        /// <param name="onScreen">Display the message on screen (when available).</param>
        /// <param name="msgTag">Reference identifier used to update existing messages instead of creating a new one.</param>
        public static void Log(
            string message, LogLevel severity = LogLevel.Debug, string category = "DiScenFwNET",
            bool onConsole = true, bool onScreen = false, string msgTag = "")
        {
            if (DisplayMessageEvent != null)
            {
                DisplayMessageEvent(severity, message, category, onConsole, onScreen, msgTag);
            }
        }


        /// <summary>
        /// Clear all the scenario data.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearScenario();


        /// <summary>
        /// Update Scenario data (just checking API definition).
        /// </summary>
        /// <param name="timeDelta"></param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateScenario(float timeDelta);


        /// <summary>
        /// Load a JSON text file and fill Scenario data.
        /// </summary>
        /// <param name="filePath">Path to the JSON text file.</param>
        /// <returns></returns>
        public static bool LoadScenario(string filePath)
        {
            Initialize();
            IntPtr ptr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _LoadScenario(ptr,Bool.False);
            Marshal.FreeHGlobal(ptr);
            bool loaded = result == Bool.True;
            if (SyncSceneEvent != null)
            {
                SyncSceneEvent();
            }
            return loaded;
        }


        /// <summary>
        /// Write Scenario data to a JSON text file.
        /// </summary>
        /// <param name="filePath">Path to the JSON text file.</param>
        /// <returns></returns>
        public static bool SaveScenario(string filePath)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _SaveScenario(ptr);
            Marshal.FreeHGlobal(ptr);
            return result == Bool.True;
        }


        /// <summary>
        /// Write Scenario data to a JSON text.
        /// </summary>
        /// <returns>JSON text with encoded scenario data.</returns>
        public static string GetScenarioJson()
        {
            IntPtr ptr = _GetScenarioJson();
            string jsonText = Marshal.PtrToStringAnsi(ptr);
            return jsonText;
        }


        /// <summary>
        /// Parse a JSON text and fill Scenario data.
        /// </summary>
        /// <param name="jsonText">JSON text with encoded scenario data.</param>
        /// <returns>true if the text was successfully parsed, false otherwise.</returns>
        public static bool SetScenarioJson(string jsonText)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(jsonText);
            Bool result = _SetScenarioJson(ptr);
            Marshal.FreeHGlobal(ptr);
            return result == Bool.True;
        }


        /// <summary>
        /// Get a list of entities in the scenario.
        /// </summary>
        /// <returns>Array of entities.</returns>
        public static EntityData[] GetEntities()
        {
            int size = GetEntitiesCount();
            EntityDataRaw[] entitiesPtr = new EntityDataRaw[size];
            int result = _GetEntities(entitiesPtr, ref size);
            EntityData[] entities = new EntityData[size];
            if (result > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    entities[i] = new EntityData();
                    MarshalEntityData(ref entitiesPtr[i], entities[i]);
                }
            }
            return entities;
        }


        /// <summary>
        /// Get a list of elements in the scenario.
        /// </summary>
        /// <returns>Array of elements.</returns>
        public static ElementData[] GetElements()
        {
            int size = GetElementsCount();
            ElementDataRaw[] elementsPtr = new ElementDataRaw[size];
            int result = _GetElements(elementsPtr, ref size);
            ElementData[] elements = new ElementData[size];
            if (result > 0)
            {
                for (int i = 0; i < size; i++)
                {
                    elements[i] = new ElementData();
                    MarshalEntityData(ref elementsPtr[i].Entity, (EntityData)elements[i]);
                    MarshalElementData(ref elementsPtr[i], elements[i]);
                }
            }
            return elements;
        }


        /// <summary>
        /// Get the location of an element.
        /// </summary>
        /// <param name="identifier">Identifier of the element.</param>
        /// <returns>Position in 3D space of the element.</returns>
        public static Vector3D GetElementLocation(string identifier)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(identifier);
            Vector3D loc = _GetElementLocation(ptr);
            Marshal.FreeHGlobal(ptr);
            return loc;
        }


        /// <summary>
        /// Add a list of elements to the scenario.
        /// </summary>
        /// <param name="elements">Array of elements.</param>
        public static void AddElements(ElementData[] elements)
        {
            ElementDataRaw[] elementsRaw = new ElementDataRaw[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                elementsRaw[i] = new ElementDataRaw();
                elementsRaw[i].Entity = new EntityDataRaw();
                MarshalEntityDataRaw((EntityData)elements[i], ref elementsRaw[i].Entity);
                MarshalElementDataRaw(elements[i], ref elementsRaw[i]);
            }
            _AddElements(elementsRaw, elementsRaw.Length);
            for (int i = 0; i < elementsRaw.Length; i++)
            {
                Marshal.FreeHGlobal(elementsRaw[i].Entity.ClassName);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Identifier);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Type);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Category);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Description);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Configuration);

                Marshal.FreeHGlobal(elementsRaw[i].Entity.Asset.Catalog);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Asset.Category);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Asset.Uri);
                Marshal.FreeHGlobal(elementsRaw[i].Entity.Asset.PartId);
                Marshal.FreeHGlobal(elementsRaw[i].Transform.ParentId);
            }
        }


        /// <summary>
        /// Add an element to the scenario.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public static void AddElement(ElementData element)
        {
            ElementDataRaw elementRaw = new ElementDataRaw();
                elementRaw.Entity = new EntityDataRaw();
                MarshalEntityDataRaw((EntityData)element, ref elementRaw.Entity);
                MarshalElementDataRaw(element, ref elementRaw);
            _AddElement(elementRaw);
                Marshal.FreeHGlobal(elementRaw.Entity.ClassName);
                Marshal.FreeHGlobal(elementRaw.Entity.Identifier);
                Marshal.FreeHGlobal(elementRaw.Entity.Type);
                Marshal.FreeHGlobal(elementRaw.Entity.Category);
                Marshal.FreeHGlobal(elementRaw.Entity.Description);
                Marshal.FreeHGlobal(elementRaw.Entity.Configuration);

                Marshal.FreeHGlobal(elementRaw.Entity.Asset.Catalog);
                Marshal.FreeHGlobal(elementRaw.Entity.Asset.Category);
                Marshal.FreeHGlobal(elementRaw.Entity.Asset.Uri);
                Marshal.FreeHGlobal(elementRaw.Entity.Asset.PartId);
                Marshal.FreeHGlobal(elementRaw.Transform.ParentId);
        }


        public static bool DeleteElement(string elementId)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(elementId);
            bool result = _DeleteElement(ptr) == Bool.True;
            Marshal.FreeHGlobal(ptr);
            return result;
        }


        /// <summary>
        /// Clear simulation history data.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearSimulation();


        /// <summary>
        /// Load a simulation history data from a file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file containing the simulation history data.</param>
        /// <returns>true on success, false on failure.</returns>
        public static bool LoadSimulation(string filePath)
        {
            InitializeSimulation();
            IntPtr ptr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _LoadSimulation(ptr);
            Marshal.FreeHGlobal(ptr);
            return result == Bool.True;
        }


        /// <summary>
        /// Save the simulation history data to a file.
        /// </summary>
        /// <param name="filePath">Path to the JSON file for the simulation history data.</param>
        /// <returns>true on success, false on failure.</returns>
        public static bool SaveSimulation(string filePath)
        {
            IntPtr ptr = Marshal.StringToHGlobalAnsi(filePath);
            Bool result = _SaveSimulation(ptr);
            Marshal.FreeHGlobal(ptr);
            return result == Bool.True;
        }


        /// <summary>
        /// Update the simulation (if started).
        /// </summary>
        /// <param name="timeDelta">Time elapsed from the last update in seconds.</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdateSimulation(float timeDelta);


        /// <summary>
        /// Check if a valid simulation history is available.
        /// </summary>
        /// <returns>true if a valid simulation history is available, else false.</returns>
        public static bool ValidSimulation()
        {
            return _ValidSimulation() == Bool.True;
        }


        /// <summary>
        /// Check if the simulation was started.
        /// </summary>
        /// <returns>true if the simulation was started.</returns>
        public static bool SimulationStarted()
        {
            return _SimulationStarted() == Bool.True;
        }


        /// <summary>
        /// Play (start or resume) the simulation.
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PlaySimulation();


        /// <summary>
        /// Pause the simulation (if started).
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PauseSimulation();


        /// <summary>
        /// Stop the simulation (if started).
        /// </summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopSimulation();


        /// <summary>
        /// Set the simulation progress to a given position.
        /// </summary>
        /// <param name="progress">Progress from 0 (start) to 1 (end).</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSimulationProgress(double progress);


        /// <summary>
        /// Compute and get the simulation progress.
        /// </summary>
        /// <returns>The simulation progress from 0 (start) to 1 (end). </returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double ComputeSimulationProgress();


        /// <summary>
        /// Compute and get the simulation progress as a date/time string.
        /// </summary>
        /// <returns>The simulation progress as a date/time string. </returns>
        public static string GetSimulationDateTimeAsString()
        {
            IntPtr ptr = _GetSimulationDateTimeAsString();
            string info = Marshal.PtrToStringAnsi(ptr);
            return info;
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
            initialized = false;
            _Deinitialize();
        }






        private enum Bool
        {
            False = 0,
            True = 1,
        };


        private struct TransformDataRaw
        {
            public Vector3D RightAxis;
            public Vector3D ForwardAxis;
            public Vector3D UpAxis;
            public Vector3D Origin;
            public Vector3D Scale;
            public IntPtr ParentId;
        };


        private struct AssetDataRaw
        {
            public int Source;
            public IntPtr Catalog;
            public IntPtr Category;
            public IntPtr Uri;
            public IntPtr PartId;
        };


        private struct EntityDataRaw
        {
            public IntPtr ClassName;
            public IntPtr Identifier;
            public IntPtr Type;
            public IntPtr Category;
            public IntPtr Description;
            public IntPtr Configuration;
            public AssetDataRaw Asset;
        };


        private struct ElementDataRaw
        {
            public EntityDataRaw Entity;
            public TransformDataRaw Transform;
        };


        private delegate IntPtr ProjectDirCallback();

        private delegate void SyncScenarioCallback();

        private delegate void SyncSceneCallback();

        private delegate void DisplayMessageCallback(
                int severity, IntPtr message, IntPtr category,
                Bool onConsole, Bool onScreen, IntPtr msgTag);

        private delegate void SyncElementTransformCallback(IntPtr elemId,
            ref TransformDataRaw sceneTransform);

        private delegate void SyncSceneObjectTransformCallback(IntPtr elemId,
            ref TransformDataRaw scenarioTransform);

        private delegate void LerpElementTransformCallback(IntPtr elemId,
            ref TransformDataRaw transform1, ref TransformDataRaw transform2, float trim);

        private delegate void SimulationTimeChangeCallback(float progress);

        private delegate void SimulationEventCallback();

#if WIN64
        private const string pluginName = "DiScenFw-x64";
#else
        private const string pluginName = "DiScenFw-x86";
#endif
        private static bool initialized = false;
        private static bool simulationInitialized = false;


        private static IntPtr projectPathPtr = IntPtr.Zero;


        private static IntPtr _ProjectDirCallback()
        {
            if (projectPathPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(projectPathPtr);
            }
            projectPathPtr = Marshal.StringToHGlobalAnsi(ProjectDir);
            return projectPathPtr;
        }


        private static void _DisplayMessageCallback(
            int severityInt, IntPtr messagePtr, IntPtr categoryPtr,
            Bool onConsole, Bool onScreen, IntPtr msgTagPtr)
        {
            LogLevel severity = (LogLevel)severityInt;
            string message = Marshal.PtrToStringAnsi(messagePtr);
            string category = Marshal.PtrToStringAnsi(categoryPtr);
            string msgTag = Marshal.PtrToStringAnsi(msgTagPtr);
            string msg = "[" + category + "] " + message;
            if (DisplayMessageEvent != null)
            {
                DisplayMessageEvent(severity, message, category, onConsole == Bool.True, onScreen == Bool.True, msgTag);
            }
        }


        private static void _SyncScenarioCallback()
        {
            if (SyncScenarioEvent != null)
            {
                SyncScenarioEvent();
            }
        }


        private static void _SyncSceneCallback()
        {
            if (SyncSceneEvent != null)
            {
                SyncSceneEvent();
            }
        }


        private static void _SyncElementTransformCallback(IntPtr elemId,
            ref TransformDataRaw sceneTransform)
        {
            if (SyncElementTransformEvent != null)
            {
                string id = Marshal.PtrToStringAnsi(elemId);
                LocalTransformData localTransformData = new LocalTransformData();
                SyncElementTransformEvent(id, ref localTransformData);
                MarshalTransformDataRaw(localTransformData, ref sceneTransform);
            }
        }


        private static void _SyncSceneObjectTransformCallback(IntPtr elemId,
            ref TransformDataRaw scenarioTransform)
        {
            if (SyncSceneObjectTransformEvent != null)
            {
                string id = Marshal.PtrToStringAnsi(elemId);
                LocalTransformData localTransformData = new LocalTransformData();
                MarshalTransformData(scenarioTransform, ref localTransformData);
                SyncSceneObjectTransformEvent(id, ref localTransformData);
            }
        }


        private static void _LerpElementTransformCallback(IntPtr elemId,
            ref TransformDataRaw transform1, ref TransformDataRaw transform2, float trim)
        {
            if (LerpElementTransformEvent != null)
            {
                string id = Marshal.PtrToStringAnsi(elemId);
                LocalTransformData localTransformData1 = new LocalTransformData();
                MarshalTransformData(transform1, ref localTransformData1);
                LocalTransformData localTransformData2 = new LocalTransformData();
                MarshalTransformData(transform2, ref localTransformData2);
                LerpElementTransformEvent(id, ref localTransformData1, ref localTransformData2, trim);
            }
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterProjectDirCallback(ProjectDirCallback projectDirCallback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterDisplayMessageCallback(DisplayMessageCallback displayMessageCallback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSyncScenarioCallback(SyncScenarioCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSyncSceneCallback(SyncSceneCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSyncElementTransformCallback(SyncElementTransformCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSyncSceneObjectTransformCallback(SyncSceneObjectTransformCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterLerpElementTransformCallback(LerpElementTransformCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSimulationLoadedCallback(SimulationEventCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSimulationUpdatedCallback(SimulationEventCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSimulationPlayCallback(SimulationEventCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSimulationPauseCallback(SimulationEventCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSimulationStopCallback(SimulationEventCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterSimulationTimeChangedCallback(SimulationTimeChangeCallback callback);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LoadScenario")]
        private static extern Bool _LoadScenario(IntPtr filePath, Bool syncVE);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SaveScenario")]
        private static extern Bool _SaveScenario(IntPtr filePath);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetScenarioJson")]
        private static extern IntPtr _GetScenarioJson();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetScenarioJson")]
        private static extern Bool _SetScenarioJson(IntPtr jsonText);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetEntitiesCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetEntities")]
        private static extern int _GetEntities([In, Out] EntityDataRaw[] entitiesData, ref int size);


        private static void MarshalEntityData(ref EntityDataRaw entityRaw, EntityData entity)
        {
            entity.ClassName = Marshal.PtrToStringAnsi(entityRaw.ClassName);
            entity.Identifier = Marshal.PtrToStringAnsi(entityRaw.Identifier);
            entity.Type = Marshal.PtrToStringAnsi(entityRaw.Type);
            entity.Category = Marshal.PtrToStringAnsi(entityRaw.Category);
            entity.Description = Marshal.PtrToStringAnsi(entityRaw.Description);
            entity.Configuration = Marshal.PtrToStringAnsi(entityRaw.Configuration);

            entity.Asset.Source = (AssetSourceType)entityRaw.Asset.Source;
            entity.Asset.Catalog = Marshal.PtrToStringAnsi(entityRaw.Asset.Catalog);
            entity.Asset.AssetType = Marshal.PtrToStringAnsi(entityRaw.Asset.Category);
            entity.Asset.Uri = Marshal.PtrToStringAnsi(entityRaw.Asset.Uri);
            entity.Asset.PartId = Marshal.PtrToStringAnsi(entityRaw.Asset.PartId);
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetElementsCount();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetElements")]
        private static extern int _GetElements([In, Out] ElementDataRaw[] entitiesData, ref int size);


        private static void MarshalElementData(ref ElementDataRaw elementRaw, ElementData element)
        {
            MarshalTransformData(elementRaw.Transform, ref element.LocalTransform);
        }


        private static void MarshalTransformData(TransformDataRaw transformDataRaw, ref LocalTransformData transformData)
        {
            transformData.Origin = transformDataRaw.Origin;
            transformData.RightAxis = transformDataRaw.RightAxis;
            transformData.ForwardAxis = transformDataRaw.ForwardAxis;
            transformData.UpAxis = transformDataRaw.UpAxis;
            transformData.Scale = transformDataRaw.Scale;
            transformData.ParentId = Marshal.PtrToStringAnsi(transformDataRaw.ParentId);
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetElementLocation")]
        private static extern Vector3D _GetElementLocation(IntPtr id);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddElements")]
        private static extern void _AddElements([In, Out] ElementDataRaw[] elements, int size);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "AddElement")]
        private static extern void _AddElement([In, Out] ElementDataRaw element);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DeleteElement")]
        private static extern Bool _DeleteElement(IntPtr elementId);


        private static void MarshalEntityDataRaw(EntityData entity, ref EntityDataRaw entityRaw)
        {
            entityRaw.ClassName = Marshal.StringToHGlobalAnsi(entity.ClassName);
            entityRaw.Identifier = Marshal.StringToHGlobalAnsi(entity.Identifier);
            entityRaw.Type = Marshal.StringToHGlobalAnsi(entity.Type);
            entityRaw.Category = Marshal.StringToHGlobalAnsi(entity.Category);
            entityRaw.Description = Marshal.StringToHGlobalAnsi(entity.Description);
            entityRaw.Configuration = Marshal.StringToHGlobalAnsi(entity.Configuration);

            entityRaw.Asset = new AssetDataRaw();
            entityRaw.Asset.Source = (int)entity.Asset.Source;
            entityRaw.Asset.Catalog = Marshal.StringToHGlobalAnsi(entity.Asset.Catalog);
            entityRaw.Asset.Category = Marshal.StringToHGlobalAnsi(entity.Asset.AssetType);
            entityRaw.Asset.Uri = Marshal.StringToHGlobalAnsi(entity.Asset.Uri);
            entityRaw.Asset.PartId = Marshal.StringToHGlobalAnsi(entity.Asset.PartId);
        }


        private static void MarshalElementDataRaw(ElementData element, ref ElementDataRaw elementRaw)
        {
            //elementRaw.Transform = new TransformDataRaw();
            //elementRaw.Transform.Origin = element.LocalTransform.Origin;
            //elementRaw.Transform.RightAxis = element.LocalTransform.RightAxis;
            //elementRaw.Transform.ForwardAxis = element.LocalTransform.ForwardAxis;
            //elementRaw.Transform.UpAxis = element.LocalTransform.UpAxis;
            //elementRaw.Transform.Scale = element.LocalTransform.Scale;
            //elementRaw.Transform.ParentId = Marshal.StringToHGlobalAnsi(element.LocalTransform.ParentId);

            MarshalTransformDataRaw(element.LocalTransform, ref elementRaw.Transform);
        }


        private static void MarshalTransformDataRaw(LocalTransformData transformData, ref TransformDataRaw transformDataRaw)
        {
            transformDataRaw = new TransformDataRaw();
            transformDataRaw.Origin = transformData.Origin;
            transformDataRaw.RightAxis = transformData.RightAxis;
            transformDataRaw.ForwardAxis = transformData.ForwardAxis;
            transformDataRaw.UpAxis = transformData.UpAxis;
            transformDataRaw.Scale = transformData.Scale;
            transformDataRaw.ParentId = Marshal.StringToHGlobalAnsi(transformData.ParentId);
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "LoadSimulation")]
        private static extern Bool _LoadSimulation(IntPtr filePath);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SaveSimulation")]
        private static extern Bool _SaveSimulation(IntPtr filePath);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ValidSimulation")]
        private static extern Bool _ValidSimulation();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SimulationStarted")]
        private static extern Bool _SimulationStarted();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetSimulationDateTimeAsString")]
        private static extern IntPtr _GetSimulationDateTimeAsString();


        private static void _SimulationLoadedCallback()
        {
            if (SimulationLoadedEvent != null)
            {
                SimulationLoadedEvent();
            }
        }


        private static void _SimulationUpdatedCallback()
        {
            if (SimulationUpdatedEvent != null)
            {
                SimulationUpdatedEvent();
            }
        }


        private static void _SimulationPlayCallback()
        {
            if (SimulationPlayEvent != null)
            {
                SimulationPlayEvent();
            }
        }


        private static void _SimulationPauseCallback()
        {
            if (SimulationPauseEvent != null)
            {
                SimulationPauseEvent();
            }
        }


        private static void _SimulationStopCallback()
        {
            if (SimulationStopEvent != null)
            {
                SimulationStopEvent();
            }
        }


        private static void _SimulationTimeChangedCallback(float progress)
        {
            if (SimulationTimeChangedEvent != null)
            {
                SimulationTimeChangedEvent(progress);
            }
        }


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Deinitialize")]
        private static extern void _Deinitialize();
    }
}
