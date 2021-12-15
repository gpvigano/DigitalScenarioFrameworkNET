using System;
using System.Collections;
using System.Collections.Generic;

namespace DiScenFw
{
    /// <summary>
    /// Vector in 3D space.
    /// </summary>
    /// <remarks>
    /// Structure fields are ordered like in a right-handed coordinate system (Z up).
    /// In a right-handed XYZ/Z-up coordinate system: Right = X, Forward = Y, Up = Z.
    /// </remarks>
    [System.Serializable]
    public struct Vector3D
    {
        /// <summary> Position along the right axis. </summary>
        public float Right;

        /// <summary> Position along the forward axis. </summary>
        public float Forward;

        /// <summary> Position along the up axis. </summary>
        public float Up;
    }


    /// <summary>
    /// Local transformation in 3D space.
    /// </summary>
    [System.Serializable]
    public struct LocalTransformData
    {
        /// <summary> Vector of the right axis for this coordinate system. </summary>
        public Vector3D RightAxis;

        /// <summary> Vector of the forward axis for this coordinate system. </summary>
        public Vector3D ForwardAxis;

        /// <summary> Vector of the up axis for this coordinate system. </summary>
        public Vector3D UpAxis;

        /// <summary> Origin of this coordinate system. </summary>
        public Vector3D Origin;

        /// <summary> Scaling (1 = no rescaling). </summary>
        public Vector3D Scale;

        /// <summary> Identifier of the entity to which this transform is relative. </summary>
        public string ParentId;
    }


    /// <summary>
    /// Type of source containing an asset.
    /// </summary>
    [System.Serializable]
    public enum AssetSourceType
    {
        Scene = 0,
        Project,
        External,
        Undefined
    }


    /// <summary>
    /// Reference to an asset defined out of the framework.
    /// </summary>
    [System.Serializable]
    public struct AssetData
    {
        /// <summary> Type of source containing the asset. </summary>
        public AssetSourceType Source;

        /// <summary> Catalog of assets containing this asset. </summary>
        public string Catalog;

        /// <summary> Category of this asset. </summary>
        public string AssetType;

        /// <summary> Uniform resource identifier of this asset. </summary>
        public string Uri;

        /// <summary> Identifier of the asset part. </summary>
        public string PartId;
    }

    
    /// <summary>
    /// Basic entity definition.
    /// </summary>
    [System.Serializable]
    public class EntityData
    {
        /// <summary> Type of entity (e.g. proper class name). </summary>
        public string ClassName;

        /// <summary> Unique identifier of the entity (required). </summary>
        public string Identifier;

        /// <summary> Type of entity. </summary>
        public string Type;

        /// <summary> Entity category (optional). </summary>
        public string Category;

        /// <summary> Entity description (optional). </summary>
        public string Description;

        /// <summary> Entity configuration (optional). </summary>
        public string Configuration;

        /// <summary> Asset reference (if any). </summary>
        public AssetData Asset;
    }


    /// <summary>
    /// Basic element with a local 3D transformation.
    /// </summary>
    [System.Serializable]
    public class ElementData : EntityData
    {
        /// <summary> Local 3D transformation. </summary>
        public LocalTransformData LocalTransform = new LocalTransformData();
    }
}
