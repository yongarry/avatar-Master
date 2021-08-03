using UnityEngine;
using System.Collections;


[AddComponentMenu("Mesh/Curved Plane")]
public class CurvedPlane : MonoBehaviour {

//	[SerializeField]
//	public bool HideSettings;

	[Header("Plane Generating Settings")]
	[Tooltip("Will ReCreate the plane each time in fixed update. (useful, if you need to change planes shape often")]
	public bool UseFixedUpdate = true;

	[Header("Plane Mesh Settings")]

	[Tooltip("Disableing will set curve coeficients to 0")]
	public bool useCurving = true;

	[Tooltip("Count of triangles in the plane")]
	[SerializeField]
	[Range(2, 100)]
	public int quality = 40;

	[SerializeField]
	[Tooltip("Use default center position. (Recommend enable)")]
	bool defaultCenter = true;

	[SerializeField]
	[Tooltip("Center of the Plane. Radius depends on it")]
	Vector3 m_CustomCenter;




	[SerializeField]
	[Range(0.0f, 2f)]
	[Tooltip("Horizontal Curve Coeficient(Normal = 1)")]
	public float m_CurveCoeficientX = 1;


	[SerializeField]
	[Range(0.0f, 2f)]
	[Tooltip("Vertical Curve Coeficient(Normal = 1)")]
	public float m_CurveCoeficientY = 1;

	[Space(10)]
	[Header("Plane Render Settings")]

	[SerializeField]
	[Tooltip("Material, wich applies to the plane")]
	Material CustomMaterial;
	[Space(5)]
	[SerializeField]
	[Tooltip("Plane curvature radius will depent on this gameobject. (By default will use main camera)")]
	GameObject m_Target;

	[Space(10)]
	[Header("Plane Size Settings")]
	[Tooltip("Setting size of a plane depending on materials texture size")]
	public bool TexturesSize = false;


	[SerializeField]
	[Tooltip("Scaling textures scale to change planes scale")]
	public float Scale = 1;

	[SerializeField]
	[Tooltip("width of a plane, if SizeByImageInMaterial is OFF")]
	[Range(1.0f, 10.0f)]
	public float width = 6f;

	[SerializeField]
	[Tooltip("height of a plane, if SizeByImageInMaterial is OFF")]
	[Range(1.0f, 10.0f)]
	public float height = 4f;


	#region Private values
	Vector3[] vertices;
	Vector3[] normals;
	float m_Radius;
	Vector3 _center;
	Material _material;
	GameObject _target;
	#endregion


	void FixedUpdate()
	{
		if (UseFixedUpdate) {
			UpdatePlane ();
		}
	}

	void OnEnable()
	{
		CreatePlane();
	}

	public void UpdatePlane()
	{
		CreatePlane();
	}

	private Vector3 Center {
		get{
			if (defaultCenter) {
				_center = transform.localPosition;
			} else {
				_center = m_CustomCenter;
			}
			return _center;
		}
		set{
			_center = value;
		}
	}

	public bool GetCurving()
	{
		return useCurving;
	}

	public void SetCurving(bool b)
	{
		useCurving = b;
	}

	public Material material {
		get{
			if (CustomMaterial == null) {
				if(_material == null){
					Material customMaterial = new Material (Shader.Find ("Standard"));
					customMaterial.color = Color.white;
					_material = customMaterial; 
				}
			} else {
				_material = CustomMaterial;
			}
			return _material;
		}
		set{
			_material = value;
		}
	}


	public GameObject Target {
		get{
			if (m_Target == null) {
				if (_target == null) {
					_target = Camera.main.gameObject; 
				}
			} else {
				_target = m_Target;
			}
			return _target;
		}
		set{
			_target = value;
		}
	}

