using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class BufferGeometry4_Meta
{
	public int		version = 0;
	public string	type = "BufferGeometry";
	public string	generator = "PopThreeJsBufferGeometry";
};



[Serializable]
public class BufferGeometry4_Attribute_Float
{
	public BufferGeometry4_Attribute_Float(int VectorSize)
	{ 
		itemSize = VectorSize;
	}

	public List<Vector3>	GetArray_Vector3()
	{
		int Step = 3;
		List<Vector3> OutArray = new List<Vector3> ();
		OutArray.Capacity = array.Count / 3;
		Vector3 Temp = new Vector3 ();
		for (int i = 0;	i < array.Count;	i += Step) {
			Temp.x = array [i + 0];
			Temp.y = array [i + 1];
			Temp.z = array [i + 2];
			OutArray.Add (Temp);
		}
		return OutArray;			
	}

	public List<Vector2>	GetArray_Vector2()
	{
		int Step = 2;
		List<Vector2> OutArray = new List<Vector2> ();
		OutArray.Capacity = array.Count / 2;
		Vector2 Temp = new Vector2 ();
		for (int i = 0;	i < array.Count;	i += Step) {
			Temp.x = array [i + 0];
			Temp.y = array [i + 1];
			OutArray.Add (Temp);
		}
		return OutArray;			
	}

	public List<float>	array = new List<float>();
	public string		type = "Float32Array";
	public bool			normalized = false;
	public int			itemSize = 3;
};


[Serializable]
public class BufferGeometry4_Attribute_Int
{
	public BufferGeometry4_Attribute_Int(int VectorSize=1)
	{ 
		itemSize = VectorSize;
	}

	public List<int>	array = new List<int>();
	public string		type = "Int32Array";
	public bool			normalized = false;
	public int			itemSize = 1;
};


[Serializable]
public class BufferGeometry4_Attributes
{
	public BufferGeometry4_Attribute_Float	position;	//	3x
	public BufferGeometry4_Attribute_Float	normal;		//	3x
	public BufferGeometry4_Attribute_Float	uv;			//	2x
	public BufferGeometry4_Attribute_Float	uv2;		//	2x
	public BufferGeometry4_Attribute_Float	color;		//	3x
};


[Serializable]
public class BufferGeometry4_Data
{
	public BufferGeometry4_Attribute_Int	index;
	public BufferGeometry4_Attributes		attributes;
};


/*
var TYPED_ARRAYS = {
			'Int8Array': Int8Array,
			'Uint8Array': Uint8Array,
			'Uint8ClampedArray': Uint8ClampedArray,
			'Int16Array': Int16Array,
			'Uint16Array': Uint16Array,
			'Int32Array': Int32Array,
			'Uint32Array': Uint32Array,
			'Float32Array': Float32Array,
			'Float64Array': Float64Array
		};
*/


//	https://github.com/mrdoob/three.js/blob/d8b24a1d2d4887a5db77d7112e5ba1539377c905/src/core/Geometry.js#L224   
//	https://github.com/mrdoob/three.js/wiki/JSON-Geometry-format-4
/*
 {
    "metadata": {
        "version": 4,
        "type": "BufferGeometry",
        "generator": "BufferGeometryExporter"
    },
    "data": {
        "attributes": {
            "position": {
                "itemSize": 3,
                "type": "Float32Array",
                "array": [50,50,50,...]
            },
            "normal": {
                "itemSize": 3,
                "type": "Float32Array",
                "array": [1,0,0,...]
            },
            "uv": {
                "itemSize": 2,
                "type": "Float32Array",
                "array": [0,1,...]
            }
        },
        "boundingSphere": {
            "center": [0,0,0],
            "radius": 86.60254037844386
        }
    }
}
*/
public class BufferGeometry4
{
	public BufferGeometry4_Meta		metadata;
	public BufferGeometry4_Data		data;

	delegate void	ModifyVector3(ref Vector3 v);
	delegate void	ModifyVector2(ref Vector2 v);
	delegate void	ModifyColor(ref Color v);