	private void CreatePlane()
	{
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer> ();
		if (renderer == null) {
			renderer = gameObject.AddComponent<MeshRenderer> ();
		}
		renderer.material = material;
		MeshFilter filter = gameObject.GetComponent<MeshFilter>();
		if (filter == null) {
			filter = gameObject.AddComponent<MeshFilter> ();
		}
		Mesh mesh = filter.mesh;
		mesh.Clear();

		int resX = quality; // 2 minimum
		int resY = quality;

		m_Radius = GetRadius(Center/*m_Center.transform.position*/, Target.transform.position);

		#region Vertices		
		float _height = height;
		float _width = width;
		if(TexturesSize){
			if(material.mainTexture == null){
				TexturesSize = false;
				Debug.LogError("No Texture Assigned To The Material -> switching \"SizeByMaterial\" OFF");
				return;
			}else{
				_height = material.mainTexture.height / 100f * Scale;
				_width = material.mainTexture.width / 100f * Scale;
			}
		}

		vertices = new Vector3[resX * resY];
		for (int y = 0; y < resY; y++)
		{
			// [ -length / 2, length / 2 ]
			float yPos = ((float)y / (resY - 1) - .5f) * _height;
			for (int x = 0; x < resX; x++)
			{
				// [ -width / 2, width / 2 ]
				float xPos = ((float)x / (resX - 1) - .5f) * _width;
				//ZCorrection
				if (useCurving)
				{
					vertices[x + y * resX] = CalculateZposition(new Vector3(xPos, yPos, 0));
				}
				else
				{
					vertices[x + y * resX] = new Vector3(xPos, yPos, 0);
				}
			}
		}

		#endregion

		#region Normales
		Vector3[] normales = new Vector3[vertices.Length];
		for (int n = 0; n < normales.Length; n++)
			normales[n] = Vector3.back;
		#endregion

		#region UVs		
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int v = 0; v < resY; v++)
		{
			for (int u = 0; u < resX; u++)
			{
				uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resY - 1));
			}
		}
		#endregion

		#region Triangles
		int nbFaces = (resX - 1) * (resY - 1);
		int[] triangles = new int[nbFaces * 6];
		int t = 0;
		for (int face = 0; face < nbFaces; face++)
		{
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resY - 1) * resX);

			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;

			triangles[t++] = i + resX;
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1;
		}
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();
		;
	}

	private Vector3 CalculateZposition(Vector3 PointPosition)
	{
		//if(m_Radius == 0)
		//{
		//	m_Radius = GetRadius(_center/*m_Center.transform.position*/, m_camera.transform.position);
		//}
		Vector3 newPos = PointPosition;
		if (m_CurveCoeficientX == m_CurveCoeficientY && m_CurveCoeficientX ==  1f)
		{
			newPos.z = GetZdistance(GetDistFromCenter(newPos));
		}else
		{
			float x = GetXdistFromCenter(PointPosition) * m_CurveCoeficientX * transform.localScale.x;
			float y = GetYdistFromCenter(PointPosition) * m_CurveCoeficientY * transform.localScale.y;
			float h = Mathf.Sqrt(x * x + y * y);
			newPos.z = GetZdistance(h);
		}
		return newPos;
	}

	private float GetZdistance(float distFromCenter)
	{
		return -(m_Radius - Mathf.Sqrt(m_Radius * m_Radius - distFromCenter * distFromCenter));
	}


	private float GetXdistFromCenter(Vector3 pos)
	{
		Vector3 center = Center;
		center.z = 0;
		center.y = 0;
		pos.y = 0;
		pos.z = 0;
		return Vector3.Distance(center, pos);
	}

	private float GetYdistFromCenter(Vector3 pos)
	{
		Vector3 center = Center;
		center.z = 0;
		center.x = 0;
		pos.x = 0;
		pos.z = 0;
		return Vector3.Distance(center, pos);
	}

	private float GetDistFromCenter(Vector3 pos)
	{
		pos.z = 0;
		Vector3 center = Center;//m_Center.transform.localPosition;
		center.z = 0;
		return Vector3.Distance(center, pos);
	}

	private float GetRadius(Vector3 center, Vector3 camera)
	{
		//Debug.Log ("Radius = " + Vector3.Distance (center, camera));
		return Vector3.Distance(center, camera);
	}

}