	int WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Vector3[] VectorArray,ModifyVector3 Modify=null)
	{ 
		//	gr: maybe pad here
		if ( VectorArray == null )
			return 0;

		int VectorSize = 3;
		if ( Attribute == null )
			Attribute = new BufferGeometry4_Attribute_Float(VectorSize);

		int FirstIndex = Attribute.array.Count / VectorSize;

		Vector3 Temp = new Vector3();
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			Temp = VectorArray[i];
			if ( Modify != null )
				Modify( ref Temp );
			Attribute.array.Add( Temp.x );
			Attribute.array.Add( Temp.y );
			Attribute.array.Add( Temp.z );
		}
		
		return FirstIndex;
	}

	int WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Vector2[] VectorArray,ModifyVector2 Modify=null)
	{ 
		//	gr: maybe pad here
		if ( VectorArray == null )
			return 0;

		int VectorSize = 2;
		if ( Attribute == null )
			Attribute = new BufferGeometry4_Attribute_Float(VectorSize);

		int FirstIndex = Attribute.array.Count / VectorSize;

		var Temp = new Vector2();
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			Temp = VectorArray[i];
			if ( Modify != null )
				Modify( ref Temp );
			Attribute.array.Add( Temp.x );
			Attribute.array.Add( Temp.y );
		}
		
		return FirstIndex;
	}


	//	for this json it's RGB, not RGBA
	int WriteAttribute(ref BufferGeometry4_Attribute_Float Attribute, Color[] VectorArray,ModifyColor Modify=null)
	{ 
		//	gr: maybe pad here
		if ( VectorArray == null )
			return 0;

		int VectorSize = 3;
		if ( Attribute == null )
			Attribute = new BufferGeometry4_Attribute_Float(VectorSize);

		int FirstIndex = Attribute.array.Count / VectorSize;

		var Temp = new Color();
		for ( int i=0;	i<VectorArray.Length;	i++ )
		{ 
			Temp = VectorArray[i];
			if ( Modify != null )
				Modify( ref Temp );
			Attribute.array.Add( Temp.r );
			Attribute.array.Add( Temp.g );
			Attribute.array.Add( Temp.b );
		}
		
		return FirstIndex;
	}

	void WriteAttribute(ref BufferGeometry4_Attribute_Int Attribute,int[] Indexes,int FirstIndexOffset)
	{
		if ( Attribute == null )
		{
			Attribute = new BufferGeometry4_Attribute_Int();
		}

		//	add new ones offset
		for ( int i=0;	i<Indexes.Length;	i++ )
			Attribute.array.Add( FirstIndexOffset + Indexes[i] );
	}



	void PadAttribute(ref BufferGeometry4_Attribute_Float Attribute)
	{
		if ( Attribute != null )
			return;
		Attribute = new BufferGeometry4_Attribute_Float(1);
	}


	public BufferGeometry4()
	{
	}

	public BufferGeometry4(Mesh mesh,Matrix4x4 PositionTransform,int Version=0) :
	this	( new Mesh[1]{mesh}, PositionTransform, Version )
	{
	}

	public BufferGeometry4(Mesh[] Meshes,Matrix4x4 PositionTransform,int Version=0)
	{ 
		metadata = new BufferGeometry4_Meta();
		metadata.version = Version;
		data = new BufferGeometry4_Data();

		data.attributes = new BufferGeometry4_Attributes();

		ModifyVector3 ReorientatePosition = (ref Vector3 p) =>
		{
			p = PositionTransform.MultiplyPoint(p);

			WorldUnityToThreeJs( ref p );

			/*gr: old method needed setting...
			if ( PositionTransform.HasValue )
			{
				var TransformedP = PositionTransform.Value * new Vector4( p.x, p.y, p.z, 1 );
				p.Set( TransformedP.x, TransformedP.y, TransformedP.z );
			}
			*/
		};	

		//	serialise data
		for ( int m=0;	m<Meshes.Length;	m++ )
		{
			var mesh = Meshes[m];
			int FirstVertex = WriteAttribute( ref data.attributes.position, mesh.vertices, ReorientatePosition );
			WriteAttribute( ref data.attributes.normal, mesh.normals );
			WriteAttribute( ref data.attributes.uv, mesh.uv );
			WriteAttribute( ref data.attributes.uv2, mesh.uv2 );
			WriteAttribute( ref data.attributes.color, mesh.colors );

			//	gr: if an attrib is null, unity still serialises an "empty" class. if its empty.. three js will throw an exception... so make empty attribs
			PadAttribute( ref data.attributes.position );
			PadAttribute( ref data.attributes.normal );
			PadAttribute( ref data.attributes.uv );
			PadAttribute( ref data.attributes.uv2 );
			PadAttribute( ref data.attributes.color );

			//	write triangle indexes
			WriteAttribute( ref data.index, mesh.triangles, FirstVertex );
		}
	}


	public static void WorldUnityToThreeJs(ref Vector3 UnityPos)
	{
		//	+y is up in threejs (by default)
		//p.y = -p.y;
		//	xz seems to be wrong too. Experimenting suggests just z needs correcting
		//p.x = -p.x;
		UnityPos.z = -UnityPos.z;
	}

	//	generate mesh from data
	public Mesh GetMesh()
	{
		var mesh = new Mesh ();

		var Positions = data.attributes.position.GetArray_Vector3 ();
		mesh.SetVertices (Positions);

		if (data.attributes.normal.itemSize == 3) {
			var Normals = data.attributes.normal.GetArray_Vector3 ();
			//	gr: invert
			for (int i = 0;	i < Normals.Count;	i++)
				Normals [i] *= -1;
			mesh.SetNormals (Normals);
		}

		if (data.attributes.uv.itemSize == 2) {
			var Uvs = data.attributes.uv.GetArray_Vector2 ();
			mesh.SetUVs (0, Uvs);
		}
		else if (data.attributes.uv.itemSize == 3) {
			var Uvs = data.attributes.uv.GetArray_Vector3 ();
			mesh.SetUVs (0, Uvs);
		}

		if (data.attributes.uv2.itemSize == 2) {
			var Uvs = data.attributes.uv.GetArray_Vector2 ();
			mesh.SetUVs (1, Uvs);
		}
		else if (data.attributes.uv2.itemSize == 3) {
			var Uvs = data.attributes.uv2.GetArray_Vector3 ();
			mesh.SetUVs (1, Uvs);
		}

		var Indexes = data.index.array;
		mesh.SetIndices (Indexes.ToArray(), MeshTopology.Triangles, 0);

		return mesh;
	}
};


public class PopThreeJsBufferGeometry
{
	static public string GetMeshJsonString(Mesh mesh,Matrix4x4 Transform)
	{
		var Geo = new BufferGeometry4( mesh, Transform );
		return JsonUtility.ToJson( Geo, true );
	}

	static public string GetMeshJsonString(List<Mesh> meshes,Matrix4x4 Transform)
	{
		var Geo = new BufferGeometry4( meshes.ToArray(), Transform );
		return JsonUtility.ToJson( Geo, true );
	}
	
#if UNITY_EDITOR
	static public void SaveMeshFile(Mesh mesh,Matrix4x4 Transform)
	{
		string path = UnityEditor.EditorUtility.SaveFilePanel( "Save mesh json", "", mesh.name + ".json", "json" );
		if( path.Length != 0 )
		{
			SaveMeshFile (path, mesh, Transform);
		}
	}
#endif

	static public void SaveMeshFile(string FilePath,Mesh mesh,Matrix4x4 Transform)
	{
		var Json = PopThreeJsBufferGeometry.GetMeshJsonString( mesh, Transform );
		var FileHandle = File.CreateText( FilePath );
		FileHandle.Write(Json);
		FileHandle.Close();
		PopBrowseToFile.ShowFile( FilePath );
	}


#if UNITY_EDITOR
	[MenuItem("CONTEXT/MeshFilter/Export Mesh as .json...")]
	public static void SaveMeshInPlace (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh mesh = mf.sharedMesh;
		SaveMeshFile( mesh, Matrix4x4.identity );
	}
#endif


#if UNITY_EDITOR
	[MenuItem("CONTEXT/MeshFilter/Export Mesh as .json baked in world space...")]
	public static void SaveMeshInPlaceBaked (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh mesh = mf.sharedMesh;

		var Mtx = mf.transform.localToWorldMatrix;

		SaveMeshFile( mesh, Mtx );
	}
#endif


#if UNITY_EDITOR
	[MenuItem("CONTEXT/MeshFilter/Export Mesh as .asset...")]
	public static void SaveMeshAsAsset (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh mesh = mf.sharedMesh;

		string path = "Assets/Test/xx.asset";
		//string path = UnityEditor.EditorUtility.SaveFolderPanel( "Save mesh asset", "", "" );
		//if (path.Length != 0) 
		{
			try {
				UnityEditor.AssetDatabase.CreateAsset (mesh, path);
				UnityEditor.AssetDatabase.SaveAssets ();
			} catch (System.Exception e) {
				Debug.LogError ("Error exporting mesh " + e.Message);
			}
		}
	}
#endif

}
